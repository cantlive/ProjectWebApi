using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using BIZ.Core.Common.Script;

namespace ProjectWebApi.Attributes
{
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var ex = GetScriptException(actionExecutedContext.Exception);

            if (ex != null)
                actionExecutedContext.Exception = ex;

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new
                {
                    message = ex?.Message,
                    exceptionMessage = ex?.Message,
                    exceptionType = ex?.GetType().FullName
                });

            base.OnException(actionExecutedContext);
        }

        private ScriptException GetScriptException(Exception ex)
        {
            switch (ex)
            {
                case null: return null;
                case ScriptException scriptEx: return scriptEx;
                default: return GetScriptException(ex.InnerException);
            }
        }
    }
}