using KanbanBoard.Models;

namespace KanbanBoard.Strategies;

/// <summary>
/// Sorteringsstrategi som placerar uppgifter med närmast deadline först.
/// Uppgifter utan deadline hamnar sist.
/// </summary>
public class DeadlineSortStrategy : ITaskSortStrategy
{
    public IEnumerable<KanbanTask> Sort(IEnumerable<KanbanTask> tasks)
    {
        // Uppgifter utan deadline placeras sist.
        // false sorteras före true, vilket innebär att
        // uppgifter som har en deadline kommer först.
        //
        // Därefter sorterar vi på datum i stigande ordning.
        // Om två uppgifter har samma datum används skapandedatum.
        return tasks
            .OrderBy(task => !task.DueDate.HasValue)
            .ThenBy(task => task.DueDate)
            .ThenBy(task => task.CreatedAt)
            .ToList();
    }
}