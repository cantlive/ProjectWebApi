using System;
using System.Text;
using BIZ.Core.Common.Host.Modules.Facades;
using BIZ.Core.Common.Script;
using BIZ.ExternalIntegration.ASP.MVC;
using BIZ.ExternalIntegration.Common;
using ProjectWebApi.Attributes;
using ProjectWebApi.Models;
using ScriptLibrary;

namespace ProjectWebApi.DataAccess
{
    [WebApiExceptionFilter]
    public static class UserAuthentication
    {
        public static EmployeeResponse LogIn(string authHeader)
        {
            if (authHeader == null || authHeader.StartsWith("Basic") == false)
                throw new ScriptException("You are not authorized");

            string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = usernamePassword.IndexOf(':');

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);

            EmployeeResponse currentUser = GetCurrentUser();
            bool validPassword = IsPasswordValid(username, password);
            if (BIZApplicationInitializer.IsAuthenticated() && (username != currentUser.Name || validPassword == false))
                BIZApplicationInitializer.RemoveLocalSession();

            if (username == currentUser.Name && validPassword)
                return currentUser;

            var authInfo = BIZAuthInfo.Create(username, password, null); 
            if (BIZApplicationInitializer.RegisterSession(authInfo))
            {
                Guid uid = default;
                string name = string.Empty;

                BIZApplicationInitializer.RunInCurrentSession(() =>
                {
                    OrgPatternChanger.Instance.SetOrgPattern();
                    dcr_Employees employee = Global.GetCurrentEmp();

                    if (employee != null)
                    {
                        uid = employee.UID;
                        name = employee.Name;
                    }                   
                });

                return new EmployeeResponse { UID = uid, Name = name };
            }
            else
            {
                BIZApplicationInitializer.RemoveLocalSession();
                throw new ScriptException("You are not authorized");
            }
        }

        public static EmployeeResponse GetCurrentUser() 
        {
            Guid uid = default;
            string name = string.Empty;

            BIZApplicationInitializer.RunInCurrentSession(() =>
            {
                dcr_Employees employee = Global.GetCurrentEmp();

                if (employee != null)
                {
                    uid = employee.UID;
                    name = employee.Name;
                }
            });

            return new EmployeeResponse { UID = uid, Name = name };
        }

        private static bool IsPasswordValid(string username, string password) 
        {
            return BIZApplicationInitializer.RunInCurrentSession(() =>
            {
                return DataStorageClass.Current.SecurityProvider.ValidateUser(username, password);
            });
        }
    }
}