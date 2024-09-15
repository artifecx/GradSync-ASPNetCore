using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Admin
    {
        public string UserId { get; set; }
        public bool? IsSuper { get; set; }

        public virtual User User { get; set; }
    }
}
