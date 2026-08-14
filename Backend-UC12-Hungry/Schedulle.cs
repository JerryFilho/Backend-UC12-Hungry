using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_UC12_Hungry;

/// <summary>
/// Classe Active Record que representa um Agendamento (Schedulle) no sistema.
/// Encapsula as propriedades de dados e os métodos de acesso e persistência no banco de dados.
/// Responsabilidade do integrante: João.
/// </summary>
public class Schedulle
{
    // Identificador único do agendamento (Chave Primária autoincrement no banco)
    public int Id { get; set; }

    // Data e horário agendados pelo cliente
    public DateTime Datetimez { get; set; }

    // Tipo de agendamento (ex: "Presencial", "Reserva de Mesa", etc.)
    public string Type { get; set; } = string.Empty;

    // Quantidade de pessoas vinculadas ao agendamento
    public int People { get; set; }

    // Observações adicionais enviadas pelo cliente (opcional)
    public string? Observation { get; set; }

    // ID do usuário (Cliente) que realizou o agendamento (Chave Estrangeira -> users.id)
    public int UserId { get; set; }

    // ID da empresa/estabelecimento com quem foi agendado (Chave Estrangeira -> companies.id)
    public int CompanyId { get; set; }

    // Data de criação do registro no banco de dados (coluna creat_at)
    public DateTime CreatedAt { get; set; }

    // Data da última atualização do registro
    public DateTime UpdatedAt { get; set; }

    public const string tabela = "schedulles";

    public Schedulle() { }

    public Schedulle(int id, DateTime datetimez, string type, int people, string? observation, int userId, int companyId)
    {
        Id = id;
        Datetimez = datetimez;
        Type = type;
        People = people;
        Observation = observation;
        UserId = userId;
        CompanyId = companyId;
    }

    /// <summary>
    /// Exibe no console os detalhes deste agendamento.
    /// </summary>
    public void Mostrar()
    {
        Console.WriteLine($"--------------------------------------------------");
        Console.WriteLine($"ID: {Id} | Data/Hora: {Datetimez:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Tipo: {Type} | Pessoas: {People}");
        Console.WriteLine($"Obs: {Observation ?? "Sem observações"}");
        Console.WriteLine($"Usuário ID: {UserId} | Empresa ID: {CompanyId}");
        Console.WriteLine($"Criado em: {CreatedAt:dd/MM/yyyy HH:mm}");
    }

    /// <summary>
    /// Exibe uma lista de agendamentos formatada.
    /// </summary>
    public void Mostrar(List<Schedulle> agendamentos)
    {
        if (agendamentos.Count == 0)
        {
            Console.WriteLine("Nenhum agendamento encontrado.");
            return;
        }

        foreach (var item in agendamentos)
        {
            item.Mostrar();
        }
    }

