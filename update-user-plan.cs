#r "System.Xml"
#r "System.Xml.Linq"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Xml.Linq;
using System.IO;
using System.Text;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    string planCode = GetEnvironmentVariable("TARGET_PLAN_VALUE"); //Add this in Settings > Configuration. 
                                                                   //Set to the ID of the plan which you want your org's users to have.
    string apiKey = GetEnvironmentVariable("3SCALE_API_KEY");//Add this in Settings > Configuration. 
    string baseUrl = GetEnvironmentVariable("3SCALE_BASE_URL");//Add this in Settings > Configuration. EXAMPLE: https://tc-admin.dev.api.canada.ca/admin/api
    string emailDomain = GetEnvironmentVariable("EMAIL_DOMAIN").ToUpper(); //Add this in Settings > Configuration. EXAMPLE: "@tc.gc.ca"
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

    XElement request = XElement.Parse(requestBody);
    string accountId = request.Element("object").Element("account").Element("id").Value;

    string url = baseUrl + "/accounts/" + accountId + ".xml?access_token=" + apiKey;
    string accountRaw = Get(url);
    XElement account = XElement.Parse(accountRaw);

    string emailAddress = account.Element("users").Element("user").Element("email").Value.ToUpper();

    if(emailAddress.Contains(emailDomain)){
        if((emailAddress.LastIndexOf(emailDomain) + emailDomain.Length) == emailAddress.Length){
            url = baseUrl + "/accounts/" + accountId + "/change_plan.xml?access_token=" + apiKey + "&plan_id=" + planCode;
            Put(url);
        }
    }

    return new OkObjectResult("OK");
}

public static string GetEnvironmentVariable(string name)
{
    return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}

public static string Get(string uri)
{
    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

    using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
    using(Stream stream = response.GetResponseStream())
    using(StreamReader reader = new StreamReader(stream))
    {
        return reader.ReadToEnd();
    }
}

public static string Put(string uri, string data = "", string contentType = "utf-8", string method = "PUT")
{
    byte[] dataBytes = Encoding.UTF8.GetBytes(data);

    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
    request.ContentLength = dataBytes.Length;
    request.ContentType = contentType;
    request.Method = method;

    using(Stream requestBody = request.GetRequestStream())
    {
        requestBody.Write(dataBytes, 0, dataBytes.Length);
    }

    using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
    using(Stream stream = response.GetResponseStream())
    using(StreamReader reader = new StreamReader(stream))
    {
        return reader.ReadToEnd();
    }
}