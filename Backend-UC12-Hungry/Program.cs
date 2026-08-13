using MySqlConnector;

using var connection = new MySqlConnection(ConfiguracaoBD.connectionString);

connection.Open();



using var command = new MySqlCommand("SELECT * FROM testes;", connection);

using var reader = command.ExecuteReader();

while (reader.Read())

    Console.WriteLine(reader.GetString(1));
