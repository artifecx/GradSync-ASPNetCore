using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class MessageThread
{
    public string MessageThreadId { get; set; }

    public string Title { get; set; }

    public virtual ICollection<MessageParticipant> MessageParticipants { get; set; } = new List<MessageParticipant>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
