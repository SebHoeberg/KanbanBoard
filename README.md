# KanbanBoard

KanbanBoard är en personlig webbapplikation för uppgiftshantering, utvecklad i C# med ASP.NET Core MVC. Applikationen gör det möjligt att organisera uppgifter på olika Kanban-tavlor och följa arbetets framsteg.

Projektet är utvecklat inom kursen **APR400 – Avancerad programmering i C#** vid Högskolan Väst.

## Funktioner

- Skapa, redigera och ta bort Kanban-tavlor.
- Skapa, redigera och ta bort uppgifter.
- Organisera uppgifter i fyra kolumner: Backlogg, Att göra, Pågår och Klart.
- Flytta uppgifter mellan kolumner med drag-and-drop eller knappar.
- Ange prioritet, beskrivning och deadline för uppgifter.
- Sortera uppgifter efter prioritet, deadline eller titel.
- Spara tavlor och uppgifter lokalt i en SQLite-databas.
- Mörkt och responsivt användargränssnitt.

## Tekniker

| Teknik | Användning |
|---|---|
| C# / .NET 10 | Applikationens backend |
| ASP.NET Core MVC | Controllers, modeller och Razor-vyer |
| Entity Framework Core | Databashantering |
| SQLite | Lokal databas |
| HTML, CSS och JavaScript | Användargränssnitt och drag-and-drop |
| Bootstrap | Layout och responsiv design |
| xUnit | Enhetstester |

## Design Pattern – Strategy

Projektet använder designmönstret **Strategy Pattern** för att hantera sortering av uppgifter.

En gemensam interface, `ITaskSortStrategy`, definierar hur sortering ska utföras. Tre olika strategier implementerar detta interface:

- `PrioritySortStrategy` – sortering efter prioritet.
- `DeadlineSortStrategy` – sortering efter deadline.
- `TitleSortStrategy` – alfabetisk sortering efter titel.

Klassen `TaskSortService` använder den valda strategin för att sortera uppgifterna. På så sätt kan sorteringsalgoritmen bytas utan att övrig kod behöver ändras.

## Projektstruktur

```text
KanbanBoard/
├── Controllers/
├── Data/
├── Models/
├── Strategies/
├── Views/
├── wwwroot/
└── Program.cs

KanbanBoard.Tests/
└── Enhetstester för sorteringsstrategierna
```

## Köra projektet

För att köra applikationen behöver du .NET 10 SDK.

1. Klona projektet från GitHub.
2. Öppna solution-filen i JetBrains Rider eller en annan kompatibel utvecklingsmiljö.
3. Återställ projektets NuGet-paket.
4. Skapa databasen med Entity Framework Core-migrationerna.
5. Starta webbapplikationen.

Exempel på kommandon från webbprojektets mapp:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

## Enhetstester

Projektet innehåller enhetstester med xUnit som kontrollerar sorteringsstrategierna.

Testerna kan köras från solution-mappen med:

```bash
dotnet test
```

Vid senaste testkörningen godkändes samtliga 5 tester.

## Avgränsningar

KanbanBoard är utvecklad som en personlig uppgiftshanterare och innehåller därför inget system för flera användare eller inloggning.

Applikationen använder en lokal SQLite-databas och är i första hand avsedd att köras lokalt.

## Utveckling

Projektet är utvecklat i JetBrains Rider som en del av en individuell programmeringsuppgift. AI-verktyg har använts som stöd vid bland annat kodgenerering, felsökning och utveckling av funktioner. Koden har testats och vidareutvecklats under arbetets gång.
