using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppHarbor.Web.Security
{
    public interface ICookieValidator
    {
        bool IsCookieValid(AuthenticationCookie cookie);
    }
}
