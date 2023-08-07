using System;
using System.Collections.Generic;

namespace LeitourApi.Models;

public partial class Page
{
    public int PageId { get; set; }

    public string? NamePage { get; set; }

    public string DescriptionPage { get; set; } = null!;

    public byte[]? Cover { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? AlteratedDate { get; set; }

    public int UserAlteration { get; set; }
}
