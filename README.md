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

in `<httpModules>` section under `<system.web>`.  You can safely add both if
you want to.  It won't hurt. This is useful when you are testing in Cassini and
deploying on IIS on AppHarbor.)  Also, make sure `mode="None"` attribute is set
on your `<authentication>` tag (under `<system.web>`).

You also need to add the encryption keys you want to use under `<appSettings>`.
There is a console application provided in the repository, named [`KeyGenerator`](https://github.com/appharbor/AppHarbor.Web.Security/tree/master/KeyGenerator),
that will automatically generate keys for you and print out configuration keys
that you can copy and paste directly.

**It is not a good idea to use online services to generate encryption keys.**
You should also avoid using the keys used to protect the cookie for other
purposes.

In code, you need to add a `using` directive:

    using AppHarbor.Web.Security;

and instead of using `FormsAuthentication` class (and its `SetAuthCookie`
and `SignOut` methods), you should use an instance of [`CookieAuthenticator`](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AppHarbor.Web.Security/CookieAuthenticator.cs) class:

    IAuthenticator authenticator = new CookieAuthenticator(); // or grab one from your IoC container
    authenticator.SetCookie(userName, rememberMe);
    authenticator.SignOut();


# How It Works #

The underlying architecture is quite similar to the way the built-in forms
authentication works: [an HTTP module](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AppHarbor.Web.Security/CookieAuthenticationModule.cs#L7) 
intercepts [the `AuthenticateRequest` event](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AppHarbor.Web.Security/CookieAuthenticationModule.cs#L21) of the application, and retrieves and validates the authentication
cookie.  Based on the information in the cookie, the HTTP module creates
a new `Identity` object and [sets the `User` property of the current `HttpContext`](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AppHarbor.Web.Security/CookieAuthenticationModule.cs#L35) appropriately.

It does not support the cookieless authentication functionality that the
built-in forms authentication provides.

In order to provide revocable session functionality, you need to keep
track of the [GUID assigned to each `AuthenticationCookie` issued](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AppHarbor.Web.Security/CookieAuthenticator.cs#L22).  You can
then modify the [`AuthenticationCookie.IsExpired` method](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AppHarbor.Web.Security/AuthenticationCookie.cs#L111) to return `true` if
the session GUID is revoked in the database.


# Security #

To protect the cookie from tampering, we use Rijndael (AES)
algorithm to encrypt the cookie data, and then sign the encrypted data with
HMAC-SHA256.  This Encrypt-then-Sign scheme is recommended by well-known
cryptographers, Mihir Bellare and Chanathip Namprempre, in their paper
[Authenticated Encryption: Relations among notions and analysis of the generic
composition paradigm](http://charlotte.ucsd.edu/~mihir/papers/oem.pdf).  Given
secure underlying encryption and signing algorithms, this scheme is deemed
secure and is not known to be vulnerable to [Padding Oracle Attacks, like the
one ASP.NET v4.0 forms authentication sufferred from recently](http://netifera.com/research/poet/ieee-aspnetcrypto.pdf).

The other advantage of this solution relative to the ASP.NET's built-in
offering is that ASP.NET reuses the same set of keys that it uses for
forms authentication in other places like ViewState encryption with
varying levels of criticality.  Our solution lets you use a unique set
of keys just for authentication.

You can change the default
[encryption](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AppHarbor.Web.Security/ConfigFileAuthenticationConfiguration.cs#L37)
and
[validation](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AppHarbor.Web.Security/ConfigFileAuthenticationConfiguration.cs#L64)
algorithms by modifying the appropriate properties in the
[`ConfigFileAuthenticationConfiguration`
class](https://github.com/appharbor/AppHarbor.Web.Security/blob/master/AppHarbor.Web.Security/ConfigFileAuthenticationConfiguration.cs). 


# Frequently Asked Questions #

## Does it only work on AppHarbor? ##

No, you can use it in any ASP.NET application.
