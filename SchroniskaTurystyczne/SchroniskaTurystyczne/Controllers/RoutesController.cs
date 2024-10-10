using Microsoft.AspNetCore.Mvc;
using SchroniskaTurystyczne.Data;

namespace SchroniskaTurystyczne.Controllers
{
    public class RoutesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoutesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Shelters/Create
        public IActionResult Create()
        {
            return View();
        }
    }
}
