# Feedr

JavaScript-opgaven ligger under Filklient. Her kan du uploade og hente filer. De kan slettes under Filer på serveren. Test API åbner Swagger, når projektet kører i Development.

Åbn `Feedr.sln` i Visual Studio og kør. Du skal have .NET 10 og SQL sat op først, se nederst.

Klienten ligger i `Feedr/wwwroot/client`. Flytter du den til en anden webserver, skal du rette API-adressen i `config.js` og tilføje klientens adresse under `FileClient.AllowedOrigins` i serverens `appsettings.json`.

Dokumentation til JavaScript: [PDF](docs/JavaScript-dokumentation.pdf) / [Word](docs/JavaScript-dokumentation.docx)

## Databasen fra den tidligere GUI-opgave

Databasesiden er stadig med, så SQL Server skal være sat op. Projektet bruger `localhost` med Windows-login.

Har du ikke SQL Server, kan du bruge LocalDB:

1. Åbn Visual Studio Installer og tryk Modify.
2. Find LocalDB under Individual components og installer det.
3. Skift `Server=localhost` til `Server=(localdb)\\MSSQLLocalDB` i `Feedr/appsettings.json`.

Databasen og lidt testdata bliver lavet automatisk, hvis databasen mangler eller er tom. Dine egne data bliver ikke overskrevet.

Dokumentation til GUI-opgaven: [PDF](docs/GUI-dokumentation.pdf) / [Word](docs/GUI-dokumentation-GoogleDocs.docx)
