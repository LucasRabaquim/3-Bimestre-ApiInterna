using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LeitourApi.Models;

[Table("tbAnnotation")]
public class Annotation
{
    [Key]
    public int AnnotationId { get; set; }
    public int SavedBookId { get; set; }
    public string AnnotationText { get; set; } = null!;
    public DateTime AlteratedDate { get; set; }

}
