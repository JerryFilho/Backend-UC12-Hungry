using Backend_UC12_Hungry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Instanciação de serviços Active Record
var userService = new User();
var companyService = new Company();
var schedulleService = new Schedulle();
var photoService = new Photo();
var assessmentService = new Assessment();
var paymentService = new Payment();

bool executando = true;

// Loop principal do menu interativo
while (executando)
{
    Console.Clear();
    Console.WriteLine("==================================================");
    Console.WriteLine("                SISTEMA HUNGRY                    ");
    Console.WriteLine("         Menu de Gerenciamento Geral              ");
    Console.WriteLine("==================================================");
    Console.WriteLine(" 1 - Gerenciar Usuários (User)");
    Console.WriteLine(" 2 - Gerenciar Empresas (Company)");
    Console.WriteLine(" 3 - Gerenciar Agendamentos (Schedulle)");
    Console.WriteLine(" 4 - Gerenciar Fotos (Photo)");
    Console.WriteLine(" 5 - Gerenciar Avaliações (Assessment)");
    Console.WriteLine(" 6 - Gerenciar Pagamentos (Payment)");
    Console.WriteLine(" 0 - Sair");
    Console.WriteLine("==================================================");
    Console.Write("Escolha uma opção: ");

    string? opcao = Console.ReadLine();

    switch (opcao)
    {
        case "1":
            await MenuUsuarios();
            break;
        case "2":
            await MenuEmpresas();
            break;
        case "3":
            MenuAgendamentos(schedulleService);
            break;
        case "4":
            await MenuFotos();
            break;
        case "5":
            MenuAvaliacoes(assessmentService, schedulleService);
            break;
        case "6":
            await MenuPagamentos();
            break;
        case "0":
            executando = false;
            Console.WriteLine("\nSaindo do sistema... Até logo!");
            break;
        default:
            PressionarParaContinuar("Opção inválida! Pressione ENTER para tentar novamente.");
            break;
    }
}

#region Menu de Usuários
async Task MenuUsuarios()
{
    bool noSubmenu = true;
    while (noSubmenu)
    {
        Console.Clear();
        Console.WriteLine("==================================================");
        Console.WriteLine("               GERENCIAR USUÁRIOS                 ");
        Console.WriteLine("==================================================");
        Console.WriteLine(" 1 - Cadastrar Usuário");
        Console.WriteLine(" 2 - Listar Todos os Usuários");
        Console.WriteLine(" 3 - Buscar Usuário por ID");
        Console.WriteLine(" 4 - Atualizar Usuário");
        Console.WriteLine(" 5 - Remover Usuário");
        Console.WriteLine(" 0 - Voltar ao Menu Principal");
        Console.WriteLine("==================================================");
        Console.Write("Escolha uma opção: ");

        string? subOpcao = Console.ReadLine();

        switch (subOpcao)
        {
            case "1":
                Console.Clear();
                Console.WriteLine("--- CADASTRAR USUÁRIO ---");
                Console.Write("Nome: ");
                string name = Console.ReadLine() ?? "";
                Console.Write("Tipo (CLIENT/ADMIN/OWNER): ");
                string type = Console.ReadLine() ?? "CLIENT";
                Console.Write("E-mail: ");
                string email = Console.ReadLine() ?? "";
                Console.Write("Senha: ");
                string password = Console.ReadLine() ?? "";
                Console.Write("Data de Nascimento (dd/MM/yyyy): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime birthDate)) birthDate = DateTime.Now;
                Console.Write("CPF: ");
                string cpf = Console.ReadLine() ?? "";

                var novoUsuario = new User
                {
                    Name = name,
                    Type = type,
                    Email = email,
                    Password = password,
                    BirthDate = birthDate,
                    Cpf = cpf,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await novoUsuario.InserirAsync();
                PressionarParaContinuar("Usuário inserido com sucesso!");
                break;

            case "2":
                Console.Clear();
                Console.WriteLine("--- LISTA DE USUÁRIOS ---");
                var lista = await userService.BuscarTodosAsync();
                foreach (var u in lista)
                {
                    Console.WriteLine($"ID: {u.Id} | Nome: {u.Name} | Tipo: {u.Type} | E-mail: {u.Email}");
                }
                PressionarParaContinuar();
                break;

            case "3":
                Console.Clear();
                Console.Write("Digite o ID do Usuário: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var u = new User();
                    await u.BuscarAsync(id);
                    if (u.Id > 0)
                    {
                        Console.WriteLine($"ID: {u.Id} | Nome: {u.Name} | Tipo: {u.Type} | E-mail: {u.Email} | CPF: {u.Cpf}");
                    }
                    else
                    {
                        Console.WriteLine("Usuário não encontrado.");
                    }
                }
                PressionarParaContinuar();
                break;

            case "4":
                Console.Clear();
                Console.Write("Digite o ID do Usuário a atualizar: ");
                if (int.TryParse(Console.ReadLine(), out int idAlt))
                {
                    var u = new User();
                    await u.BuscarAsync(idAlt);
                    if (u.Id > 0)
                    {
                        Console.Write($"Novo Nome ({u.Name}): ");
                        string newName = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(newName)) u.Name = newName;
                        
                        Console.Write($"Novo Tipo ({u.Type}): ");
                        string newType = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(newType)) u.Type = newType;

                        Console.Write($"Novo Email ({u.Email}): ");
                        string newEmail = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(newEmail)) u.Email = newEmail;

                        await u.AtualizarAsync();
                        PressionarParaContinuar("Usuário atualizado com sucesso!");
                    }
                    else
                    {
                        PressionarParaContinuar("Usuário não encontrado.");
                    }
                }
                break;

            case "5":
                Console.Clear();
                Console.Write("Digite o ID do Usuário a remover: ");
                if (int.TryParse(Console.ReadLine(), out int idRem))
                {
                    bool removido = await userService.RemoverAsync(idRem);
                    if (removido)
                        PressionarParaContinuar("Usuário removido com sucesso (admins não podem ser removidos).");
                    else
                        PressionarParaContinuar("Falha ao remover o usuário.");
                }
                break;

            case "0":
                noSubmenu = false;
                break;
        }
    }
}
#endregion

