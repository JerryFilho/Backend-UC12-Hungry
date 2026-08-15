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
    public User? user { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int? UserId
    {
        get => user?.Id;
        set
        {
            if (value.HasValue)
            {
                user ??= new User();
                user.Id = value.Value;
            }
            else
            {
                user = null;
            }
        }
    }

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
        User? user,
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
        this.user = user;
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
        User? user)
    {
        Name = name;
        Category = category;
        Cnpj = cnpj;
        Places = places;
        Phone = phone;
        Fundation = fundation;
        Description = description;
        this.user = user;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public void Mostrar()
    {
        Console.WriteLine($"ID: {Id} | Nome: {Name} | Categoria: {Category} | CNPJ: {Cnpj} | Telefone: {Phone}");
        Console.WriteLine($"   Lugares: {Places} | Fundação: {Fundation:yyyy-MM-dd} | UserID: {user?.Id}");
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
        comando.Parameters.AddWithValue("userId", user != null ? user.Id : DBNull.Value);
        comando.Parameters.AddWithValue("createdAt", CreatedAt);
        comando.Parameters.AddWithValue("updatedAt", UpdatedAt);

        await conexao.OpenAsync();
        await comando.ExecuteNonQueryAsync();
    }

    public async Task BuscarAsync(int id)
    {
        string query = $"""
            SELECT c.*, 
                   u.name AS user_name, u.type AS user_type, u.email AS user_email, 
                   u.password AS user_password, u.birth_date AS user_birth, u.cpf AS user_cpf, 
                   u.createdAt AS user_created, u.updatedAt AS user_updated
            FROM {tabela} c
            LEFT JOIN users u ON c.user_id = u.id
            WHERE c.id = @id;
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
            
            if (!dados.IsDBNull(dados.GetOrdinal("user_id")))
            {
                user = new User(
                    dados.GetInt32("user_id"),
                    dados.GetString("user_name"),
                    dados.GetString("user_type"),
                    dados.GetString("user_email"),
                    dados.GetString("user_password"),
                    dados.GetDateTime("user_birth"),
                    dados.GetString("user_cpf"),
                    dados.GetDateTime("user_created"),
                    dados.GetDateTime("user_updated")
                );
            }
            else
            {
                user = null;
            }

            CreatedAt = dados.GetDateTime("created_at");
            UpdatedAt = dados.GetDateTime("updated_at");
        }
    }

    public async Task<List<Company>> BuscarTodosAsync()
    {
        string query = $"""
            SELECT c.*, 
                   u.name AS user_name, u.type AS user_type, u.email AS user_email, 
                   u.password AS user_password, u.birth_date AS user_birth, u.cpf AS user_cpf, 
                   u.createdAt AS user_created, u.updatedAt AS user_updated
            FROM {tabela} c
            LEFT JOIN users u ON c.user_id = u.id
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        await conexao.OpenAsync();
        await using var dados = await comando.ExecuteReaderAsync();

        List<Company> lista = new();
        while (await dados.ReadAsync())
        {
            User? relatedUser = null;
            if (!dados.IsDBNull(dados.GetOrdinal("user_id")))
            {
                relatedUser = new User(
                    dados.GetInt32("user_id"),
                    dados.GetString("user_name"),
                    dados.GetString("user_type"),
                    dados.GetString("user_email"),
                    dados.GetString("user_password"),
                    dados.GetDateTime("user_birth"),
                    dados.GetString("user_cpf"),
                    dados.GetDateTime("user_created"),
                    dados.GetDateTime("user_updated")
                );
            }

            Company company = new Company(
                dados.GetInt32("id"),
                dados.GetString("name"),
                dados.GetString("category"),
                dados.GetString("cnpj"),
                dados.GetString("places"),
                dados.GetString("phone"),
                dados.GetDateTime("fundation"),
                dados.GetString("description"),
                relatedUser,
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
        comando.Parameters.AddWithValue("userId", user != null ? user.Id : DBNull.Value);
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