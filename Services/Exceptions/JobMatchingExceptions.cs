using System;

namespace Services.Exceptions
{
    public class JobMatchingExceptions
    {
        public class JobMatchingException : Exception
        {
            public string Id { get; }
            public JobMatchingException(string message) : base(message) { }
            public JobMatchingException(string message, string id) : base(message)
            {
                Id = id;
            }
        }
    }
}
