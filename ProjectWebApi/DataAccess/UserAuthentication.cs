using System;
using System.Text;
using BIZ.Core.Common.Script;
using BIZ.ExternalIntegration.ASP.MVC;
using BIZ.ExternalIntegration.Common;
using ProjectWebApi.Attributes;
using ScriptLibrary;

namespace ProjectWebApi.DataAccess
{
    [WebApiExceptionFilter]
    public static class UserAuthentication
    {
        public static dcr_Employees CurrentEmployee { get; private set; }
        public static string CurrentLogin { get; private set; }
        public static string CurrentPassword { get; private set; }

        public static void LogIn(string authHeader)
        {
            if (authHeader == null || authHeader.StartsWith("Basic") == false)
                throw new ScriptException("You are not authorized");

            string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = usernamePassword.IndexOf(':');

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);

            if (BIZApplicationInitializer.IsAuthenticated() && username == CurrentLogin && password == CurrentPassword)
                return;

            BIZApplicationInitializer.RemoveLocalSession();
            var authInfo = BIZAuthInfo.Create(username, password, null);
            if (BIZApplicationInitializer.RegisterSession(authInfo))
            {
                BIZApplicationInitializer.RunInCurrentSession(() =>
                {
                    OrgPatternChanger.Instance.SetOrgPattern();
                    CurrentEmployee = Global.GetCurrentEmp();
                    CurrentLogin = username;
                    CurrentPassword = password;
                });
                return;
            }

            throw new ScriptException("You are not authorized");
        }
    }
}