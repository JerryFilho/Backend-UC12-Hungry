using MySqlConnector;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Cpf { get; set; } = string.Empty;

    // public List<Schedulle> Schedulles { get; set; } = new();
    // public List<Assessment> Assessments { get; set; } = new();
    // public List<Company> Companies { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
    // public List<Photo> Photos { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public const string tabela = "users";

    public User() { }

    public User(
        int id,
        string name,
        string type,
        string email,
        string password,
        DateTime birthDate,
        string cpf,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Name = name;
        Type = type;
        Email = email;
        Password = password;
        BirthDate = birthDate;
        Cpf = cpf;

        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public async Task InserirAsync()
    {
        using var connection = new MySqlConnection(ConexaoBD.connectionString);
        await connection.OpenAsync();

        string sql = $@"
            INSERT INTO {tabela}
            (
                name,
                type,
                email,
                password,
                birth_date,
                cpf,
                createdAt,
                updatedAt
            )
            VALUES
            (
                @name,
                @type,
                @email,
                @password,
                @birthDate,
                @cpf,
                @createdAt,
                @updatedAt
            )";

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@name", Name);
        command.Parameters.AddWithValue("@type", Type);
        command.Parameters.AddWithValue("@email", Email);
        command.Parameters.AddWithValue("@password", Password);
        command.Parameters.AddWithValue("@birthDate", BirthDate);
        command.Parameters.AddWithValue("@cpf", Cpf);
        command.Parameters.AddWithValue("@createdAt", CreatedAt);
        command.Parameters.AddWithValue("@updatedAt", UpdatedAt);

        await command.ExecuteNonQueryAsync();
    }

    public async Task BuscarAsync(int id)
    {
        using var connection = new MySqlConnection(ConexaoBD.connectionString);
        await connection.OpenAsync();

        string sql = $@"
            SELECT
                id,
                name,
                type,
                email,
                password,
                birth_date,
                cpf,
                createdAt,
                updatedAt
            FROM {tabela}
            WHERE id = @id";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            this.Id = reader.GetInt32("id");
            this.Name = reader.GetString("name");
            this.Type = reader.GetString("type");
            this.Email = reader.GetString("email");
            this.Password = reader.GetString("password");
            this.BirthDate = reader.GetDateTime("birth_date");
            this.Cpf = reader.GetString("cpf");
            this.CreatedAt = reader.GetDateTime("createdAt");
            this.UpdatedAt = reader.GetDateTime("updatedAt");
        }
        else
        {
            this.Id = 0; // indicates not found
        }
    }

    public async Task AtualizarAsync()
    {
        using var connection = new MySqlConnection(ConexaoBD.connectionString);
        await connection.OpenAsync();

        string sql = $@"
            UPDATE {tabela}
            SET
                name = @name,
                type = @type,
                email = @email,
                password = @password,
                birth_date = @birthDate,
                cpf = @cpf,
                updatedAt = @updatedAt
            WHERE id = @id";

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", Id);
        command.Parameters.AddWithValue("@name", Name);
        command.Parameters.AddWithValue("@type", Type);
        command.Parameters.AddWithValue("@email", Email);
        command.Parameters.AddWithValue("@password", Password);
        command.Parameters.AddWithValue("@birthDate", BirthDate);
        command.Parameters.AddWithValue("@cpf", Cpf);
        command.Parameters.AddWithValue("@updatedAt", DateTime.Now);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> RemoverAsync(int id)
    {
        using var connection = new MySqlConnection(ConexaoBD.connectionString);
        await connection.OpenAsync();

        string sqlVerificar = $@"
            SELECT type
            FROM {tabela}
            WHERE id = @id";

        using var verificarCommand = new MySqlCommand(sqlVerificar, connection);
        verificarCommand.Parameters.AddWithValue("@id", id);

        object? resultado = await verificarCommand.ExecuteScalarAsync();

        if (resultado == null)
            return false;

        string typeStr = resultado.ToString()!;

        if (typeStr == "ADMIN")
            return false;

        string sql = $@"
            DELETE FROM {tabela}
            WHERE id = @id";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        int linhasAfetadas = await command.ExecuteNonQueryAsync();
        return linhasAfetadas > 0;
    }

    public async Task<List<User>> BuscarTodosAsync()
    {
        var users = new List<User>();
        using var connection = new MySqlConnection(ConexaoBD.connectionString);
        await connection.OpenAsync();

        string sql = $@"
            SELECT
                id,
                name,
                type,
                email,
                password,
                birth_date,
                cpf,
                createdAt,
                updatedAt
            FROM {tabela}";

        using var command = new MySqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new User(
                reader.GetInt32("id"),
                reader.GetString("name"),
                reader.GetString("type"),
                reader.GetString("email"),
                reader.GetString("password"),
                reader.GetDateTime("birth_date"),
                reader.GetString("cpf"),
                reader.GetDateTime("createdAt"),
                reader.GetDateTime("updatedAt")
            ));
        }

        return users;
    }
}