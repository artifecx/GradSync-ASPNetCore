using System;

namespace Services.Exceptions
{
    public class JobExceptions
    {
        public class JobException : Exception
        {
            public string Id { get; }
            public JobException(string message) : base(message) { }
            public JobException(string message, string id) : base(message)
            {
                Id = id;
            }
        }
    }
}
