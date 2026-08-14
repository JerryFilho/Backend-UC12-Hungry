using MySqlConnector;

public class Payment
{
    public int Id { get; set; }
    public float Value { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ToDate { get; set; }

    public int CompanyId { get; set; }
    // public Company Company { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public const string tabela = "payment";

    public Payment() { }

    public Payment(
        int id,
        float value,
        DateTime dueDate,
        DateTime toDate,
        int companyId,
        int? userId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Value = value;
        DueDate = dueDate;
        ToDate = toDate;
        CompanyId = companyId;
        UserId = userId;

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
                value,
                due_date,
                to_date,
                companyId,
                user_id,
                createdAt,
                updatedAt
            )
            VALUES
            (
                @value,
                @dueDate,
                @toDate,
                @companyId,
                @userId,
                @createdAt,
                @updatedAt
            )";

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@value", Value);
        command.Parameters.AddWithValue("@dueDate", DueDate);
        command.Parameters.AddWithValue("@toDate", ToDate);
        command.Parameters.AddWithValue("@companyId", CompanyId);

        if (UserId.HasValue)
        {
            command.Parameters.AddWithValue("@userId", UserId.Value);
        }
        else
        {
            command.Parameters.AddWithValue("@userId", DBNull.Value);
        }

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
                value,
                due_date,
                to_date,
                companyId,
                user_id,
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
            this.Value = reader.GetFloat("value");
            this.DueDate = reader.GetDateTime("due_date");
            this.ToDate = reader.GetDateTime("to_date");
            this.CompanyId = reader.GetInt32("companyId");
            
            this.UserId = reader.IsDBNull(reader.GetOrdinal("user_id")) 
                ? null 
                : reader.GetInt32("user_id");

            this.CreatedAt = reader.GetDateTime("createdAt");
            this.UpdatedAt = reader.GetDateTime("updatedAt");
        }
        else
        {
            this.Id = 0; // indicates not found
        }
    }

    public async Task<List<Payment>> BuscarTodosAsync()
    {
        var payments = new List<Payment>();
        using var connection = new MySqlConnection(ConexaoBD.connectionString);
        await connection.OpenAsync();

        string sql = $@"
            SELECT
                id,
                value,
                due_date,
                to_date,
                companyId,
                user_id,
                createdAt,
                updatedAt
            FROM {tabela}";

        using var command = new MySqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int? userId = reader.IsDBNull(reader.GetOrdinal("user_id"))
                ? null
                : reader.GetInt32("user_id");

            payments.Add(new Payment(
                reader.GetInt32("id"),
                reader.GetFloat("value"),
                reader.GetDateTime("due_date"),
                reader.GetDateTime("to_date"),
                reader.GetInt32("companyId"),
                userId,
                reader.GetDateTime("createdAt"),
                reader.GetDateTime("updatedAt")
            ));
        }

        return payments;
    }
}