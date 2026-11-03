using KanbanBoard.Models;

namespace KanbanBoard.Strategies;

/// <summary>
/// Använder Strategy-mönstret för att sortera Kanban-uppgifter.
/// Sorteringsalgoritmen kan bytas utan att ändra denna klass.
/// </summary>
public class TaskSortService
{
    private ITaskSortStrategy _strategy;

    public TaskSortService(ITaskSortStrategy strategy)
    {
        _strategy = strategy;
    }

    /// <summary>
    /// Byter sorteringsstrategi under körning.
    /// </summary>
    public void SetStrategy(ITaskSortStrategy strategy)
    {
        _strategy = strategy;
    }

    /// <summary>
    /// Sorterar uppgifter med den valda strategin.
    /// </summary>
    public IEnumerable<KanbanTask> SortTasks(
        IEnumerable<KanbanTask> tasks)
    {
        return _strategy.Sort(tasks);
    }
}