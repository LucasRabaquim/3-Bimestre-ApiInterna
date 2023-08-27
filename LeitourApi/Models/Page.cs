using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeitourApi.Models;

[Table("tbPage")]
public partial class Page
{
    public int PageId { get; set; }

    public string? NamePage { get; set; }

    public string DescriptionPage { get; set; } = null!;

    public byte[]? Cover { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? AlteratedDate { get; set; }

    public bool ActivePage {get; set;}
}
