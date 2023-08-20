using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeitourApi.Models;

[Table("tbPost")]
public partial class Post
{
    [Key]
    public int PostId { get; set; }

    [Column("userId")]
    public int UserId { get; set; }

    public int? PageId { get; set; }

    public string MessagePost { get; set; } = null!;

    public int Likes { get; set; }

    // public byte[]? Media { get; set; }

    public DateTime PostDate { get; set; }

    public DateTime? AlteratedDate { get; set; }
}
