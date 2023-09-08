
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeitourApi.Models
{
    [Table("tbSavedBook")]
    public partial class SavedBook
    {
        [Key]
        public int SavedId { get; set; }
        public int UserId { get; set; }
        public bool Public { get; set; }
        public string BookKey { get; set; } = null!;
    }
}
