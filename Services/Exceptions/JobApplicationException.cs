using System;

namespace Services.Exceptions
{
    public class JobApplicationExceptions
    {
        public class JobApplicationException : Exception
        {
            public string Id { get; }
            public JobApplicationException(string message) : base(message) { }
            public JobApplicationException(string message, string id) : base(message)
            {
                Id = id;
            }
        }
    }
}
