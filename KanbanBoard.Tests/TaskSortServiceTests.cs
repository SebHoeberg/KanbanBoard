using KanbanBoard.Models;
using KanbanBoard.Strategies;
using Xunit;

namespace KanbanBoard.Tests;

/// <summary>
/// Testar att TaskSortService kan byta sorteringsstrategi
/// under körning, vilket är kärnan i Strategy-mönstret.
/// </summary>
public class TaskSortServiceTests
{
    [Fact]
    public void SortTasks_CanSwitchStrategyAtRuntime()
    {
        // Arrange: Två uppgifter där prioritet och titel
        // ger olika sorteringsordning.
        var tasks = new List<KanbanTask>
        {
            new KanbanTask
            {
                Title = "Öva presentation",
                Priority = TaskPriority.High
            },
            new KanbanTask
            {
                Title = "Bygga API",
                Priority = TaskPriority.Low
            }
        };

        // Börjar med att sortera efter prioritet.
        var sortService = new TaskSortService(
            new PrioritySortStrategy());

        // Act: Sorterar först efter prioritet.
        var priorityResult = sortService.SortTasks(tasks).ToList();

        // Byter sedan strategi i samma tjänst.
        sortService.SetStrategy(new TitleSortStrategy());

        // Sorterar samma uppgifter igen, nu efter titel.
        var titleResult = sortService.SortTasks(tasks).ToList();

        // Assert: Hög prioritet ska komma först.
        Assert.Equal("Öva presentation", priorityResult[0].Title);

        // Vid alfabetisk sortering ska Bygga API komma först.
        Assert.Equal("Bygga API", titleResult[0].Title);
    }
}