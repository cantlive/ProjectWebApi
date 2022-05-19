using System.Collections.Generic;
using ProjectWebApi.Models;

namespace ProjectWebApi.DataAccess.Repositories
{
    public interface IDocumentRepository
    {
        List<DocumentResponse> GetPIDocuments(DocumentRequest request);
        List<DocumentResponse> GetIDNDocuments(DocumentRequest request);
    }
}