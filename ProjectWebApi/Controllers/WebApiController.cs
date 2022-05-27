using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using ProjectWebApi.Attributes;
using ProjectWebApi.DataAccess;
using ProjectWebApi.DataAccess.Repositories;
using ProjectWebApi.Models;

namespace ProjectWebApi.Controllers
{
    [WebApiExceptionFilter]
    public class WebApiController : ApiController
    {
        private readonly IDocumentRepository _documentRepository;

        public WebApiController()
        {
            _documentRepository = new DocumentRepository();
        }

        [HttpGet]
        public List<DocumentResponse> GetPIDocuments([FromUri] DocumentRequest request)
        {
            LogIn();
            return _documentRepository.GetPIDocuments(request);
        }

        [HttpGet]
        public List<DocumentResponse> GetIDNDocuments([FromUri] DocumentRequest request)
        {
            LogIn();
            return _documentRepository.GetIDNDocuments(request);
        }

        [HttpGet]
        public string GetLogin()
        {
            return UserAuthentication.CurrentLogin;
        }

        public void LogIn()
        {
            HttpContext httpContext = HttpContext.Current;
            string authHeader = httpContext.Request.Headers["Authorization"];
            UserAuthentication.LogIn(authHeader);
        }
    }
}