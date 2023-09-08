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
    public required int UserId { get; set; }

    [Column("NameUser")]
    //[MaxLength(20)]
    public required string NameUser { get; set; } = null!;

    [MaxLength(60)]
    public required string Email { get; set; } = null!;

    [Column("PasswordUser")]
    public required string Password { get; set; } = null!;

    // public byte[]? ProfilePhoto { get; set; }

    public decimal Theme { get; set; } = 0;

    public int RoleUser { get; set; } = 0;

    public required bool? ActiveUser { get; set; } = true;
}