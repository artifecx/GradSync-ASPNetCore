using System.Collections.Generic;

namespace Services.ServiceModels
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; }
        public Dictionary<string, string> Preferences { get; set; }

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
