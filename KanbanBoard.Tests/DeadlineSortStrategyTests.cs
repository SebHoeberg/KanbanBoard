using KanbanBoard.Models;
using KanbanBoard.Strategies;
using Xunit;

namespace KanbanBoard.Tests;

/// <summary>
/// Enhetstester för sortering efter deadline.
/// Kontrollerar även hur uppgifter utan deadline hanteras.
/// </summary>
public class DeadlineSortStrategyTests
{
    [Fact]
    public void Sort_EarliestDeadlineComesFirst_AndNoDeadlineLast()
    {
        // Arrange: Skapar tre uppgifter med olika deadlines.
        // En uppgift saknar deadline helt.
        var tasks = new List<KanbanTask>
        {
            new KanbanTask
            {
                Title = "Ingen deadline",
                DueDate = null
            },
            new KanbanTask
            {
                Title = "Sen deadline",
                DueDate = new DateTime(2026, 11, 20)
            },
            new KanbanTask
            {
                Title = "Tidig deadline",
                DueDate = new DateTime(2026, 11, 5)
            }
        };

        var strategy = new DeadlineSortStrategy();

        // Act: Sorterar uppgifterna efter deadline.
        var result = strategy.Sort(tasks).ToList();

        // Assert: Tidigaste datumet ska komma först.
        Assert.Equal("Tidig deadline", result[0].Title);
        Assert.Equal("Sen deadline", result[1].Title);
        Assert.Equal("Ingen deadline", result[2].Title);
    }
}