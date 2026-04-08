using Microsoft.Data.Sqlite;

const string CsvPath = @"C:\Users\German\source\repos\ControlInventario\productores.csv";
const string DbPath = @"C:\Users\German\source\repos\ControlInventario\ControlInventarioApi\inventario.db";
const string InsertSql = """
    INSERT OR REPLACE INTO Productores 
    (Id, Prioridad, es_colectivo, Observaciones, EmpresaId, is_active, Apellidos, CI, Nombre, MunicipioId)
    VALUES 
    (@id, @prioridad, @esColectivo, @observaciones, @empresaId, @isActive, @apellidos, @ci, @nombre, @municipioId)
    """;

try
{
    var connectionString = $"Data Source={DbPath}";
    var importResult = await ImportProducersAsync(CsvPath, connectionString);
    
    Console.WriteLine("\n=== Resumen ===");
    Console.WriteLine($"Importados: {importResult.Imported}");
    Console.WriteLine($"Errores: {importResult.Errors}");
    Console.WriteLine($"Líneas omitidas: {importResult.Skipped}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error fatal: {ex.Message}");
    Environment.Exit(1);
}

static async Task<ImportResult> ImportProducersAsync(string csvPath, string connectionString)
{
    var result = new ImportResult();
    
    if (!File.Exists(csvPath))
    {
        throw new FileNotFoundException($"El archivo CSV no existe: {csvPath}");
    }

    var lines = await File.ReadAllLinesAsync(csvPath);
    
    if (lines.Length <= 1)
    {
        Console.WriteLine("El archivo CSV está vacío o solo contiene el encabezado.");
        return result;
    }

    Console.WriteLine($"Importando {lines.Length - 1} productores...");

    await using var connection = new SqliteConnection(connectionString);
    await connection.OpenAsync();

    for (int i = 1; i < lines.Length; i++)
    {
        var lineNumber = i + 1;
        var values = ParseCsvLine(lines[i]);
        
        if (values.Length < 10)
        {
            result.Skipped++;
            Console.WriteLine($"⚠ Línea {lineNumber} omitida: datos insuficientes");
            continue;
        }
        
        try
        {
            await ImportProducerAsync(connection, values, lineNumber);
            result.Imported++;
        }
        catch (Exception ex)
        {
            result.Errors++;
            Console.WriteLine($"✗ Error en línea {lineNumber}: {ex.Message}");
        }
    }

    return result;
}

static async Task ImportProducerAsync(SqliteConnection connection, string[] values, int lineNumber)
{
    var producer = ParseProducerData(values);
    
    await using var cmd = new SqliteCommand(InsertSql, connection);
    cmd.Parameters.AddWithValue("@id", producer.Id);
    cmd.Parameters.AddWithValue("@prioridad", string.IsNullOrEmpty(producer.Prioridad) ? DBNull.Value : producer.Prioridad);
    cmd.Parameters.AddWithValue("@esColectivo", producer.EsColectivo);
    cmd.Parameters.AddWithValue("@observaciones", string.IsNullOrEmpty(producer.Observaciones) ? DBNull.Value : producer.Observaciones);
    cmd.Parameters.AddWithValue("@empresaId", string.IsNullOrEmpty(producer.EmpresaId) ? DBNull.Value : producer.EmpresaId);
    cmd.Parameters.AddWithValue("@isActive", producer.IsActive);
    cmd.Parameters.AddWithValue("@apellidos", producer.Apellidos ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@ci", producer.CI ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@nombre", producer.Nombre ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@municipioId", producer.MunicipioId);
    
    await cmd.ExecuteNonQueryAsync();
    
    Console.WriteLine($"✓ Productor {producer.Id}: {producer.Nombre} {producer.Apellidos}");
}

static ProducerData ParseProducerData(string[] values)
{
    return new ProducerData
    {
        Id = ParseInt(values[0], "Id"),
        Prioridad = SafeGetString(values, 1),
        EsColectivo = ParseInt(values[2], "es_colectivo"),
        Observaciones = SafeGetString(values, 3),
        EmpresaId = SafeGetString(values, 4),
        IsActive = ParseInt(values[5], "is_active"),
        Apellidos = SafeGetString(values, 6),
        CI = SafeGetString(values, 7),
        Nombre = SafeGetString(values, 8),
        MunicipioId = ParseInt(values[9], "MunicipioId")
    };
}

static int ParseInt(string value, string fieldName)
{
    if (!int.TryParse(value, out var result))
    {
        throw new FormatException($"Valor inválido para {fieldName}: '{value}'");
    }
    return result;
}

static string? SafeGetString(string[] values, int index)
{
    return index < values.Length ? values[index] : null;
}

static string[] ParseCsvLine(string line)
{
    var result = new List<string>();
    var current = new System.Text.StringBuilder();
    var inQuotes = false;
    
    foreach (var c in line)
    {
        if (c == '"')
        {
            inQuotes = !inQuotes;
        }
        else if (c == ',' && !inQuotes)
        {
            result.Add(current.ToString());
            current.Clear();
        }
        else
        {
            current.Append(c);
        }
    }
    result.Add(current.ToString());
    return result.ToArray();
}

record ImportResult
{
    public int Imported { get; init; } = 0;
    public int Errors { get; init; } = 0;
    public int Skipped { get; init; } = 0;
}

record ProducerData
{
    public int Id { get; init; }
    public string? Prioridad { get; init; }
    public int EsColectivo { get; init; }
    public string? Observaciones { get; init; }
    public string? EmpresaId { get; init; }
    public int IsActive { get; init; }
    public string? Apellidos { get; init; }
    public string? CI { get; init; }
    public string? Nombre { get; init; }
    public int MunicipioId { get; init; }
}
