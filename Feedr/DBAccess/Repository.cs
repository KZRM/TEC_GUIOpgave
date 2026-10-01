using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Feedr.DBAccess;

public class Repository(IDbContextFactory<FeedrDBContext> contextFactory)
{
    public string[] GetTableNames()
    {
        // 1.2: Hent de aktuelle tabelnavne direkte fra databasen i dbo-schemaet.
        using var db = contextFactory.CreateDbContext();
        return db.Database.SqlQueryRaw<string>(
                "SELECT name AS [Value] FROM sys.tables "
                + "WHERE schema_id = SCHEMA_ID('dbo') AND is_ms_shipped = 0")
            .OrderBy(name => name)
            .ToArray();
    }

    public int GetRecordCount(string tableName)
    {
        // Accepter kun et tabelnavn fra dropdownens liste.
        if (!GetTableNames().Contains(tableName)) throw new ArgumentException("Ukendt tabel.");

        using var db = contextFactory.CreateDbContext();
        var sql = $"SELECT COUNT(*) FROM [dbo].[{tableName}]";
        var connection = db.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        connection.Open();

        // ExecuteScalar returnerer én værdi: antallet fra COUNT(*).
        return (int)command.ExecuteScalar()!;
    }

    public DataTable GetTable(string tableName, int page, int pageSize)
    {
        // Accepter kun et tabelnavn fra dropdownens liste.
        if (!GetTableNames().Contains(tableName)) throw new ArgumentException("Ukendt tabel.");

        using var db = contextFactory.CreateDbContext();

        // ORDER BY 1 sorterer efter kolonne 1 -> ID i alle tabeller.
        // OFFSET springer tidligere sider over. Den ekstra række bruges af Næste-knappen.
        var sql = $"SELECT * FROM [dbo].[{tableName}] ORDER BY 1 "
            + $"OFFSET {page * pageSize} ROWS FETCH NEXT {pageSize + 1} ROWS ONLY";

        // Fill henter data og klarer selv åbning og lukning af forbindelsen.
        using var adapter = new SqlDataAdapter(sql, (SqlConnection)db.Database.GetDbConnection());

       // Find også tabellens primærnøgle, så den kan låses ved redigering.
adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
        var result = new DataTable();
        adapter.Fill(result);
        return result;
    }

    public void SaveChanges(string tableName, DataTable changes)
    {
        if (!GetTableNames().Contains(tableName))
            throw new ArgumentException("Ukendt tabel.");

        using var db = contextFactory.CreateDbContext();
        using var adapter = new SqlDataAdapter(
            $"SELECT * FROM [dbo].[{tableName}]", (SqlConnection)db.Database.GetDbConnection());

        // CommandBuilder laver SQL med parametre. Rækkens tilstand vælger INSERT, UPDATE eller DELETE.
        using var builder = new SqlCommandBuilder(adapter);
        adapter.Update(changes);
    }
}
