using System;

namespace ProjectWebApi.Models
{
    public class DocumentResponse
    {
        public Guid UID { get; set; }
        public string TimePosition { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string BarcodeStatus { get; set; }
    }
}