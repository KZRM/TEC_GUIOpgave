using Feedr.State;

namespace Feedr;

// Fælles oversættelser. Hver metode får brugerens sprog som argument.
// _ bruger dansk som standard, hvis sproget ikke har en case endnu.
public static class Translations
{
    public static string DatabaseOverview(Language language) => language switch
    {
        Language.Danish => "Databaseoversigt",
        Language.English => "Database overview",
        _ => "Databaseoversigt"
    };

    public static string ServiceStatus(Language language) => language switch
    {
        Language.Danish => "Databaseservicen kører.",
        Language.English => "The database service is running.",
        _ => "Databaseservicen kører."
    };

    public static string NewRecord(Language language) => language switch
    {
        Language.Danish => "Ny post",
        Language.English => "New record",
        _ => "Ny post"
    };

    public static string EditRecord(Language language) => language switch
    {
        Language.Danish => "Rediger post",
        Language.English => "Edit record",
        _ => "Rediger post"
    };

    public static string ChangesSaved(Language language) => language switch
    {
        Language.Danish => "Ændringerne er gemt.",
        Language.English => "Changes saved.",
        _ => "Ændringerne er gemt."
    };

    public static string Actions(Language language) => language switch
    {
        Language.Danish => "Handlinger",
        Language.English => "Actions",
        _ => "Handlinger"
    };

    public static string Edit(Language language) => language switch
    {
        Language.Danish => "Rediger",
        Language.English => "Edit",
        _ => "Rediger"
    };

    public static string PageInfo(Language language, int pageNumber, int pageSize) => language switch
    {
        Language.Danish => $"Side {pageNumber} · Højst {pageSize} rækker pr. side",
        Language.English => $"Page {pageNumber} · Up to {pageSize} rows per page",
        _ => $"Side {pageNumber} · Højst {pageSize} rækker pr. side"
    };

    public static string Previous(Language language) => language switch
    {
        Language.Danish => "Forrige",
        Language.English => "Previous",
        _ => "Forrige"
    };

    public static string Next(Language language) => language switch
    {
        Language.Danish => "Næste",
        Language.English => "Next",
        _ => "Næste"
    };

    public static string Automatic(Language language) => language switch
    {
        Language.Danish => "Automatisk",
        Language.English => "Automatic",
        _ => "Automatisk"
    };

    public static string Save(Language language) => language switch
    {
        Language.Danish => "Gem",
        Language.English => "Save",
        _ => "Gem"
    };

    public static string Cancel(Language language) => language switch
    {
        Language.Danish => "Annuller",
        Language.English => "Cancel",
        _ => "Annuller"
    };

    public static string Delete(Language language) => language switch
    {
        Language.Danish => "Slet",
        Language.English => "Delete",
        _ => "Slet"
    };

    public static string DeleteQuestion(Language language) => language switch
    {
        Language.Danish => "Slet denne post?",
        Language.English => "Delete this record?",
        _ => "Slet denne post?"
    };

    public static string ConfirmDelete(Language language) => language switch
    {
        Language.Danish => "Bekræft sletning",
        Language.English => "Confirm delete",
        _ => "Bekræft sletning"
    };

    public static string KeepRecord(Language language) => language switch
    {
        Language.Danish => "Behold posten",
        Language.English => "Keep record",
        _ => "Behold posten"
    };

    public static string LockedFieldsError(Language language) => language switch
    {
        Language.Danish => "ID og låste felter må ikke ændres.",
        Language.English => "IDs and locked fields cannot be changed.",
        _ => "ID og låste felter må ikke ændres."
    };

    public static string SaveError(Language language) => language switch
    {
        Language.Danish => "Kunne ikke gemme. Kontrollér værdier og henviste ID'er; unikke værdier må ikke findes i forvejen.",
        Language.English => "Could not save. Check the values and referenced IDs; unique values must not already exist.",
        _ => "Kunne ikke gemme. Kontrollér værdier og henviste ID'er; unikke værdier må ikke findes i forvejen."
    };

    public static string DeleteError(Language language) => language switch
    {
        Language.Danish => "Kunne ikke slette. Posten kan være i brug i en anden tabel eller allerede ændret.",
        Language.English => "Could not delete. The record may be in use by another table or already changed.",
        _ => "Kunne ikke slette. Posten kan være i brug i en anden tabel eller allerede ændret."
    };
}
