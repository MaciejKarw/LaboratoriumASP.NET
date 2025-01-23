using System.ComponentModel.DataAnnotations;
using Projekt.Models.Movies;

public class MovieKeywordViewModel
{
    public int MovieId { get; set; }
    public int CompanyId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public List<Keyword> ExistingKeywords { get; set; } = new();
    
    [Required(ErrorMessage = "Nazwa słowa kluczowego jest wymagana")]
    [Display(Name = "Nowe słowo kluczowe")]
    public string? NewKeywordName { get; set; }
} 