using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LeitourApi.Models;

[Table("tbFollowingPage")]
[PrimaryKey("UserId","PageId")]
public class FollowingPage
{
    [Key]
    public int UserId { get; set; }
    [Key]
    public int PageId { get; set; }

    public int RoleUser { get; set; }
    
    public FollowingPage() { }
    public FollowingPage(int userId, int pageId, int roleUser)
    {
        this.UserId = userId;
        this.PageId = pageId;
        this.RoleUser = roleUser;
    }
}
