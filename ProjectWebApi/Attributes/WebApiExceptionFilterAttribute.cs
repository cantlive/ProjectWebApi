using System;
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