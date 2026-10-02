# Feedr

Husk at have .NET 10 og SQL Server installeret først. Projektet er sat til SQL Server på `localhost` med Windows-login.

Har du ikke SQL Server, kan du installere LocalDB i Visual Studio Installer under Modify → Individual components → søg efter LocalDB. Skift så `Server=localhost` til `Server=(localdb)\\MSSQLLocalDB` i `Feedr/appsettings.json`.

Åbn `Feedr.sln` i Visual Studio og kør projektet. Databasen og testdata bliver oprettet automatisk, hvis databasen er tom eller mangler. Eksisterende data bliver ikke overskrevet.
