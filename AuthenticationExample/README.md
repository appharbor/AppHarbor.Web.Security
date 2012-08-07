# Sample Project - Important, Read Carefully: #

This sample is a modified version of the default ASP.NET MVC 3 sample web
project that Visual Studio creates to show how to use `CookieAuthentication`
instead of `FormsAuthentication`.  Before you use this sample, note the
following. DO NOT DEPLOY THE EXAMPLE WITHOUT DOING THE FOLLOWING:

In order to test easily without requiring a SQL database, this sample simply
checks if your password is equal to the string `"validpassword"`.  You need
to change [`AccountController.ValidateCredentials` method](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AuthenticationExample/Controllers/AccountController.cs#L11) to actually authenticate users with a real database.

You need to [change the encryption keys in the `web.config` file](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AuthenticationExample/Web.config#L18). You can use the provided [`KeyGenerator` project](https://github.com/appharbor/AppHarbor.Web.Security/tree/master/KeyGenerator) to generate new
keys.
