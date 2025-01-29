using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_projekt.Data;
using mvc_projekt.Models;

namespace mvc_projekt.Controllers
{
    public class ZadaniesController : Controller
    {
        private readonly mvc_projektContext _context;

        public ZadaniesController(mvc_projektContext context)
        {
            _context = context;
        }

        // GET: Zadanies
        public async Task<IActionResult> Index(string searchString, int? statusId, int? categoryId, string sortOrder, int? pageNumber)
        {
            var zadania = _context.Zadanie.Include(z => z.Kategoria).Include(z => z.Status).AsQueryable();

            // Filtrowanie po tytule
            if (!string.IsNullOrEmpty(searchString))
            {
                zadania = zadania.Where(z => z.Tytul.Contains(searchString));
            }

            // Filtrowanie po statusie
            if (statusId.HasValue && statusId > 0)
            {
                zadania = zadania.Where(z => z.StatusId == statusId);
            }

            // Filtrowanie po kategorii
            if (categoryId.HasValue && categoryId > 0)
            {
                zadania = zadania.Where(z => z.KategoriaId == categoryId);
            }

            // Sortowanie
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParam"] = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewData["CategorySortParam"] = sortOrder == "category_asc" ? "category_desc" : "category_asc";
            ViewData["StatusSortParam"] = sortOrder == "status_asc" ? "status_desc" : "status_asc";

            switch (sortOrder)
            {
                case "title_desc":
                    zadania = zadania.OrderByDescending(z => z.Tytul);
                    break;
                case "title_asc":
                    zadania = zadania.OrderBy(z => z.Tytul);
                    break;
                case "category_asc":
                    zadania = zadania.OrderBy(z => z.Kategoria.Nazwa);
                    break;
                case "category_desc":
                    zadania = zadania.OrderByDescending(z => z.Kategoria.Nazwa);
                    break;
                case "status_asc":
                    zadania = zadania.OrderBy(z => z.Status.Nazwa);
                    break;
                case "status_desc":
                    zadania = zadania.OrderByDescending(z => z.Status.Nazwa);
                    break;
                default:
                    zadania = zadania.OrderBy(z => z.Tytul);
                    break;
            }

            // Paginacja
            int pageSize = 5;
            int page = pageNumber ?? 1;
            int totalItems = await zadania.CountAsync();
            var items = await zadania.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["PageNumber"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = statusId;
            ViewData["CurrentCategory"] = categoryId;

            ViewBag.Statuses = await _context.Status.ToListAsync();
            ViewBag.Categories = await _context.Kategoria.ToListAsync();

            return View(items);
        }


        // GET: Zadanies/Create
        public IActionResult Create()
        {
            ViewData["KategoriaId"] = new SelectList(_context.Kategoria, "Id", "Nazwa");
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Nazwa");
            ViewData["ZadaniaPodrzedneId"] = new SelectList(_context.Zadanie, "Id", "Tytul");
            ViewData["ZadanieNadrzedneId"] = new SelectList(_context.Zadanie, "Id", "Tytul");
            return View();
        }

        // POST: Zadanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tytul,Opis,KategoriaId,StatusId,ZadanieNadrzedneId,ZadaniaPodrzedneId")] Zadanie zadanie)
        {
            if (zadanie.ZadanieNadrzedneId.HasValue && zadanie.ZadanieNadrzedneId == zadanie.ZadaniaPodrzedneId)
            {
                ModelState.AddModelError("ZadanieNadrzedneId", "Zadanie nadrzędne i podrzędne nie mogą być takie same.");
                ModelState.AddModelError("ZadaniaPodrzedneId", "Zadanie nadrzędne i podrzędne nie mogą być takie same.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(zadanie);
                await _context.SaveChangesAsync(); // Zapisujemy zadanie, aby mieć jego ID

                // Aktualizacja zadania podrzędnego
                if (zadanie.ZadaniaPodrzedneId.HasValue)
                {
                    var zadaniePodrzedne = await _context.Zadanie.FindAsync(zadanie.ZadaniaPodrzedneId.Value);
                    if (zadaniePodrzedne != null)
                    {
                        zadaniePodrzedne.ZadanieNadrzedneId = zadanie.Id;
                        _context.Update(zadaniePodrzedne);
                    }
                }

                // Aktualizacja zadania nadrzędnego
                if (zadanie.ZadanieNadrzedneId.HasValue)
                {
                    var zadanieNadrzedne = await _context.Zadanie.FindAsync(zadanie.ZadanieNadrzedneId.Value);
                    if (zadanieNadrzedne != null)
                    {
                        zadanieNadrzedne.ZadaniaPodrzedneId = zadanie.Id;
                        _context.Update(zadanieNadrzedne);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        
            }

            ViewData["KategoriaId"] = new SelectList(_context.Kategoria, "Id", "Nazwa", zadanie.KategoriaId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Nazwa", zadanie.StatusId);
            ViewData["ZadaniaPodrzedneId"] = new SelectList(_context.Zadanie, "Id", "Tytul", zadanie.ZadaniaPodrzedneId);
            ViewData["ZadanieNadrzedneId"] = new SelectList(_context.Zadanie, "Id", "Tytul", zadanie.ZadanieNadrzedneId);
            return View(zadanie);
        }
        // GET: Zadanies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zadanie = await _context.Zadanie.FindAsync(id);
            if (zadanie == null)
            {
                return NotFound();
            }
            ViewData["KategoriaId"] = new SelectList(_context.Kategoria, "Id", "Nazwa", zadanie.KategoriaId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Nazwa", zadanie.StatusId);
            ViewData["ZadaniaPodrzedneId"] = new SelectList(_context.Zadanie.Where(z => z.Id != zadanie.Id), "Id", "Tytul", zadanie.ZadaniaPodrzedneId);
            ViewData["ZadanieNadrzedneId"] = new SelectList(_context.Zadanie.Where(z => z.Id != zadanie.Id), "Id", "Tytul", zadanie.ZadanieNadrzedneId);
            return View(zadanie);
        }

        // POST: Zadanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tytul,Opis,KategoriaId,StatusId,ZadanieNadrzedneId,ZadaniaPodrzedneId")] Zadanie zadanie)
        {
            if (id != zadanie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Usuwanie powiązania z poprzednim zadaniem podrzędnym, jeśli użytkownik ustawił wartość na null
                    if (!zadanie.ZadaniaPodrzedneId.HasValue)
                    {
                        var poprzednieZadaniePodrzedne = await _context.Zadanie
                            .FirstOrDefaultAsync(z => z.ZadanieNadrzedneId == zadanie.Id);
                        if (poprzednieZadaniePodrzedne != null)
                        {
                            poprzednieZadaniePodrzedne.ZadanieNadrzedneId = null;
                            _context.Update(poprzednieZadaniePodrzedne);
                        }
                    }
                    else
                    {
                        // Aktualizacja zadania podrzędnego
                        var zadaniePodrzedne = await _context.Zadanie.FindAsync(zadanie.ZadaniaPodrzedneId.Value);
                        if (zadaniePodrzedne != null)
                        {
                            zadaniePodrzedne.ZadanieNadrzedneId = zadanie.Id;
                            _context.Update(zadaniePodrzedne);
                        }
                    }

                    // Usuwanie powiązania z poprzednim zadaniem nadrzędnym, jeśli użytkownik ustawił wartość na null
                    if (!zadanie.ZadanieNadrzedneId.HasValue)
                    {
                        var poprzednieZadanieNadrzedne = await _context.Zadanie
                            .FirstOrDefaultAsync(z => z.ZadaniaPodrzedneId == zadanie.Id);
                        if (poprzednieZadanieNadrzedne != null)
                        {
                            poprzednieZadanieNadrzedne.ZadaniaPodrzedneId = null;
                            _context.Update(poprzednieZadanieNadrzedne);
                        }
                    }
                    else
                    {
                        // Aktualizacja zadania nadrzędnego
                        var zadanieNadrzedne = await _context.Zadanie.FindAsync(zadanie.ZadanieNadrzedneId.Value);
                        if (zadanieNadrzedne != null)
                        {
                            zadanieNadrzedne.ZadaniaPodrzedneId = zadanie.Id;
                            _context.Update(zadanieNadrzedne);
                        }
                    }

                    _context.Update(zadanie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZadanieExists(zadanie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["KategoriaId"] = new SelectList(_context.Kategoria, "Id", "Nazwa", zadanie.KategoriaId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Nazwa", zadanie.StatusId);
            ViewData["ZadaniaPodrzedneId"] = new SelectList(_context.Zadanie.Where(z => z.Id != zadanie.Id), "Id", "Tytul", zadanie.ZadaniaPodrzedneId);
            ViewData["ZadanieNadrzedneId"] = new SelectList(_context.Zadanie.Where(z => z.Id != zadanie.Id), "Id", "Tytul", zadanie.ZadanieNadrzedneId);
            return View(zadanie);
        }

        // GET: Zadanies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var zadanie = await _context.Zadanie
                .Include(z => z.Kategoria)
                .Include(z => z.Status)
                .Include(z => z.ZadaniaPodrzedne)
                .Include(z => z.ZadanieNadrzedne)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zadanie == null)
            {
                return NotFound();
            }

            return View(zadanie);
        }

        // POST: Zadanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zadanie = await _context.Zadanie.FindAsync(id);
            if (zadanie == null)
            {
                return NotFound();
            }

            // Set all child references to null before deleting the task
            var childTask = await _context.Zadanie
                .Where(z => z.ZadaniaPodrzedneId == zadanie.Id)
                .ToListAsync();

            var ParentTask = await _context.Zadanie
             .Where(z => z.ZadanieNadrzedneId == zadanie.Id)
             .ToListAsync();

            foreach (var c in childTask)
            {
                c.ZadaniaPodrzedneId = null; // Remove the reference to the parent task
                _context.Zadanie.Update(c);
            }

            foreach (var p in ParentTask)
            {
                p.ZadanieNadrzedneId = null; // Remove the reference to the parent task
                _context.Zadanie.Update(p);
            }




            // Now delete the parent task
            _context.Zadanie.Remove(zadanie);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zadanie = await _context.Zadanie
                .Include(z => z.Kategoria)
                .Include(z => z.Status)
                .Include(z => z.ZadanieNadrzedne)
                .Include(z => z.ZadaniaPodrzedne)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (zadanie == null)
            {
                return NotFound();
            }

            return View(zadanie);
        }


        private bool ZadanieExists(int id)
        {
            return _context.Zadanie.Any(e => e.Id == id);
        }
    }
}
