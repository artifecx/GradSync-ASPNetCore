using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Resume
    {
        public Resume()
        {
            Applicants = new HashSet<Applicant>();
        }

        public string ResumeId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileType { get; set; }
        public string ExtractedText { get; set; }
        public DateTime UploadedDate { get; set; }

        public virtual ICollection<Applicant> Applicants { get; set; }
    }
}
