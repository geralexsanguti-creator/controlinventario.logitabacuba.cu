using Microsoft.Data.Sqlite;
using System.Globalization;

var csvPath = @"C:\Users\German\source\repos\ControlInventario\productores.csv";
var dbPath = @"C:\Users\German\source\repos\ControlInventario\ControlInventarioApi\inventario.db";

var connectionString = $"Data Source={dbPath}";

using var connection = new SqliteConnection(connectionString);
connection.Open();

var lines = File.ReadAllLines(csvPath);
var header = lines[0].Split(',');

Console.WriteLine($"Importando {lines.Length - 1} productores...");

int imported = 0;
int errors = 0;

for (int i = 1; i < lines.Length; i++)
{
    var values = ParseCsvLine(lines[i]);
    if (values.Length < 10) continue;
    
    try
    {
        var id = int.Parse(values[0]);
        var prioridad = string.IsNullOrEmpty(values[1]) ? null : values[1];
        var esColectivo = int.Parse(values[2]);
        var observaciones = values[3];
        var empresaId = string.IsNullOrEmpty(values[4]) ? null : values[4];
        var isActive = int.Parse(values[5]);
        var apellidos = values[6];
        var ci = values[7];
        var nombre = values[8];
        var municipioId = int.Parse(values[9]);

        var sql = @"
            INSERT OR REPLACE INTO Productores 
            (Id, Prioridad, es_colectivo, Observaciones, EmpresaId, is_active, Apellidos, CI, Nombre, MunicipioId)
            VALUES 
            (@id, @prioridad, @esColectivo, @observaciones, @empresaId, @isActive, @apellidos, @ci, @nombre, @municipioId)";

        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@prioridad", string.IsNullOrEmpty(prioridad) ? DBNull.Value : prioridad);
        cmd.Parameters.AddWithValue("@esColectivo", esColectivo);
        cmd.Parameters.AddWithValue("@observaciones", observaciones ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@empresaId", string.IsNullOrEmpty(empresaId) ? DBNull.Value : empresaId);
        cmd.Parameters.AddWithValue("@isActive", isActive);
        cmd.Parameters.AddWithValue("@apellidos", apellidos);
        cmd.Parameters.AddWithValue("@ci", ci);
        cmd.Parameters.AddWithValue("@nombre", nombre);
        cmd.Parameters.AddWithValue("@municipioId", municipioId);
        
        cmd.ExecuteNonQuery();
        imported++;
        Console.WriteLine($"✓ Productor {id}: {nombre} {apellidos}");
    }
    catch (Exception ex)
    {
        errors++;
        Console.WriteLine($"✗ Error en línea {i}: {ex.Message}");
    }
}

Console.WriteLine($"\n=== Resumen ===");
Console.WriteLine($"Importados: {imported}");
Console.WriteLine($"Errores: {errors}");

static string[] ParseCsvLine(string line)
{
    var result = new List<string>();
    var current = "";
    var inQuotes = false;
    
    foreach (var c in line)
    {
        if (c == '"')
        {
            inQuotes = !inQuotes;
        }
        else if (c == ',' && !inQuotes)
        {
            result.Add(current);
            current = "";
        }
        else
        {
            current += c;
        }
    }
    result.Add(current);
    return result.ToArray();
}