#region Menu de Empresas
async Task MenuEmpresas()
{
    bool noSubmenu = true;
    while (noSubmenu)
    {
        Console.Clear();
        Console.WriteLine("==================================================");
        Console.WriteLine("               GERENCIAR EMPRESAS                 ");
        Console.WriteLine("==================================================");
        Console.WriteLine(" 1 - Cadastrar Empresa");
        Console.WriteLine(" 2 - Listar Todas as Empresas");
        Console.WriteLine(" 3 - Buscar Empresa por ID");
        Console.WriteLine(" 4 - Atualizar Empresa");
        Console.WriteLine(" 5 - Remover Empresa");
        Console.WriteLine(" 0 - Voltar ao Menu Principal");
        Console.WriteLine("==================================================");
        Console.Write("Escolha uma opção: ");

        string? subOpcao = Console.ReadLine();

        switch (subOpcao)
        {
            case "1":
                Console.Clear();
                Console.WriteLine("--- CADASTRAR EMPRESA ---");
                Console.Write("Nome: ");
                string name = Console.ReadLine() ?? "";
                Console.Write("Categoria: ");
                string category = Console.ReadLine() ?? "";
                Console.Write("CNPJ: ");
                string cnpj = Console.ReadLine() ?? "";
                Console.Write("Lugares: ");
                string places = Console.ReadLine() ?? "";
                Console.Write("Telefone: ");
                string phone = Console.ReadLine() ?? "";
                Console.Write("ID do Usuário Dono (Owner): ");
                int.TryParse(Console.ReadLine(), out int userId);

                var novaEmpresa = new Company
                {
                    Name = name,
                    Category = category,
                    Cnpj = cnpj,
                    Places = places,
                    Phone = phone,
                    Fundation = DateTime.Now,
                    Description = "Nova empresa",
                    UserId = userId
                };

                await novaEmpresa.InserirAsync();
                PressionarParaContinuar("Empresa cadastrada com sucesso!");
                break;

            case "2":
                Console.Clear();
                var lista = await companyService.BuscarTodosAsync();
                companyService.Mostrar(lista);
                PressionarParaContinuar();
                break;

            case "3":
                Console.Clear();
                Console.Write("Digite o ID da Empresa: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var c = new Company();
                    await c.BuscarAsync(id);
                    if (c.Id > 0)
                    {
                        c.Mostrar();
                    }
                    else
                    {
                        Console.WriteLine("Empresa não encontrada.");
                    }
                }
                PressionarParaContinuar();
                break;

            case "4":
                Console.Clear();
                Console.Write("Digite o ID da Empresa a atualizar: ");
                if (int.TryParse(Console.ReadLine(), out int idAlt))
                {
                    var c = new Company();
                    await c.BuscarAsync(idAlt);
                    if (c.Id > 0)
                    {
                        Console.Write($"Novo Nome ({c.Name}): ");
                        string newName = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(newName)) c.Name = newName;

                        Console.Write($"Nova Categoria ({c.Category}): ");
                        string newCat = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(newCat)) c.Category = newCat;

                        await c.AtualizarAsync();
                        PressionarParaContinuar("Empresa atualizada com sucesso!");
                    }
                    else
                    {
                        PressionarParaContinuar("Empresa não encontrada.");
                    }
                }
                break;

            case "5":
                Console.Clear();
                Console.Write("Digite o ID da Empresa a remover: ");
                if (int.TryParse(Console.ReadLine(), out int idRem))
                {
                    await companyService.RemoverAsync(idRem);
                    PressionarParaContinuar("Empresa removida com sucesso.");
                }
                break;

            case "0":
                noSubmenu = false;
                break;
        }
    }
}
#endregion

