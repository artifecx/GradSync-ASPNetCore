using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Avatar
{
    public string AvatarId { get; set; }

    public string FileName { get; set; }

    public byte[] FileContent { get; set; }

    public string FileType { get; set; }

    public DateTime UploadedDate { get; set; }

    public virtual User User { get; set; }
}
