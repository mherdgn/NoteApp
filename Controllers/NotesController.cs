
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NoteApp.Data;
using NoteApp.Models;

namespace NoteApp.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Giriş yapan kullanıcının ID'sini bul
        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        // NOTLAR
        public IActionResult Index(string searchString, int page = 1)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            page = Math.Max(page, 1);
            int pageSize = 9;

            var notesQuery = _context.Notes
                .Where(n => n.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                notesQuery = notesQuery.Where(n =>
                    n.Title.Contains(searchString) ||
                    n.Content.Contains(searchString));
            }

            notesQuery = notesQuery.OrderByDescending(n => n.CreatedAt);

            int totalNotes = notesQuery.Count();

            var notes = notesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToList();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] =
                (int)Math.Ceiling((double)totalNotes / pageSize);
            ViewData["SearchString"] = searchString;

            return View(notes);
        }

        // DETAY
        public async Task<IActionResult> Details(int? id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var note = await _context.Notes
                .AsNoTracking()
                .FirstOrDefaultAsync(n =>
                    n.Id == id && n.UserId == userId.Value);

            if (note == null)
                return NotFound();

            return View(note);
        }

        // YENİ NOT - GET
        public IActionResult Create()
        {
            if (GetCurrentUserId() == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // YENİ NOT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Content")] Note note)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                note.UserId = userId.Value;
                note.CreatedAt = DateTime.UtcNow;
                note.IsFavorite = false;

                _context.Notes.Add(note);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Not başarıyla eklendi.";

                return RedirectToAction(nameof(Index));
            }

            return View(note);
        }

        // DÜZENLE - GET
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n =>
                    n.Id == id && n.UserId == userId.Value);

            if (note == null)
                return NotFound();

            return View(note);
        }

        // DÜZENLE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Content")] Note note)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (id != note.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Notes
                    .FirstOrDefaultAsync(n =>
                        n.Id == id && n.UserId == userId.Value);

                if (existing == null)
                    return NotFound();

                existing.Title = note.Title;
                existing.Content = note.Content;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Not güncellendi.";

                return RedirectToAction(nameof(Index));
            }

            return View(note);
        }

        // SİL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var note = await _context.Notes
                .FirstOrDefaultAsync(n =>
                    n.Id == id && n.UserId == userId.Value);

            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Not silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        // FAVORİ DEĞİŞTİR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n =>
                    n.Id == id && n.UserId == userId.Value);

            if (note == null)
                return NotFound();

            note.IsFavorite = !note.IsFavorite;

            await _context.SaveChangesAsync();

            return Json(note.IsFavorite);
        }

        // FAVORİLER
        public async Task<IActionResult> Favorites()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var favorites = await _context.Notes
                .Where(n =>
                    n.UserId == userId.Value && n.IsFavorite)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return View(favorites);
        }
    }
}
