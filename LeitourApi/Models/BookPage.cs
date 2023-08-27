using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace LeitourApi.Models;

[Table("tbBookPage")]
[PrimaryKey("BookKey","PageId")]
public partial class BookPage
{
    [Key]
    public string BookKey { get; set; }

    [Key]
    public int PageId { get; set; }

    public BookPage() { }
    public BookPage(string bookKey, int pageId)
    {
        this.BookKey = bookKey;
        this.PageId = pageId;
    }
}
