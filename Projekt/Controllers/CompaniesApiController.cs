using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Models.Movies;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[Route("api/companies")]
[ApiController]
public class CompaniesApiController : ControllerBase
{
    private readonly MoviesContext _context;
    
    public CompaniesApiController(MoviesContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetFiltered(string? filter)
    {
        var query = _context.ProductionCompanies.AsQueryable();
        
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(o => o.CompanyName != null && o.CompanyName.ToLower().Contains(filter.ToLower()));
        }
        
        return Ok(query
            .OrderBy(o => o.CompanyName)
            .AsNoTracking()
            .ToList()
        );
    }
}


