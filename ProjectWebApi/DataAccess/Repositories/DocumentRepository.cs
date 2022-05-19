using System.Collections.Generic;
using System.Linq;
using Akforta.eLeed.Common;
using BIZ.Core.Common.Script;
using BIZ.Core.Common.Utilities;
using BIZ.ExternalIntegration.ASP.MVC;
using ProjectWebApi.Models;
using ScriptLibrary;

namespace ProjectWebApi.DataAccess.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        public List<DocumentResponse> GetPIDocuments(DocumentRequest request)
        {
            return BIZApplicationInitializer.RunInCurrentSession(() =>
            {
                var dc = new DataContext();
                List<doc_ReceiptBill> documents = dc.doc_ReceiptBill
                    .Where(x => x.TimePosition >= request.BeginDate &&
                                x.TimePosition <= request.EndDate &&
                                x.ID_OrgPattern == OrgPattern.CurrentForEdit.ID)
                    .ToList();

                Dictionary<CommonObject, string> barcodesStatuses = GetLastBarcodesStatuses(documents.Select(x => x.ID).ToList());

                return documents
                    .Select(x => new DocumentResponse
                    {
                        UID = x.UID,
                        Number = x.Number.NumberString,
                        Status = x.STATE.ToString(),
                        TimePosition = x.TimePosition.ToString("dd.MM.yyyy HH:mm:ss"),
                        Type = x.GetType().ToStringSafe(),
                        BarcodeStatus = barcodesStatuses.ContainsKey(x) ? barcodesStatuses[x] : string.Empty
                    })
                    .ToList(); ;
            });
        }

        public List<DocumentResponse> GetIDNDocuments(DocumentRequest request)
        {
            return BIZApplicationInitializer.RunInCurrentSession(() =>
            {
                var dc = new DataContext();
                List<doc_ExpensesInvoice> documents = dc.doc_ExpensesInvoice
                    .Where(x => x.TimePosition >= request.BeginDate &&
                                x.TimePosition <= request.EndDate &&
                                x.ID_OrgPattern == OrgPattern.CurrentForEdit.ID)
                    .ToList();

                Dictionary<CommonObject, string> barcodesStatuses = GetLastBarcodesStatuses(documents.Select(x => x.ID).ToList());

                return documents.Select(x => new DocumentResponse
                {
                    UID = x.UID,
                    Number = x.Number.NumberString,
                    Status = x.STATE.ToString(),
                    TimePosition = x.TimePosition.ToString("dd.MM.yyyy HH:mm:ss"),
                    Type = x.GetType().ToStringSafe(),
                    BarcodeStatus = barcodesStatuses.ContainsKey(x) ? barcodesStatuses[x] : string.Empty
                })
                .ToList();
            });
        }

        private Dictionary<CommonObject, string> GetLastBarcodesStatuses(List<OID> documents)
        {
            var result = new Dictionary<CommonObject, string>();

            var filter = FilterOp.ZeroResult;
            if (documents.Count > 0)
                filter = new FilterOp("[Reason] IN ({0}) AND [OrgPattern] = {1}", documents, OrgPattern.CurrentForEdit);
            var order = new OrderOp("[TimePosition] DESC");

            IEnumerable<doc_BarcodeReconciliation> barcodes = QueryManager<doc_BarcodeReconciliation>.Select(filter, order).AsEnumerable();
            foreach (doc_BarcodeReconciliation barcode in barcodes)
            {
                if (result.ContainsKey(barcode.Reason) == false)
                    result.Add(barcode.Reason, barcode.STATE.ToString());
            }

            return result;
        }
    }
}