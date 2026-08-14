using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Cnpj { get; set; }
    public string Places { get; set; }
    public string Phone { get; set; }
    public DateTime Fundation { get; set; }
    public string Description { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public const string tabela = "companies";

    public Company() { }

    public Company(
        int id,
        string name,
        string category,
        string cnpj,
        string places,
        string phone,
        DateTime fundation,
        string description,
        int? userId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Name = name;
        Category = category;
        Cnpj = cnpj;
        Places = places;
        Phone = phone;
        Fundation = fundation;
        Description = description;
        UserId = userId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Company(
        string name,
        string category,
        string cnpj,
        string places,
        string phone,
        DateTime fundation,
        string description,
        int? userId)
    {
        Name = name;
        Category = category;
        Cnpj = cnpj;
        Places = places;
        Phone = phone;
        Fundation = fundation;
        Description = description;
        UserId = userId;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public void Mostrar()
    {
        Console.WriteLine($"ID: {Id} | Nome: {Name} | Categoria: {Category} | CNPJ: {Cnpj} | Telefone: {Phone}");
        Console.WriteLine($"   Lugares: {Places} | Fundação: {Fundation:yyyy-MM-dd} | UserID: {UserId}");
        Console.WriteLine($"   Descrição: {Description}");
        Console.WriteLine($"   Criado em: {CreatedAt} | Atualizado em: {UpdatedAt}");
        Console.WriteLine("------------------------------------------------------------------");
    }

    public void Mostrar(List<Company> empresas)
    {
        Console.WriteLine("=== LISTA DE EMPRESAS ===");
        if (empresas.Count == 0)
        {
            Console.WriteLine("Nenhuma empresa cadastrada.");
            return;
        }

        foreach (var emp in empresas)
        {
            emp.Mostrar();
        }
    }

    public async Task InserirAsync()
    {
        string query = $"""
            INSERT INTO {tabela}
            (
                name,
                category,
                cnpj,
                places,
                phone,
                fundation,
                description,
                user_id,
                created_at,
                updated_at
            )
            VALUES
            (
                @name,
                @category,
                @cnpj,
                @places,
                @phone,
                @fundation,
                @description,
                @userId,
                @createdAt,
                @updatedAt
            )
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        comando.Parameters.AddWithValue("name", Name);
        comando.Parameters.AddWithValue("category", Category);
        comando.Parameters.AddWithValue("cnpj", Cnpj);
        comando.Parameters.AddWithValue("places", Places);
        comando.Parameters.AddWithValue("phone", Phone);
        comando.Parameters.AddWithValue("fundation", Fundation);
        comando.Parameters.AddWithValue("description", Description);
        comando.Parameters.AddWithValue("userId", UserId.HasValue ? UserId.Value : DBNull.Value);
        comando.Parameters.AddWithValue("createdAt", CreatedAt);
        comando.Parameters.AddWithValue("updatedAt", UpdatedAt);

        await conexao.OpenAsync();
        await comando.ExecuteNonQueryAsync();
    }

    public async Task BuscarAsync(int id)
    {
        string query = $"""
            SELECT * FROM {tabela} WHERE id = @id;
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);
        comando.Parameters.AddWithValue("id", id);

        await conexao.OpenAsync();
        await using var dados = await comando.ExecuteReaderAsync();

        if (await dados.ReadAsync())
        {
            Id = dados.GetInt32("id");
            Name = dados.GetString("name");
            Category = dados.GetString("category");
            Cnpj = dados.GetString("cnpj");
            Places = dados.GetString("places");
            Phone = dados.GetString("phone");
            Fundation = dados.GetDateTime("fundation");
            Description = dados.GetString("description");
            UserId = dados.IsDBNull(dados.GetOrdinal("user_id")) ? null : dados.GetInt32("user_id");
            CreatedAt = dados.GetDateTime("created_at");
            UpdatedAt = dados.GetDateTime("updated_at");
        }
    }

    public async Task<List<Company>> BuscarTodosAsync()
    {
        string query = $"""
            SELECT
               *
            FROM {tabela}
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        await conexao.OpenAsync();
        await using var dados = await comando.ExecuteReaderAsync();

        List<Company> lista = new();
        while (await dados.ReadAsync())
        {
            int? userId = dados.IsDBNull(dados.GetOrdinal("user_id")) ? null : dados.GetInt32("user_id");

            Company company = new Company(
                dados.GetInt32("id"),
                dados.GetString("name"),
                dados.GetString("category"),
                dados.GetString("cnpj"),
                dados.GetString("places"),
                dados.GetString("phone"),
                dados.GetDateTime("fundation"),
                dados.GetString("description"),
                userId,
                dados.GetDateTime("created_at"),
                dados.GetDateTime("updated_at")
            );

            lista.Add(company);
        }

        return lista;
    }

    public async Task AtualizarAsync()
    {
        string query = $"""
            UPDATE {tabela}
            SET
                name = @name,
                category = @category,
                cnpj = @cnpj,
                places = @places,
                phone = @phone,
                fundation = @fundation,
                description = @description,
                user_id = @userId,
                updated_at = @updatedAt
            WHERE id = @id
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        comando.Parameters.AddWithValue("id", Id);
        comando.Parameters.AddWithValue("name", Name);
        comando.Parameters.AddWithValue("category", Category);
        comando.Parameters.AddWithValue("cnpj", Cnpj);
        comando.Parameters.AddWithValue("places", Places);
        comando.Parameters.AddWithValue("phone", Phone);
        comando.Parameters.AddWithValue("fundation", Fundation);
        comando.Parameters.AddWithValue("description", Description);
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