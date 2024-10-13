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

        public class UserNotVerifiedException : Exception
        {
            public string Id { get; }
            public UserNotVerifiedException(string message) : base(message) { }
            public UserNotVerifiedException(string message, string id) : base(message)
            {
                Id = id;
            }
        }
    }
}
