using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LeitourApi.Models;

[Table("tbFollowingList")]
[PrimaryKey(nameof(UserId), nameof(FollowingEmail))]
public partial class FollowUser
{
    [Key]
    public int UserId { get; set; }

    [Key]
    public string FollowingEmail { get; set; }

 
    public FollowUser(){}
    public FollowUser(int userId,string email){
        this.UserId = userId;
        this.FollowingEmail = email;
    }
}