#region Menu de Agendamentos

void MenuAgendamentos(Schedulle schedulleService)
{
    bool noSubmenu = true;
    while (noSubmenu)
    {
        Console.Clear();
        Console.WriteLine("==================================================");
        Console.WriteLine("             MENU DE AGENDAMENTOS (SCHEDULLE)     ");
        Console.WriteLine("==================================================");
        Console.WriteLine(" 1 - Cadastrar Agendamento (Inserir)");
        Console.WriteLine(" 2 - Listar Todos os Agendamentos (Ler)");
        Console.WriteLine(" 3 - Buscar Agendamento por ID (Ler)");
        Console.WriteLine(" 4 - Buscar Agendamentos por Usuário (Ler)");
        Console.WriteLine(" 5 - Atualizar Agendamento (Alterar - Valida Horário)");
        Console.WriteLine(" 6 - Remover Agendamento (Remover - Valida Horário)");
        Console.WriteLine(" 0 - Voltar ao Menu Principal");
        Console.WriteLine("==================================================");
        Console.Write("Escolha uma opção: ");

        string? subOpcao = Console.ReadLine();

        switch (subOpcao)
        {
            case "1":
                CadastrarAgendamento();
                break;
            case "2":
                ListarAgendamentos(schedulleService);
                break;
            case "3":
                BuscarAgendamentoPorId();
                break;
            case "4":
                BuscarAgendamentosPorUsuario(schedulleService);
                break;
            case "5":
                AtualizarAgendamento();
                break;
            case "6":
                RemoverAgendamento(schedulleService);
                break;
            case "0":
                noSubmenu = false;
                break;
            default:
                PressionarParaContinuar("Opção inválida!");
                break;
        }
    }
}

void CadastrarAgendamento()
{
    Console.Clear();
    Console.WriteLine("--- CADASTRAR AGENDAMENTO ---");

    Console.Write("Data e Hora (dd/MM/yyyy HH:mm) ex: 25/12/2026 19:30: ");
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime datetimez))
    {
        PressionarParaContinuar("Data/Hora no formato inválido!");
        return;
    }

    Console.Write("Tipo do agendamento (ex: Mesa VIP, Balcão, Reserva): ");
    string type = Console.ReadLine() ?? "Reserva";

    Console.Write("Quantidade de pessoas: ");
    if (!int.TryParse(Console.ReadLine(), out int people)) people = 1;

    Console.Write("Observação (opcional): ");
    string? observation = Console.ReadLine();

    Console.Write("ID do Usuário (Cliente): ");
    if (!int.TryParse(Console.ReadLine(), out int userId)) userId = 1;

    Console.Write("ID da Empresa (Empresa): ");
    if (!int.TryParse(Console.ReadLine(), out int companyId)) companyId = 1;

    var agendamento = new Schedulle
    {
        Datetimez = datetimez,
        Type = type,
        People = people,
        Observation = string.IsNullOrWhiteSpace(observation) ? null : observation,
        UserId = userId,
        CompanyId = companyId
    };

    try
    {
        int novoId = agendamento.Inserir();
        PressionarParaContinuar($"[SUCESSO] Agendamento cadastrado com sucesso! ID Gerado: {novoId}");
    }
    catch (Exception ex)
    {
        PressionarParaContinuar($"[ERRO AO INSERIR]: {ex.Message}");
    }
}

