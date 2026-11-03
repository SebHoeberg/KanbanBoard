using System.ComponentModel.DataAnnotations;

namespace KanbanBoard.Models;

/// <summary>
/// Representerar en uppgift som tillhör en Kanban-tavla.
/// </summary>
public class KanbanTask
{
    public int Id { get; set; }

    // Varje uppgift måste tillhöra en befintlig tavla.
    public int BoardId { get; set; }

    public Board Board { get; set; } = null!;

    [Required(ErrorMessage = "Du måste ange en titel.")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    // Uppgiften placeras i Backlogg när den skapas.
    public TaskStatus Status { get; set; } = TaskStatus.Backlog;

    // Normal prioritet används om användaren inte väljer något annat.
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// De fyra fasta kolumnerna på Kanban-tavlan.
/// </summary>
public enum TaskStatus
{
    Backlog = 0,
    Todo = 1,
    InProgress = 2,
    Done = 3
}

/// <summary>
/// Prioritetsnivåer för uppgifter.
/// </summary>
public enum TaskPriority
{
    Low = 0,
    Normal = 1,
    High = 2
}