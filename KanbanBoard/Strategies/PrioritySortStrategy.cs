using KanbanBoard.Models;

namespace KanbanBoard.Strategies;

/// <summary>
/// Sorteringsstrategi som placerar uppgifter med högst prioritet först.
/// Implementerar det gemensamma kontraktet ITaskSortStrategy.
/// </summary>
public class PrioritySortStrategy : ITaskSortStrategy
{
    public IEnumerable<KanbanTask> Sort(IEnumerable<KanbanTask> tasks)
    {
        // TaskPriority har värdena Low = 0, Normal = 1 och High = 2.
        // Därför sorterar vi fallande så att högsta värdet kommer först.
        //
        // Om två uppgifter har samma prioritet sorteras de efter
        // skapandedatum, med den äldsta uppgiften först.
        return tasks
            .OrderByDescending(task => task.Priority)
            .ThenBy(task => task.CreatedAt)
            .ToList();
    }
}