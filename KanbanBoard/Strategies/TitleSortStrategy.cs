using System.Globalization;
using KanbanBoard.Models;

namespace KanbanBoard.Strategies;

/// <summary>
/// Sorteringsstrategi som sorterar uppgifter alfabetiskt efter titel.
/// Använder svenska sorteringsregler så att Å, Ä och Ö hamnar efter Z.
/// </summary>
public class TitleSortStrategy : ITaskSortStrategy
{
    public IEnumerable<KanbanTask> Sort(IEnumerable<KanbanTask> tasks)
    {
        // Svensk kultur används för att få korrekt alfabetisk ordning.
        // StringComparer hanterar även stora och små bokstäver.
        var comparer = StringComparer.Create(
            new CultureInfo("sv-SE"),
            ignoreCase: true);

        // Om två uppgifter har samma titel sorteras de efter
        // skapandedatum, med den äldsta uppgiften först.
        return tasks
            .OrderBy(task => task.Title, comparer)
            .ThenBy(task => task.CreatedAt)
            .ToList();
    }
}