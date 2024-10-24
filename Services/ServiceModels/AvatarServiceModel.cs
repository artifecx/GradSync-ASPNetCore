using System;
using System.IO;

namespace Services.ServiceModels
{
    /// <summary>
    /// Model for handling avatar data in services.
    /// </summary>
    public class AvatarServiceModel
    {
        /// <summary>
        /// Gets or sets the avatar's identifier.
        /// </summary>
        public string AvatarId { get; set; }

        /// <summary>
        /// Gets or sets the file name of the avatar.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file content of the avatar as a stream.
        /// </summary>
        public Stream FileContent { get; set; }

        /// <summary>
        /// Gets or sets the file type (e.g., image/jpeg, image/png).
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the uploaded date of the avatar.
        /// </summary>
        public DateTime UploadedDate { get; set; }
    }
}