void ListarAgendamentos(Schedulle schedulleService)
{
    Console.Clear();
    Console.WriteLine("--- LISTA DE AGENDAMENTOS ---");
    var lista = schedulleService.ListarTodos();

    if (lista.Count == 0)
    {
        Console.WriteLine("Nenhum agendamento encontrado.");
    }
    else
    {
        schedulleService.Mostrar(lista);
    }
    PressionarParaContinuar();
}

void BuscarAgendamentoPorId()
{
    Console.Clear();
    Console.WriteLine("--- BUSCAR AGENDAMENTO POR ID ---");
    Console.Write("Digite o ID do Agendamento: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var item = Schedulle.BuscarPorId(id);
        if (item != null)
        {
            item.Mostrar();
        }
        else
        {
            Console.WriteLine($"Agendamento com ID {id} não foi encontrado.");
        }
    }
    else
    {
        Console.WriteLine("ID inválido!");
    }
    PressionarParaContinuar();
}

void BuscarAgendamentosPorUsuario(Schedulle schedulleService)
{
    Console.Clear();
    Console.WriteLine("--- BUSCAR AGENDAMENTOS POR USUÁRIO ---");
    Console.Write("Digite o ID do Usuário: ");
    if (int.TryParse(Console.ReadLine(), out int userId))
    {
        var lista = schedulleService.BuscarPorUsuario(userId);
        if (lista.Count == 0)
        {
            Console.WriteLine($"Nenhum agendamento encontrado para o usuário ID {userId}.");
        }
        else
        {
            schedulleService.Mostrar(lista);
        }
    }
    else
    {
        Console.WriteLine("ID de usuário inválido!");
    }
    PressionarParaContinuar();
}

void AtualizarAgendamento()
{
    Console.Clear();
    Console.WriteLine("--- ATUALIZAR AGENDAMENTO ---");
    Console.Write("Digite o ID do Agendamento que deseja atualizar: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        PressionarParaContinuar("ID inválido!");
        return;
    }

    var existente = Schedulle.BuscarPorId(id);
    if (existente == null)
    {
        PressionarParaContinuar($"Agendamento com ID {id} não encontrado.");
        return;
    }

    Console.WriteLine("\nDados atuais do agendamento:");
    existente.Mostrar();

    Console.WriteLine("Digite os novos dados (Pressione ENTER para manter o valor atual):");

    Console.Write($"Nova Data e Hora ({existente.Datetimez:dd/MM/yyyy HH:mm}): ");
    string? novaDataStr = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(novaDataStr) && DateTime.TryParse(novaDataStr, out DateTime novaData))
    {
        existente.Datetimez = novaData;
    }

    Console.Write($"Novo Tipo ({existente.Type}): ");
    string? novoTipo = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(novoTipo)) existente.Type = novoTipo;

    Console.Write($"Nova Qtd de Pessoas ({existente.People}): ");
    string? novaPessoasStr = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(novaPessoasStr) && int.TryParse(novaPessoasStr, out int novasPessoas))
    {
        existente.People = novasPessoas;
    }

    Console.Write($"Nova Observação ({existente.Observation}): ");
    string? novaObs = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(novaObs)) existente.Observation = novaObs;

    try
    {
        bool alterado = existente.Atualizar();
        if (alterado)
        {
            PressionarParaContinuar("[SUCESSO] Agendamento atualizado com sucesso!");
        }
        else
        {
            PressionarParaContinuar("[AVISO] Não foi possível atualizar o agendamento.");
        }
    }
    catch (Exception ex)
    {
        PressionarParaContinuar($"[ERRO/REGRA DE NEGÓCIO]: {ex.Message}");
    }
}

