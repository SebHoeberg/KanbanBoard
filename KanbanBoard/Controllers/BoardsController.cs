
using KanbanBoard.Data;
using KanbanBoard.Models;
using KanbanBoard.Strategies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Controllers;

/// <summary>
/// Hanterar användarens Kanban-tavlor.
/// Här finns funktioner för att visa, skapa, redigera och ta bort tavlor.
/// </summary>
public class BoardsController : Controller
{
    private readonly AppDbContext _context;

    // DbContext skickas in automatiskt genom dependency injection.
    public BoardsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Boards
    // Hämtar alla tavlor från databasen.
    public async Task<IActionResult> Index()
    {
        var boards = await _context.Boards
            .AsNoTracking()
            .OrderByDescending(board => board.CreatedAt)
            .ToListAsync();

        return View(boards);
    }

    // GET: /Boards/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Boards/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name,Description")] Board board)
    {
        if (!ModelState.IsValid)
        {
            return View(board);
        }

        // Skapelsedatum sätts på servern.
        board.CreatedAt = DateTime.UtcNow;

        _context.Boards.Add(board);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Boards/Details/1
    // Visar tavlan och sorterar dess uppgifter med Strategy pattern.
    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        string sort = "priority")
    {
        var board = await _context.Boards
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (board == null)
        {
            return NotFound();
        }

        var tasks = await _context.KanbanTasks
            .AsNoTracking()
            .Where(t => t.BoardId == id)
            .ToListAsync();

        // Väljer sorteringsstrategi utifrån användarens val.
        ITaskSortStrategy strategy = sort.ToLowerInvariant() switch
        {
            "deadline" => new DeadlineSortStrategy(),
            "title" => new TitleSortStrategy(),
            _ => new PrioritySortStrategy()
        };

        var sortService = new TaskSortService(strategy);
        var sortedTasks = sortService.SortTasks(tasks).ToList();

        ViewBag.SelectedSort = sort.ToLowerInvariant() switch
        {
            "deadline" => "deadline",
            "title" => "title",
            _ => "priority"
        };

        ViewBag.Tasks = sortedTasks;

        return View(board);
    }

    // GET: /Boards/Edit/1
    // Hämtar tavlan och visar redigeringsformuläret.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var board = await _context.Boards
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (board == null)
        {
            return NotFound();
        }

        return View(board);
    }

    // POST: /Boards/Edit/1
    // Uppdaterar endast namn och beskrivning.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Name,Description")] Board editedBoard)
    {
        if (id != editedBoard.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(editedBoard);
        }

        var board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == id);

        if (board == null)
        {
            return NotFound();
        }

        // Skapelsedatum och uppgifter påverkas inte.
        board.Name = editedBoard.Name;
        board.Description = editedBoard.Description;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = board.Id });
    }

    // POST: /Boards/Delete
    // Tar bort tavlan och alla uppgifter som tillhör den.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == id);

        if (board == null)
        {
            return NotFound();
        }

        // Hämtar endast uppgifterna som tillhör tavlan.
        var tasks = await _context.KanbanTasks
            .Where(task => task.BoardId == id)
            .ToListAsync();

        // Tar först bort uppgifterna så att inga kopplade poster blir kvar.
        _context.KanbanTasks.RemoveRange(tasks);

        // Därefter tas själva tavlan bort.
        _context.Boards.Remove(board);

        // EF Core sparar båda borttagningarna i samma SaveChanges-anrop.
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
