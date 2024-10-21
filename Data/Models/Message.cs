using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Message
{
    public string MessageId { get; set; }

    public string MessageThreadId { get; set; }

    public string UserId { get; set; }

    public string Content { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual MessageThread MessageThread { get; set; }

    public virtual User User { get; set; }
}
