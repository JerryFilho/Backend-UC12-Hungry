using System;
using System.Globalization;

public class Program
{

    public static async Task Main(string[] args)
    {
        Console.Title = "Hungry - Sistema";

        while (true)
        {
            Console.Clear();

            Console.WriteLine("====================================");
            Console.WriteLine("           HUNGRY SYSTEM            ");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1 - Usuários");
            Console.WriteLine("2 - Pagamentos");
            Console.WriteLine("0 - Sair");
            Console.WriteLine();
            Console.Write("Escolha uma opção: ");

            string? opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    await MenuUsuario();
                    break;

                case "2":
                    await MenuPagamento();
                    break;

                case "0":
                    Console.WriteLine("\nSaindo do sistema...");
                    return;

                default:
                    Console.WriteLine("\nOpção inválida.");
                    Pausar();
                    break;
            }
        }
    }

    // =========================
    // MENU DE USUÁRIO
    // =========================

    static async Task MenuUsuario()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("====================================");
            Console.WriteLine("          GERENCIAR USUÁRIOS        ");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1 - Inserir usuário");
            Console.WriteLine("2 - Buscar usuário");
            Console.WriteLine("3 - Atualizar usuário");
            Console.WriteLine("4 - Remover usuário");
            Console.WriteLine("5 - Listar todos os usuários");
            Console.WriteLine("0 - Voltar");
            Console.WriteLine();
            Console.Write("Escolha uma opção: ");

            string? opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    await InserirUsuario();
                    break;

                case "2":
                    await BuscarUsuario();
                    break;

                case "3":
                    await AtualizarUsuario();
                    break;

                case "4":
                    await RemoverUsuario();
                    break;

                case "5":
                    await ListarTodosUsuarios();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("\nOpção inválida.");
                    Pausar();
                    break;
            }
        }
    }

    // =========================
    // INSERIR USUÁRIO
    // =========================

    static async Task InserirUsuario()
    {
        Console.Clear();

        Console.WriteLine("=========== INSERIR USUÁRIO ===========");
        Console.WriteLine();

        Console.Write("Nome: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Tipo (CLIENT / ADMIN / OWNER): ");
        string type = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("Senha: ");
        string password = Console.ReadLine() ?? "";

        Console.Write("Data de nascimento (dd/MM/yyyy): ");
        DateTime birthDate = LerData();

        Console.Write("CPF: ");
        string cpf = Console.ReadLine() ?? "";

        DateTime now = DateTime.Now;

        User user = new User(
            0,
            name,
            type,
            email,
            password,
            birthDate,
            cpf,
            now,
            now
        );

        try
        {
            await user.InserirAsync();

            Console.WriteLine();
            Console.WriteLine("Usuário inserido com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Erro ao inserir usuário:");
            Console.WriteLine(ex.Message);
        }

        Pausar();
    }

    // =========================
    // BUSCAR USUÁRIO
    // =========================

    static async Task BuscarUsuario()
    {
        Console.Clear();

        Console.WriteLine("=========== BUSCAR USUÁRIO ===========");
        Console.WriteLine();

        Console.Write("ID do usuário: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nID inválido.");
            Pausar();
            return;
        }

        try
        {
            User user = new User();
            await user.BuscarAsync(id);

            if (user.Id == 0)
            {
                Console.WriteLine("\nUsuário não encontrado.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Usuário encontrado:");
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"ID:              {user.Id}");
                Console.WriteLine($"Nome:            {user.Name}");
                Console.WriteLine($"Tipo:            {user.Type}");
                Console.WriteLine($"Email:           {user.Email}");
                Console.WriteLine($"Senha:           {user.Password}");
                Console.WriteLine($"Nascimento:      {user.BirthDate:dd/MM/yyyy}");
                Console.WriteLine($"CPF:             {user.Cpf}");
                Console.WriteLine($"Criado em:       {user.CreatedAt}");
                Console.WriteLine($"Atualizado em:   {user.UpdatedAt}");
                Console.WriteLine("------------------------------------");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Erro ao buscar usuário:");

            Console.WriteLine(ex.Message);
        }

        Pausar();
    }

    // =========================
    // ATUALIZAR USUÁRIO
    // =========================

    static async Task AtualizarUsuario()
    {
        Console.Clear();

        Console.WriteLine("========== ATUALIZAR USUÁRIO ==========");
        Console.WriteLine();

        Console.Write("ID do usuário: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nID inválido.");
            Pausar();
            return;
        }

        try
        {
            User usuarioAtual = new User();
            await usuarioAtual.BuscarAsync(id);

            if (usuarioAtual.Id == 0)
            {
                Console.WriteLine("\nUsuário não encontrado.");
                Pausar();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Deixe vazio para manter o valor atual.");
            Console.WriteLine();

            Console.Write($"Nome ({usuarioAtual.Name}): ");
            string name = Console.ReadLine() ?? "";

            Console.Write($"Tipo ({usuarioAtual.Type}): ");
            string type = Console.ReadLine() ?? "";

            Console.Write($"Email ({usuarioAtual.Email}): ");
            string email = Console.ReadLine() ?? "";

            Console.Write("Nova senha: ");
            string password = Console.ReadLine() ?? "";

            Console.Write($"Data de nascimento ({usuarioAtual.BirthDate:dd/MM/yyyy}): ");
            string birthDateTexto = Console.ReadLine() ?? "";

            Console.Write($"CPF ({usuarioAtual.Cpf}): ");
            string cpf = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(name))
                name = usuarioAtual.Name;

            if (string.IsNullOrWhiteSpace(type))
                type = usuarioAtual.Type;

            if (string.IsNullOrWhiteSpace(email))
                email = usuarioAtual.Email;

            if (string.IsNullOrWhiteSpace(password))
                password = usuarioAtual.Password;

            if (string.IsNullOrWhiteSpace(cpf))
                cpf = usuarioAtual.Cpf;

            DateTime birthDate = usuarioAtual.BirthDate;

            if (!string.IsNullOrWhiteSpace(birthDateTexto))
            {
                if (!DateTime.TryParseExact(
                    birthDateTexto,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out birthDate))
                {
                    Console.WriteLine("\nData inválida.");
                    Pausar();
                    return;
                }
            }

            User user = new User(
                usuarioAtual.Id,
                name,
                type,
                email,
                password,
                birthDate,
                cpf,
                usuarioAtual.CreatedAt,
                DateTime.Now
            );

            await user.AtualizarAsync();

            Console.WriteLine();
            Console.WriteLine("Usuário atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Erro ao atualizar usuário:");
            Console.WriteLine(ex.Message);
        }

        Pausar();
    }

    // =========================
    // REMOVER USUÁRIO
    // =========================

    static async Task RemoverUsuario()
    {
        Console.Clear();

        Console.WriteLine("=========== REMOVER USUÁRIO ===========");
        Console.WriteLine();

        Console.Write("ID do usuário: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nID inválido.");
            Pausar();
            return;
        }

        try
        {
            User user = new User();
            await user.BuscarAsync(id);

            if (user.Id == 0)
            {
                Console.WriteLine("\nUsuário não encontrado.");
                Pausar();
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"Usuário: {user.Name}");
            Console.WriteLine($"Tipo: {user.Type}");
            Console.WriteLine();

            if (user.Type == "ADMIN")
            {
                Console.WriteLine("ADMINISTRADORES NÃO PODEM SER REMOVIDOS.");
                Pausar();
                return;
            }

            Console.Write("Confirma a remoção? (S/N): ");
            string confirmacao = Console.ReadLine() ?? "";

            if (confirmacao.ToUpper() != "S")
            {
                Console.WriteLine("\nOperação cancelada.");
                Pausar();
                return;
            }

            bool removido = await user.RemoverAsync(id);

            Console.WriteLine();

            if (removido)
            {
                Console.WriteLine("Usuário removido com sucesso.");
                Console.WriteLine("Os dados relacionados serão tratados pela cascata do banco.");
            }
            else
            {
                Console.WriteLine("Não foi possível remover o usuário.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Erro ao remover usuário:");
            Console.WriteLine(ex.Message);
        }

        Pausar();
    }

    // =========================
    // LISTAR TODOS OS USUÁRIOS
    // =========================

    static async Task ListarTodosUsuarios()
    {
        Console.Clear();

        Console.WriteLine("======= LISTAR TODOS OS USUÁRIOS =======");
        Console.WriteLine();

        try
        {
            User userInst = new User();
            var users = await userInst.BuscarTodosAsync();

            if (users.Count == 0)
            {
                Console.WriteLine("Nenhum usuário encontrado.");
            }
            else
            {
                Console.WriteLine(String.Format("{0,-5} | {1,-20} | {2,-10} | {3,-25} | {4,-15}", "ID", "Nome", "Tipo", "Email", "CPF"));
                Console.WriteLine(new string('-', 85));

                foreach (var user in users)
                {
                    string name = user.Name.Length > 20 ? user.Name.Substring(0, 17) + "..." : user.Name;
                    string email = user.Email.Length > 25 ? user.Email.Substring(0, 22) + "..." : user.Email;
                    Console.WriteLine(String.Format("{0,-5} | {1,-20} | {2,-10} | {3,-25} | {4,-15}", 
                        user.Id, name, user.Type, email, user.Cpf));
                }
                Console.WriteLine(new string('-', 85));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Erro ao listar usuários:");
            Console.WriteLine(ex.Message);
        }

        Pausar();
    }

    // =========================
    // MENU DE PAGAMENTO
    // =========================

    static async Task MenuPagamento()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("====================================");
            Console.WriteLine("         GERENCIAR PAGAMENTOS       ");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1 - Inserir pagamento");
            Console.WriteLine("2 - Buscar pagamento");
            Console.WriteLine("3 - Listar todos os pagamentos");
            Console.WriteLine("0 - Voltar");
            Console.WriteLine();
            Console.Write("Escolha uma opção: ");

            string? opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    await InserirPagamento();
                    break;

                case "2":
                    await BuscarPagamento();
                    break;

                case "3":
                    await ListarTodosPagamentos();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("\nOpção inválida.");
                    Pausar();
                    break;
            }
        }
    }

    // =========================
    // INSERIR PAGAMENTO
    // =========================

    static async Task InserirPagamento()
    {
        Console.Clear();

        Console.WriteLine("========= INSERIR PAGAMENTO =========");
        Console.WriteLine();

        Console.Write("Valor: ");

        if (!float.TryParse(
            Console.ReadLine(),
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out float value))
        {
            Console.WriteLine("\nValor inválido.");
            Pausar();
            return;
        }

        Console.Write("Data de vencimento (dd/MM/yyyy): ");
        DateTime dueDate = LerData();

        Console.Write("Data final (dd/MM/yyyy): ");
        DateTime toDate = LerData();

        Console.Write("ID da empresa: ");

        if (!int.TryParse(Console.ReadLine(), out int companyId))
        {
            Console.WriteLine("\nID da empresa inválido.");
            Pausar();
            return;
        }

        Console.Write("ID do usuário (deixe vazio se não houver): ");
        string userIdTexto = Console.ReadLine() ?? "";

        int? userId = null;

        if (!string.IsNullOrWhiteSpace(userIdTexto))
        {
            if (!int.TryParse(userIdTexto, out int idUsuario))
            {
                Console.WriteLine("\nID do usuário inválido.");
                Pausar();
                return;
            }

            userId = idUsuario;
        }

        DateTime now = DateTime.Now;

        Payment payment = new Payment(
            0,
            value,
            dueDate,
            toDate,
            companyId,
            userId,
            now,
            now
        );

        try
        {
            await payment.InserirAsync();

            Console.WriteLine();
            Console.WriteLine("Pagamento inserido com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Erro ao inserir pagamento:");
            Console.WriteLine(ex.Message);
        }

        Pausar();
    }

    // =========================
    // BUSCAR PAGAMENTO
    // =========================

    static async Task BuscarPagamento()
    {
        Console.Clear();

        Console.WriteLine("========= BUSCAR PAGAMENTO =========");
        Console.WriteLine();

        Console.Write("ID do pagamento: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nID inválido.");
            Pausar();
            return;
        }

        try
        {
            Payment payment = new Payment();
            await payment.BuscarAsync(id);

            if (payment.Id == 0)
            {
                Console.WriteLine("\nPagamento não encontrado.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Pagamento encontrado:");
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"ID:             {payment.Id}");
                Console.WriteLine($"Valor:          R$ {payment.Value:F2}");
                Console.WriteLine($"Vencimento:     {payment.DueDate:dd/MM/yyyy}");
                Console.WriteLine($"Data final:     {payment.ToDate:dd/MM/yyyy}");
                Console.WriteLine($"Company ID:     {payment.CompanyId}");
                Console.WriteLine($"User ID:        {payment.UserId}");
                Console.WriteLine($"Criado em:      {payment.CreatedAt}");
                Console.WriteLine($"Atualizado em:  {payment.UpdatedAt}");
                Console.WriteLine("------------------------------------");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Erro ao buscar pagamento:");
            Console.WriteLine(ex.Message);
        }

        Pausar();
    }

    // =========================
    // LISTAR TODOS OS PAGAMENTOS
    // =========================

    static async Task ListarTodosPagamentos()
    {
        Console.Clear();

        Console.WriteLine("====== LISTAR TODOS OS PAGAMENTOS ======");
        Console.WriteLine();

        try
        {
            Payment payInst = new Payment();
            var payments = await payInst.BuscarTodosAsync();

            if (payments.Count == 0)
            {
                Console.WriteLine("Nenhum pagamento encontrado.");
            }
            else
            {
                Console.WriteLine(String.Format("{0,-5} | {1,-12} | {2,-12} | {3,-12} | {4,-10} | {5,-10}", "ID", "Valor (R$)", "Vencimento", "Data Final", "Company ID", "User ID"));
                Console.WriteLine(new string('-', 75));

                foreach (var payment in payments)
                {
                    Console.WriteLine(String.Format("{0,-5} | {1,-12:F2} | {2,-12:dd/MM/yyyy} | {3,-12:dd/MM/yyyy} | {4,-10} | {5,-10}", 
                        payment.Id, 
                        payment.Value, 
                        payment.DueDate, 
                        payment.ToDate, 
                        payment.CompanyId, 
                        payment.UserId.HasValue ? payment.UserId.Value.ToString() : "-"));
                }
                Console.WriteLine(new string('-', 75));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Erro ao listar pagamentos:");
            Console.WriteLine(ex.Message);
        }

        Pausar();
    }

    // =========================
    // FUNÇÕES AUXILIARES
    // =========================

    static DateTime LerData()
    {
        while (true)
        {
            string? entrada = Console.ReadLine();

            if (DateTime.TryParseExact(
                entrada,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime data))
            {
                return data;
            }

            Console.Write("Data inválida. Digite novamente (dd/MM/yyyy): ");
        }
    }

    static void Pausar()
    {
        Console.WriteLine();
        Console.WriteLine("Pressione ENTER para continuar...");
        Console.ReadLine();
    }
}