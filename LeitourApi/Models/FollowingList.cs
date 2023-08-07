using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeitourApi.Models;

public partial class FollowingList
{
    [Key]
    public int FollowingListId { get; set; }

    [InverseProperty("User")]
    public ICollection<User> Users { get; set; }
}
