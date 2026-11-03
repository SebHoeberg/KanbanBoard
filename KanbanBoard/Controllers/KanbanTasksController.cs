
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
        // Formuläret skickar bara BoardId, inte ett Board-objekt.
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

    // GET: /KanbanTasks/Edit/1
    // Hämtar en befintlig uppgift och visar redigeringsformuläret.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // AsNoTracking används eftersom uppgiften bara ska visas här.
        var task = await _context.KanbanTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    // POST: /KanbanTasks/Edit/1
    // Sparar ändringar i en befintlig uppgift.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Title,Description,Priority,DueDate")] KanbanTask updatedTask)
    {
        // Kontrollera att ID i adressen och formuläret stämmer överens.
        if (id != updatedTask.Id)
        {
            return BadRequest();
        }

        // Hämta originaluppgiften så att EF Core kan spåra ändringarna.
        var task = await _context.KanbanTasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        // Board är en navigationsegenskap som inte skickas från formuläret.
        ModelState.Remove(nameof(KanbanTask.Board));

        // BoardId, Status och CreatedAt ändras inte vid redigering.
        ModelState.Remove(nameof(KanbanTask.BoardId));
        ModelState.Remove(nameof(KanbanTask.Status));
        ModelState.Remove(nameof(KanbanTask.CreatedAt));

        if (!Enum.IsDefined(updatedTask.Priority))
        {
            ModelState.AddModelError(
                nameof(KanbanTask.Priority),
                "Välj en giltig prioritet.");
        }

        if (!ModelState.IsValid)
        {
            // Behåll tavlans ID så att avbryt-knappen kan gå tillbaka.
            updatedTask.BoardId = task.BoardId;
            return View(updatedTask);
        }

        // Uppdatera bara de fält som användaren får redigera.
        // På så sätt kan inte formuläret ändra tavla, status eller skapandedatum.
        task.Title = updatedTask.Title;
        task.Description = updatedTask.Description;
        task.Priority = updatedTask.Priority;
        task.DueDate = updatedTask.DueDate;

        await _context.SaveChangesAsync();

        return RedirectToAction(
            "Details",
            "Boards",
            new { id = task.BoardId });
    }

    // POST: /KanbanTasks/Move
    // Flyttar en uppgift till en annan Kanban-kolumn.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Move(
        int id,
        KanbanBoard.Models.TaskStatus status)
    {
        // Kontrollera att statusvärdet motsvarar en giltig kolumn.
        if (!Enum.IsDefined(status))
        {
            return BadRequest();
        }

        // Hämta uppgiften så att EF Core kan spåra ändringen.
        var task = await _context.KanbanTasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        // Uppdatera status och spara ändringen i SQLite.
        task.Status = status;
        await _context.SaveChangesAsync();

        return RedirectToAction(
            "Details",
            "Boards",
            new { id = task.BoardId });
    }

    // POST: /KanbanTasks/Delete
    // Tar bort en befintlig uppgift från databasen.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        // Hämta uppgiften som ska tas bort.
        var task = await _context.KanbanTasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        // Spara tavlans ID innan uppgiften raderas.
        // Det behövs för att återvända till rätt tavla.
        var boardId = task.BoardId;

        // EF Core markerar uppgiften för borttagning.
        _context.KanbanTasks.Remove(task);

        // Ändringen genomförs i SQLite.
        await _context.SaveChangesAsync();

        return RedirectToAction(
            "Details",
            "Boards",
            new { id = boardId });
    }
}