void RemoverAgendamento(Schedulle schedulleService)
{
    Console.Clear();
    Console.WriteLine("--- REMOVER AGENDAMENTO ---");
    Console.Write("Digite o ID do Agendamento que deseja remover: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        PressionarParaContinuar("ID inválido!");
        return;
    }

    try
    {
        bool removido = schedulleService.Remover(id);
        if (removido)
        {
            PressionarParaContinuar($"[SUCESSO] Agendamento ID {id} removido com sucesso!");
        }
        else
        {
            PressionarParaContinuar($"Agendamento ID {id} não encontrado.");
        }
    }
    catch (Exception ex)
    {
        PressionarParaContinuar($"[ERRO/REGRA DE NEGÓCIO]: {ex.Message}");
    }
}

#endregion

#region Menu de Fotos
async Task MenuFotos()
{
    bool noSubmenu = true;
    while (noSubmenu)
    {
        Console.Clear();
        Console.WriteLine("==================================================");
        Console.WriteLine("                 GERENCIAR FOTOS                  ");
        Console.WriteLine("==================================================");
        Console.WriteLine(" 1 - Cadastrar Foto");
        Console.WriteLine(" 2 - Listar Todas as Fotos");
        Console.WriteLine(" 3 - Buscar Foto por ID");
        Console.WriteLine(" 4 - Atualizar Foto");
        Console.WriteLine(" 5 - Remover Foto");
        Console.WriteLine(" 0 - Voltar ao Menu Principal");
        Console.WriteLine("==================================================");
        Console.Write("Escolha uma opção: ");

        string? subOpcao = Console.ReadLine();

        switch (subOpcao)
        {
            case "1":
                Console.Clear();
                Console.WriteLine("--- CADASTRAR FOTO ---");
                Console.Write("URL da Foto: ");
                string url = Console.ReadLine() ?? "";
                Console.Write("ID da Empresa (Company): ");
                int.TryParse(Console.ReadLine(), out int companyId);
                Console.Write("ID do Usuário (Opcional): ");
                int? userId = null;
                string? userStr = Console.ReadLine();
                if (int.TryParse(userStr, out int uId)) userId = uId;

                var novaFoto = new Photo(url, companyId, userId);
                await novaFoto.InserirAsync();
                PressionarParaContinuar("Foto cadastrada com sucesso!");
                break;

            case "2":
                Console.Clear();
                var lista = await photoService.BuscarTodosAsync();
                photoService.Mostrar(lista);
                PressionarParaContinuar();
                break;

            case "3":
                Console.Clear();
                Console.Write("Digite o ID da Foto: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var f = new Photo();
                    await f.BuscarAsync(id);
                    if (f.Id > 0)
                    {
                        f.Mostrar();
                    }
                    else
                    {
                        Console.WriteLine("Foto não encontrada.");
                    }
                }
                PressionarParaContinuar();
                break;

            case "4":
                Console.Clear();
                Console.Write("Digite o ID da Foto a atualizar: ");
                if (int.TryParse(Console.ReadLine(), out int idAlt))
                {
                    var f = new Photo();
                    await f.BuscarAsync(idAlt);
                    if (f.Id > 0)
                    {
                        Console.Write($"Nova URL ({f.Url}): ");
                        string newUrl = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(newUrl)) f.Url = newUrl;

                        await f.AtualizarAsync();
                        PressionarParaContinuar("Foto atualizada com sucesso!");
                    }
                    else
                    {
                        PressionarParaContinuar("Foto não encontrada.");
                    }
                }
                break;

            case "5":
                Console.Clear();
                Console.Write("Digite o ID da Foto a remover: ");
                if (int.TryParse(Console.ReadLine(), out int idRem))
                {
                    await photoService.RemoverAsync(idRem);
                    PressionarParaContinuar("Foto removida com sucesso.");
                }
                break;

            case "0":
                noSubmenu = false;
                break;
        }
    }
}
#endregion

#region Menu de Avaliações

