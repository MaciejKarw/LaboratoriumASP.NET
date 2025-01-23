using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Models;
using Projekt.Models.Movies;

namespace Projekt.Controllers;

public class MoviesController : Controller
{
    private readonly MoviesContext _context;

    public MoviesController(MoviesContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Keywords(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movies
            .FirstOrDefaultAsync(m => m.MovieId == id);

        if (movie == null)
        {
            return NotFound();
        }
        
        var companyId = await _context.MovieCompanies
            .Where(mc => mc.MovieId == id)
            .Select(mc => mc.CompanyId)
            .FirstOrDefaultAsync();

        var existingKeywords = await _context.MovieKeywords
            .Where(mk => mk.MovieId == id)
            .Include(mk => mk.Keyword)
            .Select(mk => mk.Keyword!)
            .ToListAsync();

        var viewModel = new MovieKeywordViewModel
        {
            MovieId = movie.MovieId,
            CompanyId = companyId ?? 0,  // Dodane pole
            MovieTitle = movie.Title ?? "",
            ExistingKeywords = existingKeywords
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Keywords(MovieKeywordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var keyword = await _context.Keywords
                .FirstOrDefaultAsync(k => k.KeywordName == model.NewKeywordName);

            if (keyword == null)
            {
                var maxKeywordId = await _context.Keywords.MaxAsync(k => k.KeywordId);
                
                keyword = new Keyword
                {
                    KeywordId = maxKeywordId + 1,
                    KeywordName = model.NewKeywordName
                };
                _context.Keywords.Add(keyword);
                await _context.SaveChangesAsync();
            }
            
            var existingLink = await _context.MovieKeywords
                .AnyAsync(mk => mk.MovieId == model.MovieId && mk.KeywordId == keyword.KeywordId);

            if (!existingLink)
            {
                _context.MovieKeywords.Add(new MovieKeyword
                {
                    MovieId = model.MovieId,
                    KeywordId = keyword.KeywordId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Keywords), new { id = model.MovieId });
        }
        
        var existingKeywords = await _context.MovieKeywords
            .Where(mk => mk.MovieId == model.MovieId)
            .Include(mk => mk.Keyword)
            .Select(mk => mk.Keyword!)
            .ToListAsync();

        model.ExistingKeywords = existingKeywords;
        return View(model);
    }
}