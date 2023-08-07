using System;
using System.Collections.Generic;

namespace LeitourApi.Models;

public partial class BookPage
{
    public string BookId { get; set; } = null!;

    public int PageId { get; set; }
}
