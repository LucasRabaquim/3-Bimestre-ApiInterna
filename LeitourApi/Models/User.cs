using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LeitourApi.Models;

[Table("tbUser")]
public partial class User
{
    [Key]
    [Column("UserId")]
    public int UserId { get; set; }

    [Column("NameUser")]
    //[MaxLength(20)]
    public string NameUser { get; set; } = null!;

    [MaxLength(60)]
    public string Email { get; set; } = null!;

    [Column("PasswordUser")]
    public string Password { get; set; } = null!;

    // public byte[]? ProfilePhoto { get; set; }

    public decimal Theme { get; set; }

    public int RoleUser { get; set; }

    public bool? ActiveUser { get; set; }

}