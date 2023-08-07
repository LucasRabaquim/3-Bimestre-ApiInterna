using System;
using System.Collections.Generic;

namespace LeitourApi.Models;

public partial class FollowingPage
{
    public int UserId { get; set; }

    public int PageId { get; set; }

    public string? RoleUser { get; set; }
}
