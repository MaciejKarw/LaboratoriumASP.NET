using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Projekt.Models.Movies;
using System.Threading.Tasks;
using Projekt.Models.ViewModels;
using Projekt.Models;
using Microsoft.AspNetCore.Authorization;

namespace Projekt.Controllers
{
    [Authorize]
    public class ProductionCompaniesController : Controller
    {
        private readonly MoviesContext _context;
        private readonly int PageSize = 20;

        public ProductionCompaniesController(MoviesContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            
            var query = _context.ProductionCompanies
                .Select(pc => new ProductionCompanyViewModel
                {
                    CompanyId = pc.CompanyId,
                    CompanyName = pc.CompanyName,
                    MoviesCount = _context.MovieCompanies
                        .Count(mc => mc.CompanyId == pc.CompanyId),
                    TotalBudget = _context.MovieCompanies
                        .Where(mc => mc.CompanyId == pc.CompanyId)
                        .Join(_context.Movies,
                            mc => mc.MovieId,
                            m => m.MovieId,
                            (mc, m) => m.Budget ?? 0)
                        .Sum()
                });

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(pc => pc.CompanyName != null && 
                                        pc.CompanyName.ToLower().Contains(searchString.ToLower()));
                ViewData["CurrentFilter"] = searchString;
            }

            query = query.OrderBy(pc => pc.CompanyName);

            return View(await PaginatedList<ProductionCompanyViewModel>.CreateAsync(query, pageNumber, PageSize));
        }

        public async Task<IActionResult> Movies(int? id, int page = 1)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moviesQuery = from m in _context.Movies
                             join mc in _context.MovieCompanies on m.MovieId equals mc.MovieId
                             where mc.CompanyId == id
                             select new MovieDetailsViewModel
                             {
                                 MovieId = m.MovieId,
                                 Title = m.Title,
                                 Popularity = m.Popularity,
                                 Revenue = m.Revenue,
                                 Runtime = m.Runtime,
                                 Stars = m.VoteAverage.HasValue ? (int)Math.Round(m.VoteAverage.Value) : 0,
                                 VoteCount = m.VoteCount
                             };

            const int pageSize = 10;
            var paginatedList = await PaginatedList<MovieDetailsViewModel>.CreateAsync(
                moviesQuery.OrderByDescending(m => m.Popularity), page, pageSize);

            return View(paginatedList);
        }
    }
} 