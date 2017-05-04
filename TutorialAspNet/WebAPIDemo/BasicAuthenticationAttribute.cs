using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Diagnostics;

namespace WebAPIDemo
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                string authenticationToken = actionContext.Request.Headers
                                            .Authorization.Parameter;

                Debug.WriteLine(string.Format("authenticationToken {0}", authenticationToken));

                string decodedAuthenticationToken = Encoding.UTF8.GetString(
                    Convert.FromBase64String(authenticationToken));

                Debug.WriteLine(string.Format("decodedAuthenticationToken {0}", decodedAuthenticationToken));

                string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                string username = usernamePasswordArray[0];
                string password = usernamePasswordArray[1];

                Debug.WriteLine(string.Format("username {0}", username));
                Debug.WriteLine(string.Format("password {0}", password));

                if (EmployeeSecurity.Login(username, password))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(username), null);

                    Debug.WriteLine("Login Successed.");
                }
                else
                {
                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);

                    Debug.WriteLine("Login failed.");
                }
            }
        }
    }
}