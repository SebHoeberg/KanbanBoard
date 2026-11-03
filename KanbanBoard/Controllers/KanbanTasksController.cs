
using KanbanBoard.Data;
using KanbanBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Controllers;

/// <summary>
/// Hanterar uppgifter som tillhör användarens Kanban-tavlor.
/// </summary>
public class KanbanTasksController : Controller
{
    private readonly AppDbContext _context;

    // Databaskontexten skickas in genom dependency injection.
    public KanbanTasksController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /KanbanTasks/Create?boardId=1
    // Visar formuläret för att skapa en ny uppgift.
    [HttpGet]
    public async Task<IActionResult> Create(int boardId)
    {
        // Kontrollera att tavlan finns innan formuläret visas.
        var boardExists = await _context.Boards
            .AnyAsync(board => board.Id == boardId);

        if (!boardExists)
        {
            return NotFound();
        }

        // Nya uppgifter placeras automatiskt i Backlogg.
        var task = new KanbanTask
        {
            BoardId = boardId,
            Status = KanbanBoard.Models.TaskStatus.Backlog,
            Priority = TaskPriority.Normal
        };

        return View(task);
    }

    // POST: /KanbanTasks/Create
    // Tar emot formuläret och sparar uppgiften i SQLite.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("BoardId,Title,Description,Priority,DueDate")] KanbanTask task)
    {
        // Kontrollerar att uppgiften kopplas till en befintlig tavla.
        var boardExists = await _context.Boards
            .AnyAsync(board => board.Id == task.BoardId);

        if (!boardExists)
        {
            return NotFound();
        }

        // Board är en navigationsegenskap som EF Core fyller i.
        // Vi tar bort ett eventuellt valideringsfel eftersom
        // formuläret bara skickar BoardId, inte ett Board-objekt.
        ModelState.Remove(nameof(KanbanTask.Board));

        // Kontrollera att prioriteten har ett tillåtet värde.
        if (!Enum.IsDefined(task.Priority))
        {
            ModelState.AddModelError(
                nameof(KanbanTask.Priority),
                "Välj en giltig prioritet.");
        }

        // Om något är fel visas formuläret igen med felmeddelanden.
        if (!ModelState.IsValid)
        {
            return View(task);
        }

        // Status och skapandedatum bestäms på servern.
        task.Status = KanbanBoard.Models.TaskStatus.Backlog;
        task.CreatedAt = DateTime.UtcNow;

        // Spara uppgiften i databasen.
        _context.KanbanTasks.Add(task);
        await _context.SaveChangesAsync();

        // Återvänd till tavlan där uppgiften skapades.
        return RedirectToAction(
            "Details",
            "Boards",
            new { id = task.BoardId });
    }
    
    // POST: /KanbanTasks/Move
    // Flyttar en uppgift till en annan Kanban-kolumn.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Move(int id, KanbanBoard.Models.TaskStatus status)
    {
        // Kontrollera att statusvärdet motsvarar en giltig kolumn.
        if (!Enum.IsDefined(status))
        {
            return BadRequest();
        }

        // Hämtar uppgiften från databasen så att EF Core kan spåra ändringen.
        var task = await _context.KanbanTasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        // Uppdaterar status och sparar ändringen i SQLite.
        task.Status = status;
        await _context.SaveChangesAsync();

        // Återvänd till tavlan som uppgiften tillhör.
        return RedirectToAction(
            "Details",
            "Boards",
            new { id = task.BoardId });
    }

}