    /// <summary>
    /// Inserir este agendamento no banco de dados.
    /// Operação Active Record: CREATE (Inserir)
    /// </summary>
    public async Task<int> InserirAsync()
    {
        string query = $"""
            INSERT INTO {tabela} 
            (datetimez, type, people, observation, user_id, company_id, creat_at, updated_at) 
            VALUES 
            (@datetimez, @type, @people, @observation, @userId, @companyId, NOW(), NOW());
            SELECT LAST_INSERT_ID();
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        comando.Parameters.AddWithValue("@datetimez", Datetimez);
        comando.Parameters.AddWithValue("@type", Type);
        comando.Parameters.AddWithValue("@people", People);
        comando.Parameters.AddWithValue("@observation", (object?)Observation ?? DBNull.Value);
        comando.Parameters.AddWithValue("@userId", UserId);
        comando.Parameters.AddWithValue("@companyId", CompanyId);

        await conexao.OpenAsync();
        var result = await comando.ExecuteScalarAsync();
        this.Id = Convert.ToInt32(result);
        return this.Id;
    }

    /// <summary>
    /// Inserir síncrono.
    /// </summary>
    public int Inserir()
    {
        return InserirAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Carrega os dados de um agendamento do banco de dados neste próprio objeto (por ID).
    /// Operação Active Record: READ (Ler por ID na instância)
    /// </summary>
    public async Task<bool> BuscaAsync(int id)
    {
        string query = $"""
            SELECT id, datetimez, type, people, observation, user_id, company_id, creat_at, updated_at 
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
    /// Busca um agendamento por ID e retorna uma nova instância de Schedulle.
    /// Operação Active Record: READ (Ler por ID)
    /// </summary>
    public static async Task<Schedulle?> BuscarPorIdAsync(int id)
    {
        Schedulle item = new Schedulle();
        bool encontrado = await item.BuscaAsync(id);
        return encontrado ? item : null;
    }

    public static Schedulle? BuscarPorId(int id)
    {
        return BuscarPorIdAsync(id).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Listar todos os agendamentos do banco de dados.
    /// Operação Active Record: READ (Ler Todos)
    /// </summary>
    public async Task<List<Schedulle>> BuscarTodosAsync()
    {
        string query = $"""
            SELECT id, datetimez, type, people, observation, user_id, company_id, creat_at, updated_at 
            FROM {tabela};
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        await conexao.OpenAsync();
        await using var reader = await comando.ExecuteReaderAsync();

        List<Schedulle> lista = new();
        while (await reader.ReadAsync())
        {
            Schedulle item = new Schedulle();
            Mapear(reader, item);
            lista.Add(item);
        }

        return lista;
    }

    public List<Schedulle> ListarTodos()
    {
        return BuscarTodosAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Buscar todos os agendamentos de um determinado usuário.
    /// </summary>
    public async Task<List<Schedulle>> BuscarPorUsuarioAsync(int userId)
    {
        string query = $"""
            SELECT id, datetimez, type, people, observation, user_id, company_id, creat_at, updated_at 
            FROM {tabela} 
            WHERE user_id = @userId;
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);
        comando.Parameters.AddWithValue("@userId", userId);

        await conexao.OpenAsync();
        await using var reader = await comando.ExecuteReaderAsync();

        List<Schedulle> lista = new();
        while (await reader.ReadAsync())
        {
            Schedulle item = new Schedulle();
            Mapear(reader, item);
            lista.Add(item);
        }

        return lista;
    }

    public List<Schedulle> BuscarPorUsuario(int userId)
    {
        return BuscarPorUsuarioAsync(userId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Atualiza as informações deste agendamento no banco de dados.
    /// Operação Active Record: UPDATE (Alterar)
    /// REGRA DE NEGÓCIO (contexto.md): Não deve ser possível alterar um agendamento após passar do horário agendado.
    /// </summary>
    public async Task<bool> AtualizarAsync()
    {
        // 1. Busca o agendamento atual cadastrado no banco para verificar a data agendada
        var agendamentoExistente = await BuscarPorIdAsync(this.Id);
        if (agendamentoExistente == null)
        {
            throw new InvalidOperationException($"Agendamento com ID {this.Id} não foi encontrado.");
        }

        // REGRA DE NEGÓCIO: Se a data agendada já passou da data/hora atual, impede a alteração
        if (agendamentoExistente.Datetimez < DateTime.Now)
        {
            throw new InvalidOperationException("Não é possível alterar um agendamento cujo horário agendado já passou.");
        }

        string query = $"""
            UPDATE {tabela} 
            SET datetimez = @datetimez, 
                type = @type, 
                people = @people, 
                observation = @observation, 
                user_id = @userId, 
                company_id = @companyId, 
                updated_at = NOW() 
            WHERE id = @id;
            """;

        using var conexao = new MySqlConnection(ConexaoBD.connectionString);
        using var comando = new MySqlCommand(query, conexao);

        comando.Parameters.AddWithValue("@id", Id);
        comando.Parameters.AddWithValue("@datetimez", Datetimez);
        comando.Parameters.AddWithValue("@type", Type);
        comando.Parameters.AddWithValue("@people", People);
        comando.Parameters.AddWithValue("@observation", (object?)Observation ?? DBNull.Value);
        comando.Parameters.AddWithValue("@userId", UserId);
        comando.Parameters.AddWithValue("@companyId", CompanyId);

        await conexao.OpenAsync();
        int linhasAfetadas = await comando.ExecuteNonQueryAsync();
        return linhasAfetadas > 0;
    }

    public bool Atualizar()
    {
        return AtualizarAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Remove um agendamento pelo ID.
    /// Operação Active Record: DELETE (Remover)
    /// REGRA DE NEGÓCIO (contexto.md): Não deve ser possível remover um agendamento após passar do horário agendado.
    /// </summary>
    public async Task<bool> RemoverAsync(int id)
    {
        var agendamentoExistente = await BuscarPorIdAsync(id);
        if (agendamentoExistente == null)
        {
            return false;
        }

        // REGRA DE NEGÓCIO: Impede remoção caso o horário agendado já tenha passado
        if (agendamentoExistente.Datetimez < DateTime.Now)
        {
            throw new InvalidOperationException("Não é possível remover um agendamento cujo horário agendado já passou.");
        }

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

    private static void Mapear(MySqlDataReader reader, Schedulle item)
    {
        item.Id = reader.GetInt32("id");
        item.Datetimez = reader.GetDateTime("datetimez");
        item.Type = reader.GetString("type");
        item.People = reader.GetInt32("people");
        item.Observation = reader.IsDBNull(reader.GetOrdinal("observation")) ? null : reader.GetString("observation");
        item.UserId = reader.GetInt32("user_id");
        item.CompanyId = reader.GetInt32("company_id");
        item.CreatedAt = reader.GetDateTime("creat_at");
        item.UpdatedAt = reader.GetDateTime("updated_at");
    }
}
