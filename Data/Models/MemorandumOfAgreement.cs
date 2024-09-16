using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class MemorandumOfAgreement
{
    public string MemorandumOfAgreementId { get; set; }

    public string FileName { get; set; }

    public byte[] FileContent { get; set; }

    public string FileType { get; set; }

    public DateTime UploadedDate { get; set; }

    public DateTime ValidityStart { get; set; }

    public DateTime? ValidityEnd { get; set; }

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
}