void MenuAvaliacoes(Assessment assessmentService, Schedulle schedulleService)
{
    bool noSubmenu = true;
    while (noSubmenu)
    {
        Console.Clear();
        Console.WriteLine("==================================================");
        Console.WriteLine("             MENU DE AVALIAÇÕES (ASSESSMENT)      ");
        Console.WriteLine("==================================================");
        Console.WriteLine(" 1 - Cadastrar Avaliação (Inserir)");
        Console.WriteLine(" 2 - Listar Todas as Avaliações (Ler)");
        Console.WriteLine(" 3 - Buscar Avaliação por ID (Ler)");
        Console.WriteLine(" 4 - Buscar Avaliações por Agendamento (Ler)");
        Console.WriteLine(" 5 - Remover Avaliação (Remover)");
        Console.WriteLine(" 0 - Voltar ao Menu Principal");
        Console.WriteLine("==================================================");
        Console.Write("Escolha uma opção: ");

        string? subOpcao = Console.ReadLine();

        switch (subOpcao)
        {
            case "1":
                CadastrarAvaliacao();
                break;
            case "2":
                ListarAvaliacoes(assessmentService);
                break;
            case "3":
                BuscarAvaliacaoPorId();
                break;
            case "4":
                BuscarAvaliacoesPorAgendamento(assessmentService);
                break;
            case "5":
                RemoverAvaliacao(assessmentService);
                break;
            case "0":
                noSubmenu = false;
                break;
            default:
                PressionarParaContinuar("Opção inválida!");
                break;
        }
    }
}

void CadastrarAvaliacao()
{
    Console.Clear();
    Console.WriteLine("--- CADASTRAR AVALIAÇÃO ---");

    Console.Write("ID do Agendamento que está sendo avaliado: ");
    if (!int.TryParse(Console.ReadLine(), out int schedId))
    {
        PressionarParaContinuar("ID de agendamento inválido!");
        return;
    }

    var agendamento = Schedulle.BuscarPorId(schedId);
    if (agendamento == null)
    {
        PressionarParaContinuar($"[ERRO] Agendamento ID {schedId} não foi encontrado no banco.");
        return;
    }

    Console.Write("Nota (1 a 5): ");
    if (!int.TryParse(Console.ReadLine(), out int note) || note < 1 || note > 5)
    {
        PressionarParaContinuar("Nota inválida! Digite um valor entre 1 e 5.");
        return;
    }

    Console.Write("Comentário (opcional): ");
    string? comment = Console.ReadLine();

    var avaliacao = new Assessment
    {
        UserId = agendamento.UserId,
        SchedullesId = schedId,
        Note = note,
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment
    };

    try
    {
        int novoId = avaliacao.Inserir();
        PressionarParaContinuar($"[SUCESSO] Avaliação cadastrada com sucesso! ID Gerado: {novoId}");
    }
    catch (Exception ex)
    {
        PressionarParaContinuar($"[ERRO AO INSERIR]: {ex.Message}");
    }
}

void ListarAvaliacoes(Assessment assessmentService)
{
    Console.Clear();
    Console.WriteLine("--- LISTA DE AVALIAÇÕES ---");
    var lista = assessmentService.ListarTodos();

    if (lista.Count == 0)
    {
        Console.WriteLine("Nenhuma avaliação encontrada.");
    }
    else
    {
        assessmentService.Mostrar(lista);
    }
    PressionarParaContinuar();
}

void BuscarAvaliacaoPorId()
{
    Console.Clear();
    Console.WriteLine("--- BUSCAR AVALIAÇÃO POR ID ---");
    Console.Write("Digite o ID da Avaliação: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var item = Assessment.BuscarPorId(id);
        if (item != null)
        {
            item.Mostrar();
        }
        else
        {
            Console.WriteLine($"Avaliação com ID {id} não encontrada.");
        }
    }
    else
    {
        Console.WriteLine("ID inválido!");
    }
    PressionarParaContinuar();
}

void BuscarAvaliacoesPorAgendamento(Assessment assessmentService)
{
    Console.Clear();
    Console.WriteLine("--- BUSCAR AVALIAÇÕES POR AGENDAMENTO ---");
    Console.Write("Digite o ID do Agendamento: ");
    if (int.TryParse(Console.ReadLine(), out int schedId))
    {
        var lista = assessmentService.BuscarPorAgendamento(schedId);
        if (lista.Count == 0)
        {
            Console.WriteLine($"Nenhuma avaliação encontrada para o agendamento ID {schedId}.");
        }
        else
        {
            assessmentService.Mostrar(lista);
        }
    }
    else
    {
        Console.WriteLine("ID inválido!");
    }
    PressionarParaContinuar();
}

