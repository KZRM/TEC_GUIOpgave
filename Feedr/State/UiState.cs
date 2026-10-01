namespace Feedr.State;

// 2.2–2.3: State husker sprogvalget; delvis rendering viser resultatet af valget.
// Klassen registreres som scoped, så hver Blazor-forbindelse har sin egen tilstand.
// Valget overlever tabelskift, men gemmes ikke permanent eller på tværs af en fuld sidegenindlæsning.
public class UiState
{
    public Language Language { get; set; } = Language.Danish;
}
