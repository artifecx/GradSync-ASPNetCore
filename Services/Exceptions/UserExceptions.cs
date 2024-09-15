using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions
{
    public class UserExceptions
    {
        public class UserException : Exception
        {
            public string Id { get; }
            public UserException(string message) : base(message) { }
            public UserException(string message, string id) : base(message)
            {
                Id = id;
            }
        }
    }
}
