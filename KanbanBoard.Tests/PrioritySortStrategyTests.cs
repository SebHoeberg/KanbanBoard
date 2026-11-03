using KanbanBoard.Models;
using KanbanBoard.Strategies;
using Xunit;

namespace KanbanBoard.Tests;

/// <summary>
/// Enhetstester för sortering av uppgifter efter prioritet.
/// Testerna körs utan databas eller webbserver.
/// </summary>
public class PrioritySortStrategyTests
{
    [Fact]
    public void Sort_HighPriorityComesBeforeLowPriority()
    {
        // Arrange: Skapar tre uppgifter med olika prioritet.
        // De läggs medvetet i fel ordning för att testa sorteringen.
        var tasks = new List<KanbanTask>
        {
            new KanbanTask
            {
                Title = "Låg prioritet",
                Priority = TaskPriority.Low
            },
            new KanbanTask
            {
                Title = "Normal prioritet",
                Priority = TaskPriority.Normal
            },
            new KanbanTask
            {
                Title = "Hög prioritet",
                Priority = TaskPriority.High
            }
        };

        var strategy = new PrioritySortStrategy();

        // Act: Kör sorteringsalgoritmen.
        var result = strategy.Sort(tasks).ToList();

        // Assert: Kontrollerar att uppgifterna hamnat rätt.
        Assert.Equal(TaskPriority.High, result[0].Priority);
        Assert.Equal(TaskPriority.Normal, result[1].Priority);
        Assert.Equal(TaskPriority.Low, result[2].Priority);
    }
}