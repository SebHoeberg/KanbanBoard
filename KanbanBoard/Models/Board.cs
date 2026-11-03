using System.ComponentModel.DataAnnotations;

namespace KanbanBoard.Models;

/// <summary>
/// Representerar en personlig Kanban-tavla.
/// Varje tavla kommer senare att innehålla
/// egna uppgifter fördelade på fyra kolumner.
/// </summary>
public class Board
{
    // Primärnyckel som identifierar varje tavla i databasen.
    public int Id { get; set; }

    // Tavlans namn är obligatoriskt och får innehålla högst 100 tecken.
    [Required(ErrorMessage = "Du måste ange ett namn på tavlan.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    // En frivillig beskrivning av vad tavlan används till.
    [StringLength(500)]
    public string? Description { get; set; }

    // Datumet sparas i UTC för konsekvent tidslagring.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}