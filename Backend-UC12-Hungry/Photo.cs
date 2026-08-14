using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public const string tabela = "photos";

    public Photo() { }

    public Photo(
        int id,
        string url,
        int companyId,
        int? userId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Url = url;
        CompanyId = companyId;
        UserId = userId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Photo(
        string url,
        int companyId,
        int? userId)
    {
        Url = url;
        CompanyId = companyId;
        UserId = userId;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public void Mostrar()
    {
        Console.WriteLine($"ID: {Id} | URL: {Url} | CompanyId: {CompanyId} | UserId: {UserId}");
        Console.WriteLine($"   Criado em: {CreatedAt} | Atualizado em: {UpdatedAt}");
        Console.WriteLine("------------------------------------------------------------------");
    }

    public void Mostrar(List<Photo> fotos)
    {
        Console.WriteLine("=== LISTA DE FOTOS ===");
        if (fotos.Count == 0)
        {
            Console.WriteLine("Nenhuma foto cadastrada.");
            return;
        }

        foreach (var foto in fotos)
        {
            foto.Mostrar();
        }
    }

    public async Task InserirAsync()
    {
        string query = $"""
            INSERT INTO {tabela}
            (
                url,
                company_id,
                user_id,
                created_at,
                updated_at
            )
            VALUES
            (
                @url,
                @companyId,
                @userId,
                @createdAt,
                @updatedAt
            )
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        comando.Parameters.AddWithValue("url", Url);
        comando.Parameters.AddWithValue("companyId", CompanyId);
        comando.Parameters.AddWithValue("userId", UserId.HasValue ? UserId.Value : DBNull.Value);
        comando.Parameters.AddWithValue("createdAt", CreatedAt);
        comando.Parameters.AddWithValue("updatedAt", UpdatedAt);

        await conexao.OpenAsync();
        await comando.ExecuteNonQueryAsync();
    }

    public async Task BuscarAsync(int id)
    {
        string query = $"""
            SELECT
                id,
                url,
                company_id,
                user_id,
                created_at,
                updated_at
            FROM {tabela}
            WHERE id = @id
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);
        comando.Parameters.AddWithValue("id", id);

        await conexao.OpenAsync();
        await using var dados = await comando.ExecuteReaderAsync();

        if (await dados.ReadAsync())
        {
            Id = dados.GetInt32("id");
            Url = dados.GetString("url");
            CompanyId = dados.GetInt32("company_id");
            UserId = dados.IsDBNull(dados.GetOrdinal("user_id")) ? null : dados.GetInt32("user_id");
            CreatedAt = dados.GetDateTime("created_at");
            UpdatedAt = dados.GetDateTime("updated_at");
        }
    }

    public async Task<List<Photo>> BuscarTodosAsync()
    {
        string query = $"""
            SELECT
                id,
                url,
                company_id,
                user_id,
                created_at,
                updated_at
            FROM {tabela}
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        await conexao.OpenAsync();
        await using var dados = await comando.ExecuteReaderAsync();

        List<Photo> lista = new();
        while (await dados.ReadAsync())
        {
            int? userId = dados.IsDBNull(dados.GetOrdinal("user_id")) ? null : dados.GetInt32("user_id");

            Photo photo = new Photo(
                dados.GetInt32("id"),
                dados.GetString("url"),
                dados.GetInt32("company_id"),
                userId,
                dados.GetDateTime("created_at"),
                dados.GetDateTime("updated_at")
            );

            lista.Add(photo);
        }

        return lista;
    }

    public async Task AtualizarAsync()
    {
        string query = $"""
            UPDATE {tabela}
            SET
                url = @url,
                company_id = @companyId,
                user_id = @userId,
                updated_at = @updatedAt
            WHERE id = @id
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        comando.Parameters.AddWithValue("id", Id);
        comando.Parameters.AddWithValue("url", Url);
        comando.Parameters.AddWithValue("companyId", CompanyId);
        comando.Parameters.AddWithValue("userId", UserId.HasValue ? UserId.Value : DBNull.Value);
        comando.Parameters.AddWithValue("updatedAt", UpdatedAt);

        await conexao.OpenAsync();
        await comando.ExecuteNonQueryAsync();
    }

    public async Task RemoverAsync(int id)
    {
        string query = $"""
            DELETE FROM {tabela}
            WHERE id = @id
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);
        comando.Parameters.AddWithValue("id", id);

        await conexao.OpenAsync();
        await comando.ExecuteNonQueryAsync();
    }
}