void RemoverAvaliacao(Assessment assessmentService)
{
    Console.Clear();
    Console.WriteLine("--- REMOVER AVALIAÇÃO ---");
    Console.Write("Digite o ID da Avaliação que deseja remover: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        PressionarParaContinuar("ID inválido!");
        return;
    }

    try
    {
        bool removido = assessmentService.Remover(id);
        if (removido)
        {
            PressionarParaContinuar($"[SUCESSO] Avaliação ID {id} removida com sucesso!");
        }
        else
        {
            PressionarParaContinuar($"Avaliação ID {id} não encontrada.");
        }
    }
    catch (Exception ex)
    {
        PressionarParaContinuar($"[ERRO AO REMOVER]: {ex.Message}");
    }
}

#endregion

#region Menu de Pagamentos
async Task MenuPagamentos()
{
    bool noSubmenu = true;
    while (noSubmenu)
    {
        Console.Clear();
        Console.WriteLine("==================================================");
        Console.WriteLine("               GERENCIAR PAGAMENTOS               ");
        Console.WriteLine("==================================================");
        Console.WriteLine(" 1 - Registrar Pagamento");
        Console.WriteLine(" 2 - Listar Todos os Pagamentos");
        Console.WriteLine(" 3 - Buscar Pagamento por ID");
        Console.WriteLine(" 0 - Voltar ao Menu Principal");
        Console.WriteLine("==================================================");
        Console.Write("Escolha uma opção: ");

        string? subOpcao = Console.ReadLine();

        switch (subOpcao)
        {
            case "1":
                Console.Clear();
                Console.WriteLine("--- REGISTRAR PAGAMENTO ---");
                Console.Write("Valor (R$): ");
                if (!float.TryParse(Console.ReadLine(), out float value)) value = 0;
                Console.Write("ID da Empresa (Company): ");
                int.TryParse(Console.ReadLine(), out int companyId);
                Console.Write("ID do Usuário (Cliente/Owner) Opcional: ");
                int? userId = null;
                string? userStr = Console.ReadLine();
                if (int.TryParse(userStr, out int uId)) userId = uId;

                var novoPagamento = new Payment
                {
                    Value = value,
                    DueDate = DateTime.Now.AddDays(30),
                    ToDate = DateTime.Now,
                    CompanyId = companyId,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await novoPagamento.InserirAsync();
                PressionarParaContinuar("Pagamento registrado com sucesso!");
                break;

            case "2":
                Console.Clear();
                var lista = await paymentService.BuscarTodosAsync();
                foreach (var p in lista)
                {
                    Console.WriteLine($"ID: {p.Id} | Valor: R$ {p.Value:F2} | Vencimento: {p.DueDate:dd/MM/yyyy} | Empresa ID: {p.company?.Id} | Usuário ID: {p.user?.Id}");
                }
                PressionarParaContinuar();
                break;

            case "3":
                Console.Clear();
                Console.Write("Digite o ID do Pagamento: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var p = new Payment();
                    await p.BuscarAsync(id);
                    if (p.Id > 0)
                    {
                        Console.WriteLine($"ID: {p.Id} | Valor: R$ {p.Value:F2} | Vencimento: {p.DueDate:dd/MM/yyyy} | Empresa ID: {p.company?.Id} (Nome: {p.company?.Name}) | Usuário ID: {p.user?.Id} (Nome: {p.user?.Name})");
                    }
                    else
                    {
                        Console.WriteLine("Pagamento não encontrado.");
                    }
                }
                PressionarParaContinuar();
                break;

            case "0":
                noSubmenu = false;
                break;
        }
    }
}
#endregion

void PressionarParaContinuar(string? mensagem = null)
{
    if (!string.IsNullOrEmpty(mensagem))
    {
        Console.WriteLine($"\n{mensagem}");
    }
    Console.WriteLine("\nPressione ENTER para continuar...");
    Console.ReadLine();
}

