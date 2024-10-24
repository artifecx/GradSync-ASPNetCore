using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class MessageParticipant
{
    public string MessageParticipantId { get; set; }

    public string MessageThreadId { get; set; }

    public string UserId { get; set; }

    public virtual MessageThread MessageThread { get; set; }

    public virtual User User { get; set; }
}
