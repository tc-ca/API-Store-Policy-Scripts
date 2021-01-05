To get started, create a new Azure Function with the options "publish" = "code" and "runtime stack" = ".NET Core".

Go to Functions and add a new HTTP Trigger function.

Go to "Code + Test". Click on "Get function URL" and save this in your 3scale webhook settings. Make sure that webhooks are enabled, along with the appropriate event(s).

Add in the code from the script that you wish to use. Press "save".

In the script, you will see a comment asking you to make sure that certain environment variables are set.
Take note of those and navigate to Settings > Configuration from the Function App.

Add in the variables, set appropriate values, and select "save".
