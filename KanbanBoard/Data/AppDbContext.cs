using KanbanBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Data;

/// <summary>
/// Databaskontexten som hanterar kopplingen mellan
/// våra C#-modeller och SQLite-databasen.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Representerar tabellen med användarens Kanban-tavlor.
    public DbSet<Board> Boards { get; set; } = null!;

    // Representerar tabellen med uppgifter.
    // Varje uppgift kopplas till en tavla genom BoardId.
    public DbSet<KanbanTask> KanbanTasks { get; set; } = null!;
}