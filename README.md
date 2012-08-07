&nbsp;
# Introduction #

ASP.NET has a built-in Forms Authentication system to issue and validate
cookies to authenticated users.  While having the authentication system in the
framework makes a lot of things easier for developers, there is one major issue
that may occur when running an application on multiple servers in a
load-balanced environment: the authentication algorithm might change as a
result of a patch to the framework, and since patches may not be applied to all
servers running the same application atomically, cookies issued by one server
might be deemed invalid by the other, causing weird authentication issues and
requiring the user to login again.

To mitigate that, we created an extensible authentication system, that can
readily replace ASP.NET forms authentication, and can be easily adapted to
support features like revocable cookies.

# How to Use #

It's easy to use this authentication solution.  You simply need to add a
reference to `AppHarbor.Web.Security` assembly (and bin-deploy it) and make
these changes to your root `Web.config` file:

In `<system.webServer>` section, add the following:

    <add name="AppHarbor.Web.Security.CookieAuthenticationModule" 
         type="AppHarbor.Web.Security.CookieAuthenticationModule, AppHarbor.Web.Security"
         preCondition="integratedMode" />

(if you are using IIS6 or Cassini, you need to add:
     
    <add name="AppHarbor.Web.Security.CookieAuthenticationModule" 
         type="AppHarbor.Web.Security.CookieAuthenticationModule, AppHarbor.Web.Security" />

in `<httpModules>` section under `<system.web>`.  You can add safely add both if
you want to.  It won't hurt. This is useful when you are testing in Cassini and
deploying on IIS on AppHarbor.)  Also, make sure `mode="None"` attribute is set
on your `<authentication>` tag (under `<system.web>`).

You also need to add the encryption keys you want to use under `<appSettings>`.
There is a console application provided in the repository, named `KeyGenerator`,
that will automatically generate keys for you and prints out configuration keys
that you can copy and paste directly.

It is not a good idea to use online services to generate encryption keys.

In code, you need to add a `using` directive:

    using AppHarbor.Web.Security;

and instead of using `FormsAuthentication` class (and its `SetAuthCookie`
and `SignOut` methods), you should use `CookieAuthentication` instead:

    CookieAuthentication.SetAuthCookie(userName, rememberMe);
    CookieAuthentication.SignOut();

# Frequently Asked Questions #

## Does it only work on AppHarbor? ##

No, you can use it in any ASP.NET application.
