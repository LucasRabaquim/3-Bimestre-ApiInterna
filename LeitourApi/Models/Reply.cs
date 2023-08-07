using System;
using System.Collections.Generic;

namespace LeitourApi.Models;

public partial class Reply
{
    public int ReplyId { get; set; }

    public int PostId { get; set; }

    public int UserId { get; set; }

    public string MessageReply { get; set; } = null!;

    public int Likes { get; set; }

    public DateTime ReplyDate { get; set; }

    public DateTime? AlteratedDate { get; set; }
}
