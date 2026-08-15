using MySqlConnector;

public class Payment
{
    public int Id { get; set; }
    public float Value { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ToDate { get; set; }

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

    public const string tabela = "payment";

    public Payment() { }

    public Payment(
        int id,
        float value,
        DateTime dueDate,
        DateTime toDate,
        Company company,
        User? user,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Value = value;
        DueDate = dueDate;
        ToDate = toDate;
        this.company = company;
        this.user = user;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

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
        this.company = new Company { Id = companyId };
        if (userId.HasValue)
        {
            this.user = new User { Id = userId.Value };
        }
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
        command.Parameters.AddWithValue("@companyId", company.Id);
        command.Parameters.AddWithValue("@userId", user != null ? user.Id : DBNull.Value);
        command.Parameters.AddWithValue("@createdAt", CreatedAt);
        command.Parameters.AddWithValue("@updatedAt", UpdatedAt);

        await command.ExecuteNonQueryAsync();
    }

    public async Task BuscarAsync(int id)
    {
        using var connection = new MySqlConnection(ConexaoBD.connectionString);
        await connection.OpenAsync();

        string sql = $@"
            SELECT p.*, 
                   c.name AS comp_name, c.category AS comp_cat, c.cnpj AS comp_cnpj, 
                   c.places AS comp_places, c.phone AS comp_phone, c.fundation AS comp_fund, 
                   c.description AS comp_desc, c.created_at AS comp_created, c.updated_at AS comp_updated,
                   u.name AS user_name, u.type AS user_type, u.email AS user_email, 
                   u.password AS user_password, u.birth_date AS user_birth, u.cpf AS user_cpf, 
                   u.createdAt AS user_created, u.updatedAt AS user_updated
            FROM {tabela} p
            INNER JOIN companies c ON p.companyId = c.id
            LEFT JOIN users u ON p.user_id = u.id
            WHERE p.id = @id";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            this.Id = reader.GetInt32("id");
            this.Value = reader.GetFloat("value");
            this.DueDate = reader.GetDateTime("due_date");
            this.ToDate = reader.GetDateTime("to_date");
            
            if (!reader.IsDBNull(reader.GetOrdinal("user_id")))
            {
                this.user = new User(
                    reader.GetInt32("user_id"),
                    reader.GetString("user_name"),
                    reader.GetString("user_type"),
                    reader.GetString("user_email"),
                    reader.GetString("user_password"),
                    reader.GetDateTime("user_birth"),
                    reader.GetString("user_cpf"),
                    reader.GetDateTime("user_created"),
                    reader.GetDateTime("user_updated")
                );
            }
            else
            {
                this.user = null;
            }

            this.company = new Company(
                reader.GetInt32("companyId"),
                reader.GetString("comp_name"),
                reader.GetString("comp_cat"),
                reader.GetString("comp_cnpj"),
                reader.GetString("comp_places"),
                reader.GetString("comp_phone"),
                reader.GetDateTime("comp_fund"),
                reader.GetString("comp_desc"),
                this.user, // reference user
                reader.GetDateTime("comp_created"),
                reader.GetDateTime("comp_updated")
            );

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
            SELECT p.*, 
                   c.name AS comp_name, c.category AS comp_cat, c.cnpj AS comp_cnpj, 
                   c.places AS comp_places, c.phone AS comp_phone, c.fundation AS comp_fund, 
                   c.description AS comp_desc, c.created_at AS comp_created, c.updated_at AS comp_updated,
                   u.name AS user_name, u.type AS user_type, u.email AS user_email, 
                   u.password AS user_password, u.birth_date AS user_birth, u.cpf AS user_cpf, 
                   u.createdAt AS user_created, u.updatedAt AS user_updated
            FROM {tabela} p
            INNER JOIN companies c ON p.companyId = c.id
            LEFT JOIN users u ON p.user_id = u.id";

        using var command = new MySqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            User? relatedUser = null;
            if (!reader.IsDBNull(reader.GetOrdinal("user_id")))
            {
                relatedUser = new User(
                    reader.GetInt32("user_id"),
                    reader.GetString("user_name"),
                    reader.GetString("user_type"),
                    reader.GetString("user_email"),
                    reader.GetString("user_password"),
                    reader.GetDateTime("user_birth"),
                    reader.GetString("user_cpf"),
                    reader.GetDateTime("user_created"),
                    reader.GetDateTime("user_updated")
                );
            }

            Company relatedCompany = new Company(
                reader.GetInt32("companyId"),
                reader.GetString("comp_name"),
                reader.GetString("comp_cat"),
                reader.GetString("comp_cnpj"),
                reader.GetString("comp_places"),
                reader.GetString("comp_phone"),
                reader.GetDateTime("comp_fund"),
                reader.GetString("comp_desc"),
                relatedUser,
                reader.GetDateTime("comp_created"),
                reader.GetDateTime("comp_updated")
            );

            payments.Add(new Payment(
                reader.GetInt32("id"),
                reader.GetFloat("value"),
                reader.GetDateTime("due_date"),
                reader.GetDateTime("to_date"),
                relatedCompany,
                relatedUser,
                reader.GetDateTime("createdAt"),
                reader.GetDateTime("updatedAt")
            ));
        }

        return payments;
    }
}