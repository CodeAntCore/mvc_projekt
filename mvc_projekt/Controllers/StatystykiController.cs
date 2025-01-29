using Microsoft.AspNetCore.Mvc;
using mvc_projekt.Data;
using System.Linq;

namespace mvc_projekt.Controllers
{
    public class StatystykiController : Controller
    {
        private readonly mvc_projektContext _context;

        public StatystykiController(mvc_projektContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var liczbaZadan = _context.Zadanie.Count();
            var liczbaKategorii = _context.Kategoria.Count();
            var liczbaStatusow = _context.Status.Count();

            var zadaniaWedlugKategorii = _context.Zadanie
                .GroupBy(z => z.Kategoria.Nazwa)
                .Select(g => new { Kategoria = g.Key, Liczba = g.Count() })
                .ToList();

            var zadaniaWedlugStatusu = _context.Zadanie
                .GroupBy(z => z.Status.Nazwa)
                .Select(g => new { Status = g.Key, Liczba = g.Count() })
                .ToList();

            ViewBag.LiczbaZadan = liczbaZadan;
            ViewBag.LiczbaKategorii = liczbaKategorii;
            ViewBag.LiczbaStatusow = liczbaStatusow;
            ViewBag.ZadaniaWedlugKategorii = zadaniaWedlugKategorii;
            ViewBag.ZadaniaWedlugStatusu = zadaniaWedlugStatusu;

            return View();
        }
    }
}
