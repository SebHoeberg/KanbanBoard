
using KanbanBoard.Data;
using KanbanBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Controllers;

/// <summary>
/// Hanterar användarens Kanban-tavlor.
/// Här finns funktioner för att visa och skapa tavlor.
/// </summary>
public class BoardsController : Controller
{
    private readonly AppDbContext _context;

    // DbContext skickas in automatiskt genom dependency injection.
    // Det gör att controllern inte behöver skapa en egen databasanslutning.
    public BoardsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Boards
    // Hämtar alla tavlor från databasen och skickar dem till vyn.
    public async Task<IActionResult> Index()
    {
        // AsNoTracking används eftersom vi bara ska läsa informationen.
        // Det minskar onödig minnesanvändning från EF Cores ändringsspårning.
        var boards = await _context.Boards
            .AsNoTracking()
            .OrderByDescending(board => board.CreatedAt)
            .ToListAsync();

        return View(boards);
    }

    // GET: /Boards/Create
    // Visar formuläret för att skapa en ny Kanban-tavla.
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Boards/Create
    // Tar emot formuläret och sparar tavlan i SQLite.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name,Description")] Board board)
    {
        // Kontrollerar valideringsreglerna i Board-modellen.
        if (!ModelState.IsValid)
        {
            return View(board);
        }

        // Datum sätts på servern i stället för att hämtas från formuläret.
        board.CreatedAt = DateTime.UtcNow;

        // EF Core registrerar tavlan och sparar den i databasen.
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();

        // Efter sparning skickas användaren tillbaka till tavellistan.
        return RedirectToAction(nameof(Index));
    }
    
    
    // GET: /Boards/Details/1
    // Hämtar en specifik tavla tillsammans med dess uppgifter.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        // Hämtar tavlan utan ändringsspårning eftersom vi bara läser data.
        var board = await _context.Boards
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        // Om tavlan inte finns returneras HTTP 404.
        if (board == null)
        {
            return NotFound();
        }

        // Hämtar endast uppgifterna som tillhör den valda tavlan.
        var tasks = await _context.KanbanTasks
            .AsNoTracking()
            .Where(t => t.BoardId == id)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

        // Skickar uppgifterna till vyn och tavlans information via ViewBag.
        ViewBag.Tasks = tasks;

        return View(board);
    }

}
