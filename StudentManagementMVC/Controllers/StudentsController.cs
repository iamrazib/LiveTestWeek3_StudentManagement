using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVC.Data;
using StudentManagementMVC.Models;

namespace StudentManagementMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;
        

        public StudentsController(AppDbContext context) {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            IQueryable<Student> query = _context.Students.Where(s => s.Age > 18);

            if (!string.IsNullOrWhiteSpace(search))
            {                
                var termLower = search.Trim().ToLower();

                query = query.Where(s =>
                    s.FirstName.ToLower().Contains(termLower) ||
                    s.LastName.ToLower().Contains(termLower) ||
                    (s.FirstName + " " + s.LastName).ToLower().Contains(termLower));
            }

            
            ViewBag.FilteredCount = await query.CountAsync();       
            ViewBag.TotalCount = await _context.Students.CountAsync(); 
            ViewBag.Search = search;

            var students = await query
                .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
                .ToListAsync();
            
            return View(students);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Age")] Student student)
        {
            if (!ModelState.IsValid) return View(student);

            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null) return NotFound();

            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Age")] Student student)
        {
            if (id != student.Id) return NotFound();
            if (!ModelState.IsValid) return View(student);

            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
        private bool StudentExists(int id) =>
            _context.Students.Any(e => e.Id == id);

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
