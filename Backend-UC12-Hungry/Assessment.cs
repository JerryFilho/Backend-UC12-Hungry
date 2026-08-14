using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_UC12_Hungry;

/// <summary>
/// Classe Active Record que representa uma Avaliação (Assessment) realizada por um cliente sobre um agendamento.
/// Encapsula as propriedades de dados e os métodos de acesso e persistência no banco de dados.
/// Responsabilidade do integrante: João.
/// </summary>
public class Assessment
{
    // Identificador único da avaliação (Chave Primária autoincrement no banco)
    public int Id { get; set; }

    // ID do usuário que fez a avaliação (Chave Estrangeira -> users.id)
    public int UserId { get; set; }

    // ID do agendamento avaliado (Chave Estrangeira -> schedulles.id)
    public int SchedullesId { get; set; }

    // Nota da avaliação (ex: de 1 a 5)
    public int Note { get; set; }

    // Comentário sobre a experiência no estabelecimento (opcional)
    public string? Comment { get; set; }

    // Data de criação do registro no banco
    public DateTime CreatedAt { get; set; }

    // Data de atualização do registro
    public DateTime UpdatedAt { get; set; }

    public const string tabela = "assessment";

    public Assessment() { }

    public Assessment(int id, int userId, int schedullesId, int note, string? comment)
    {
        Id = id;
        UserId = userId;
        SchedullesId = schedullesId;
        Note = note;
        Comment = comment;
    }

    /// <summary>
    /// Exibe no console os detalhes desta avaliação.
    /// </summary>
    public void Mostrar()
    {
        Console.WriteLine($"--------------------------------------------------");
        Console.WriteLine($"ID: {Id} | Nota: {Note}/5");
        Console.WriteLine($"Comentário: {Comment ?? "Sem comentário"}");
        Console.WriteLine($"Agendamento ID: {SchedullesId} | Usuário ID: {UserId}");
        Console.WriteLine($"Criada em: {CreatedAt:dd/MM/yyyy HH:mm}");
    }

    /// <summary>
    /// Exibe uma lista de avaliações formatada.
    /// </summary>
    public void Mostrar(List<Assessment> avaliacoes)
    {
        if (avaliacoes.Count == 0)
        {
            Console.WriteLine("Nenhuma avaliação encontrada.");
            return;
        }

        foreach (var item in avaliacoes)
        {
            item.Mostrar();
        }
    }

    /// <summary>
    /// Inserir uma nova avaliação no banco de dados.
    /// Operação Active Record: CREATE (Inserir)
    /// </summary>
    public async Task<int> InserirAsync()
    {
        string query = $"""
            INSERT INTO {tabela} 
            (userId, schedullesId, note, comment, createdAt, updatedAt) 
            VALUES 
            (@userId, @schedullesId, @note, @comment, NOW(), NOW());
            SELECT LAST_INSERT_ID();
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        comando.Parameters.AddWithValue("@userId", UserId);
        comando.Parameters.AddWithValue("@schedullesId", SchedullesId);
        comando.Parameters.AddWithValue("@note", Note);
        comando.Parameters.AddWithValue("@comment", (object?)Comment ?? DBNull.Value);

        await conexao.OpenAsync();
        var result = await comando.ExecuteScalarAsync();
        this.Id = Convert.ToInt32(result);
        return this.Id;
    }

    public int Inserir()
    {
        return InserirAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Carrega os dados de uma avaliação do banco de dados neste próprio objeto (por ID).
    /// Operação Active Record: READ (Ler por ID na instância)
    /// </summary>
    public async Task<bool> BuscaAsync(int id)
    {
        string query = $"""
            SELECT id, userId, schedullesId, note, comment, createdAt, updatedAt 
            FROM {tabela} 
            WHERE id = @id;
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);
        comando.Parameters.AddWithValue("@id", id);

        await conexao.OpenAsync();
        await using var reader = await comando.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            Mapear(reader, this);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Buscar uma avaliação por ID e retornar uma nova instância.
    /// Operação Active Record: READ (Ler por ID)
    /// </summary>
    public static async Task<Assessment?> BuscarPorIdAsync(int id)
    {
        Assessment item = new Assessment();
        bool encontrado = await item.BuscaAsync(id);
        return encontrado ? item : null;
    }

    public static Assessment? BuscarPorId(int id)
    {
        return BuscarPorIdAsync(id).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Listar todas as avaliações registradas.
    /// Operação Active Record: READ (Ler Todos)
    /// </summary>
    public async Task<List<Assessment>> BuscarTodosAsync()
    {
        string query = $"""
            SELECT id, userId, schedullesId, note, comment, createdAt, updatedAt 
            FROM {tabela};
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        await conexao.OpenAsync();
        await using var reader = await comando.ExecuteReaderAsync();

        List<Assessment> lista = new();
        while (await reader.ReadAsync())
        {
            Assessment item = new Assessment();
            Mapear(reader, item);
            lista.Add(item);
        }

        return lista;
    }

    public List<Assessment> ListarTodos()
    {
        return BuscarTodosAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Buscar avaliações vinculadas a um agendamento específico.
    /// </summary>
    public async Task<List<Assessment>> BuscarPorAgendamentoAsync(int schedullesId)
    {
        string query = $"""
            SELECT id, userId, schedullesId, note, comment, createdAt, updatedAt 
            FROM {tabela} 
            WHERE schedullesId = @schedullesId;
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);
        comando.Parameters.AddWithValue("@schedullesId", schedullesId);

        await conexao.OpenAsync();
        await using var reader = await comando.ExecuteReaderAsync();

        List<Assessment> lista = new();
        while (await reader.ReadAsync())
        {
            Assessment item = new Assessment();
            Mapear(reader, item);
            lista.Add(item);
        }

        return lista;
    }

    public List<Assessment> BuscarPorAgendamento(int schedullesId)
    {
        return BuscarPorAgendamentoAsync(schedullesId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Remover uma avaliação do banco de dados pelo ID.
    /// Operação Active Record: DELETE (Remover)
    /// </summary>
    public async Task<bool> RemoverAsync(int id)
    {
        string query = $"""
            DELETE FROM {tabela} 
            WHERE id = @id;
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);
        comando.Parameters.AddWithValue("@id", id);

        await conexao.OpenAsync();
        int linhasAfetadas = await comando.ExecuteNonQueryAsync();
        return linhasAfetadas > 0;
    }

    public bool Remover(int id)
    {
        return RemoverAsync(id).GetAwaiter().GetResult();
    }

    private static void Mapear(MySqlDataReader reader, Assessment item)
    {
        item.Id = reader.GetInt32("id");
        item.UserId = reader.GetInt32("userId");
        item.SchedullesId = reader.GetInt32("schedullesId");
        item.Note = reader.GetInt32("note");
        item.Comment = reader.IsDBNull(reader.GetOrdinal("comment")) ? null : reader.GetString("comment");
        item.CreatedAt = reader.GetDateTime("createdAt");
        item.UpdatedAt = reader.GetDateTime("updatedAt");
    }
}
