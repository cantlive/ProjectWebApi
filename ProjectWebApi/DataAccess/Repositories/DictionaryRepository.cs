using System.Linq;
using BIZ.ExternalIntegration.ASP.MVC;
using ProjectWebApi.Models;
using ScriptLibrary;

namespace ProjectWebApi.DataAccess.Repositories
{
    public class DictionaryRepository1 : IDictionaryRepository1
    {
        public InventoryResponse GetContractor()
        {
            return BIZApplicationInitializer.RunInCurrentSession(() => new InventoryResponse
            {
                Name = new DataContext().dcr_Contractors.Select(x => x.Name).FirstOrDefault()
            });
        }
    }
}