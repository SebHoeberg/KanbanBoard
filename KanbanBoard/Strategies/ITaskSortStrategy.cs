using KanbanBoard.Models;

namespace KanbanBoard.Strategies;

/// <summary>
/// Gemensamt kontrakt för alla strategier som sorterar Kanban-uppgifter.
/// Varje strategi bestämmer själv hur uppgifterna ska ordnas.
/// </summary>
public interface ITaskSortStrategy
{
    // Tar emot uppgifter och returnerar dem i den ordning
    // som den aktuella strategin bestämmer.
    IEnumerable<KanbanTask> Sort(IEnumerable<KanbanTask> tasks);
}