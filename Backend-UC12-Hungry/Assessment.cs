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
    public User user { get; set; } = new User();

    // ID do agendamento avaliado (Chave Estrangeira -> schedulles.id)
    public Schedulle schedulle { get; set; } = new Schedulle();

    public int UserId
    {
        get => user.Id;
        set
        {
            user ??= new User();
            user.Id = value;
        }
    }

    public int SchedullesId
    {
        get => schedulle.Id;
        set
        {
            schedulle ??= new Schedulle();
            schedulle.Id = value;
        }
    }

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

    public Assessment(int id, User user, Schedulle schedulle, int note, string? comment)
    {
        Id = id;
        this.user = user;
        this.schedulle = schedulle;
        Note = note;
        Comment = comment;
    }

    public Assessment(int id, int userId, int schedullesId, int note, string? comment)
    {
        Id = id;
        this.user = new User { Id = userId };
        this.schedulle = new Schedulle { Id = schedullesId };
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
        Console.WriteLine($"Agendamento ID: {schedulle?.Id} | Usuário ID: {user?.Id} (Nome: {user?.Name})");
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

        comando.Parameters.AddWithValue("@userId", user.Id);
        comando.Parameters.AddWithValue("@schedullesId", schedulle.Id);
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
            SELECT a.*, 
                   u.name AS user_name, u.type AS user_type, u.email AS user_email, 
                   u.password AS user_password, u.birth_date AS user_birth, u.cpf AS user_cpf, 
                   u.createdAt AS user_created, u.updatedAt AS user_updated,
                   s.datetimez AS sched_time, s.type AS sched_type, s.people AS sched_people, 
                   s.observation AS sched_obs, s.creat_at AS sched_created, s.updated_at AS sched_updated,
                   s.company_id AS sched_company_id,
                   c.name AS comp_name, c.category AS comp_cat, c.cnpj AS comp_cnpj, 
                   c.places AS comp_places, c.phone AS comp_phone, c.fundation AS comp_fund, 
                   c.description AS comp_desc, c.created_at AS comp_created, c.updated_at AS comp_updated
            FROM {tabela} a
            INNER JOIN users u ON a.userId = u.id
            INNER JOIN schedulles s ON a.schedullesId = s.id
            INNER JOIN companies c ON s.company_id = c.id
            WHERE a.id = @id;
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
            SELECT a.*, 
                   u.name AS user_name, u.type AS user_type, u.email AS user_email, 
                   u.password AS user_password, u.birth_date AS user_birth, u.cpf AS user_cpf, 
                   u.createdAt AS user_created, u.updatedAt AS user_updated,
                   s.datetimez AS sched_time, s.type AS sched_type, s.people AS sched_people, 
                   s.observation AS sched_obs, s.creat_at AS sched_created, s.updated_at AS sched_updated,
                   s.company_id AS sched_company_id,
                   c.name AS comp_name, c.category AS comp_cat, c.cnpj AS comp_cnpj, 
                   c.places AS comp_places, c.phone AS comp_phone, c.fundation AS comp_fund, 
                   c.description AS comp_desc, c.created_at AS comp_created, c.updated_at AS comp_updated
            FROM {tabela} a
            INNER JOIN users u ON a.userId = u.id
            INNER JOIN schedulles s ON a.schedullesId = s.id
            INNER JOIN companies c ON s.company_id = c.id;
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
            SELECT a.*, 
                   u.name AS user_name, u.type AS user_type, u.email AS user_email, 
                   u.password AS user_password, u.birth_date AS user_birth, u.cpf AS user_cpf, 
                   u.createdAt AS user_created, u.updatedAt AS user_updated,
                   s.datetimez AS sched_time, s.type AS sched_type, s.people AS sched_people, 
                   s.observation AS sched_obs, s.creat_at AS sched_created, s.updated_at AS sched_updated,
                   s.company_id AS sched_company_id,
                   c.name AS comp_name, c.category AS comp_cat, c.cnpj AS comp_cnpj, 
                   c.places AS comp_places, c.phone AS comp_phone, c.fundation AS comp_fund, 
                   c.description AS comp_desc, c.created_at AS comp_created, c.updated_at AS comp_updated
            FROM {tabela} a
            INNER JOIN users u ON a.userId = u.id
            INNER JOIN schedulles s ON a.schedullesId = s.id
            INNER JOIN companies c ON s.company_id = c.id
            WHERE a.schedullesId = @schedullesId;
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
        
        item.user = new User(
            reader.GetInt32("userId"),
            reader.GetString("user_name"),
            reader.GetString("user_type"),
            reader.GetString("user_email"),
            reader.GetString("user_password"),
            reader.GetDateTime("user_birth"),
            reader.GetString("user_cpf"),
            reader.GetDateTime("user_created"),
            reader.GetDateTime("user_updated")
        );

        Company company = new Company(
            reader.GetInt32("sched_company_id"),
            reader.GetString("comp_name"),
            reader.GetString("comp_cat"),
            reader.GetString("comp_cnpj"),
            reader.GetString("comp_places"),
            reader.GetString("comp_phone"),
            reader.GetDateTime("comp_fund"),
            reader.GetString("comp_desc"),
            item.user, // reference user
            reader.GetDateTime("comp_created"),
            reader.GetDateTime("comp_updated")
        );

        item.schedulle = new Schedulle(
            reader.GetInt32("schedullesId"),
            reader.GetDateTime("sched_time"),
            reader.GetString("sched_type"),
            reader.GetInt32("sched_people"),
            reader.IsDBNull(reader.GetOrdinal("sched_obs")) ? null : reader.GetString("sched_obs"),
            item.user,
            company
        )
        {
            CreatedAt = reader.GetDateTime("sched_created"),
            UpdatedAt = reader.GetDateTime("sched_updated")
        };

        item.Note = reader.GetInt32("note");
        item.Comment = reader.IsDBNull(reader.GetOrdinal("comment")) ? null : reader.GetString("comment");
        item.CreatedAt = reader.GetDateTime("createdAt");
        item.UpdatedAt = reader.GetDateTime("updatedAt");
    }
}
