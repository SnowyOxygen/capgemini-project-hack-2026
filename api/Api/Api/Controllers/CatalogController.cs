using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CatalogController : ControllerBase
    {
        private readonly DataContext _context;

        public CatalogController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("materiaux")]
        public async Task<ActionResult<IEnumerable<Materiau>>> GetMateriaux()
        {
            return await _context.Materiaux.OrderBy(m => m.Nom).ToListAsync();
        }

        [HttpGet("energies")]
        public async Task<ActionResult<IEnumerable<FacteurEnergie>>> GetFacteursEnergie()
        {
            return await _context.FacteursEnergie.OrderBy(f => f.TypeEnergie).ToListAsync();
        }
    }
}
