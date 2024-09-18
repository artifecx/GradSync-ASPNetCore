using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Resume
{
    public string ResumeId { get; set; }

    public string FileName { get; set; }

    public byte[] FileContent { get; set; }

    public string FileType { get; set; }

    public string ExtractedText { get; set; }

    public DateTime UploadedDate { get; set; }

    public virtual ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();
}
