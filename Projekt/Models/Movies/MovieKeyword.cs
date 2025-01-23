using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models.Movies;

public partial class MovieKeyword
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? MovieId { get; set; }

    public int? KeywordId { get; set; }

    public virtual Keyword? Keyword { get; set; }

    public virtual Movie? Movie { get; set; }
}
