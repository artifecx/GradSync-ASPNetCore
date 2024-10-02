using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions
{
    public class CompanyExceptions
    {
        public class CompanyException : Exception
        {
            public string Id { get; }
            public CompanyException(string message) : base(message) { }
            public CompanyException(string message, string id) : base(message)
            {
                Id = id;
            }
        }
    }
}
