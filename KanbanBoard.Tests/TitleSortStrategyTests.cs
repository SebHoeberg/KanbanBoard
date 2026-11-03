using KanbanBoard.Models;
using KanbanBoard.Strategies;
using Xunit;

namespace KanbanBoard.Tests;

/// <summary>
/// Enhetstest för alfabetisk sortering av uppgifter.
/// Kontrollerar att svenska bokstäver sorteras korrekt.
/// </summary>
public class TitleSortStrategyTests
{
    [Fact]
    public void Sort_UsesSwedishAlphabeticalOrder()
    {
        // Arrange: Uppgifterna placeras medvetet i fel ordning.
        // Testar också att stora och små bokstäver hanteras.
        var tasks = new List<KanbanTask>
        {
            new KanbanTask { Title = "Öva presentation" },
            new KanbanTask { Title = "Ändra design" },
            new KanbanTask { Title = "Skriva rapport" },
            new KanbanTask { Title = "Åtgärda buggar" },
            new KanbanTask { Title = "bygga API" }
        };

        var strategy = new TitleSortStrategy();

        // Act: Sorterar uppgifterna med svenska språkregler.
        var result = strategy.Sort(tasks)
            .Select(task => task.Title)
            .ToList();

        // Assert: Kontrollerar hela den förväntade ordningen.
        var expected = new[]
        {
            "bygga API",
            "Skriva rapport",
            "Åtgärda buggar",
            "Ändra design",
            "Öva presentation"
        };

        Assert.Equal(expected, result);
    }
}