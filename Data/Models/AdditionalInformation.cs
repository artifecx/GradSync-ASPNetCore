using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class AdditionalInformation
    {
        public string AdditionalInformationId { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileType { get; set; }
        public string TextContent { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
