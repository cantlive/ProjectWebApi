using System.Web.Http;
using ProjectWebApi.Attributes;
using ProjectWebApi.DataAccess.Repositories;
using ProjectWebApi.Models;

namespace ProjectWebApi.Controllers
{
    [WebApiExceptionFilter]
    public class WebApiController : ApiController
    {
        private readonly IDictionaryRepository1 _dictionaryRepository;

        public WebApiController(IDictionaryRepository1 dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public InventoryResponse GetInv([FromUri] InventoryRequest request)
        {
            return _dictionaryRepository.GetContractor();
        }
    }
}