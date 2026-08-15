using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; }
    public Company company { get; set; } = new Company();
    public User? user { get; set; }

    public int CompanyId
    {
        get => company.Id;
        set
        {
            company ??= new Company();
            company.Id = value;
        }
    }

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

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public const string tabela = "photos";

    public Photo() { }

    public Photo(
        int id,
        string url,
        Company company,
        User? user,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Url = url;
        this.company = company;
        this.user = user;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

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
        this.company = new Company { Id = companyId };
        if (userId.HasValue)
        {
            this.user = new User { Id = userId.Value };
        }
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Photo(
        string url,
        int companyId,
        int? userId)
    {
        Url = url;
        this.company = new Company { Id = companyId };
        if (userId.HasValue)
        {
            this.user = new User { Id = userId.Value };
        }
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public void Mostrar()
    {
        Console.WriteLine($"ID: {Id} | URL: {Url} | CompanyId: {company?.Id} (Nome: {company?.Name}) | UserId: {user?.Id} (Nome: {user?.Name})");
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
        comando.Parameters.AddWithValue("companyId", company.Id);
        comando.Parameters.AddWithValue("userId", user != null ? user.Id : DBNull.Value);
        comando.Parameters.AddWithValue("createdAt", CreatedAt);
        comando.Parameters.AddWithValue("updatedAt", UpdatedAt);

        await conexao.OpenAsync();
        await comando.ExecuteNonQueryAsync();
    }

    public async Task BuscarAsync(int id)
    {
        string query = $"""
            SELECT p.*, 
                   c.name AS comp_name, c.category AS comp_cat, c.cnpj AS comp_cnpj, 
                   c.places AS comp_places, c.phone AS comp_phone, c.fundation AS comp_fund, 
                   c.description AS comp_desc, c.created_at AS comp_created, c.updated_at AS comp_updated,
                   u.name AS user_name, u.type AS user_type, u.email AS user_email, 
                   u.password AS user_password, u.birth_date AS user_birth, u.cpf AS user_cpf, 
                   u.createdAt AS user_created, u.updatedAt AS user_updated
            FROM {tabela} p
            INNER JOIN companies c ON p.company_id = c.id
            LEFT JOIN users u ON p.user_id = u.id
            WHERE p.id = @id
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

            company = new Company(
                dados.GetInt32("company_id"),
                dados.GetString("comp_name"),
                dados.GetString("comp_cat"),
                dados.GetString("comp_cnpj"),
                dados.GetString("comp_places"),
                dados.GetString("comp_phone"),
                dados.GetDateTime("comp_fund"),
                dados.GetString("comp_desc"),
                user, // reference user
                dados.GetDateTime("comp_created"),
                dados.GetDateTime("comp_updated")
            );

            CreatedAt = dados.GetDateTime("created_at");
            UpdatedAt = dados.GetDateTime("updated_at");
        }
    }

    public async Task<List<Photo>> BuscarTodosAsync()
    {
        string query = $"""
            SELECT p.*, 
                   c.name AS comp_name, c.category AS comp_cat, c.cnpj AS comp_cnpj, 
                   c.places AS comp_places, c.phone AS comp_phone, c.fundation AS comp_fund, 
                   c.description AS comp_desc, c.created_at AS comp_created, c.updated_at AS comp_updated,
                   u.name AS user_name, u.type AS user_type, u.email AS user_email, 
                   u.password AS user_password, u.birth_date AS user_birth, u.cpf AS user_cpf, 
                   u.createdAt AS user_created, u.updatedAt AS user_updated
            FROM {tabela} p
            INNER JOIN companies c ON p.company_id = c.id
            LEFT JOIN users u ON p.user_id = u.id
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        await conexao.OpenAsync();
        await using var dados = await comando.ExecuteReaderAsync();

        List<Photo> lista = new();
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

            Company relatedCompany = new Company(
                dados.GetInt32("company_id"),
                dados.GetString("comp_name"),
                dados.GetString("comp_cat"),
                dados.GetString("comp_cnpj"),
                dados.GetString("comp_places"),
                dados.GetString("comp_phone"),
                dados.GetDateTime("comp_fund"),
                dados.GetString("comp_desc"),
                relatedUser,
                dados.GetDateTime("comp_created"),
                dados.GetDateTime("comp_updated")
            );

            Photo photo = new Photo(
                dados.GetInt32("id"),
                dados.GetString("url"),
                relatedCompany,
                relatedUser,
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
        comando.Parameters.AddWithValue("companyId", company.Id);
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