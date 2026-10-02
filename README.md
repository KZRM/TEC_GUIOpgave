# Feedr

Husk at have .NET 10 og SQL Server installeret først. Projektet er sat til SQL Server på `localhost` med Windows-login.

Har du ikke SQL Server, kan du bruge LocalDB:

1. Åbn Visual Studio Installer og vælg Modify.
2. Under Individual components, søg efter LocalDB og installer det.
3. Skift `Server=localhost` til `Server=(localdb)\\MSSQLLocalDB` i `Feedr/appsettings.json`.

Åbn `Feedr.sln` i Visual Studio og kør projektet. Databasen og testdata bliver oprettet automatisk, hvis databasen er tom eller mangler. Eksisterende data bliver ikke overskrevet.
