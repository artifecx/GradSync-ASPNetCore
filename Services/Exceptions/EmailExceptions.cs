using System;

namespace Services.Exceptions
{
    public class EmailExceptions
    {
        public class EmailException : Exception
        {
            public string Id { get; }
            public EmailException(string message) : base(message) { }
            public EmailException(string message, string id) : base(message)
            {
                Id = id;
            }
        }
    }
}
