using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeitourApi.Models;

public partial class BookUser
{
    [Key]
    public int SavedBookId { get; set; }

  
    public int UserId { get; set; }

    public string BookId { get; set; } = null!;

    public bool Public { get; set; }


}
