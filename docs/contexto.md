# Contexto
... Estamos fazendo um projeto integrador para o final do curso de Desenvolvimento de Sistemas, iremos fazer um aplicativo de restaurante. Ele consiste em um aplicativo, onde o usuário pode ver um estabelecimento no qual o agrada e pode fazer um agendamento para ir até lá, apó marcar e comparecer no local, mais tarde quando entrar no aplicativo novamente ele avalia a empresa, se agradou . No aplicativo o usuário não paga nada, apenas o estabelecimento paga para poder usar o nosso aplicativo para hospedar seu restaurante. Na parte do usuário, ele entra se cadastra, faz login, escolhe o restaurante que deseja ir, verifica sua disponibilidade, verifica os horários disponíveis, verifica a capacidade do local, verifica se tem alguma restrição (vegana, alergias, etc) e faz o agendamento. A parte da empresa ele cadastra, coloca seu restaurante, coloca fotos, observações do lugar e assim o usuário fica a a vontade para esolher o que deseja e agendar. 


## Integrantes do grupo
- Jerry
- João
- Mauricio

### Divisão de tarefas

- `User`: Jerry
- `Company`: Mauricio
- `Schedulle`: João

- `Photo`: Mauricio
- `Assessment`: João
- `Payment`: Jerry

## CRUD
Relação de `CRUD` (inserir, ler, atualizar e remover) com o banco e as classes
- `User`: Inserir, ler, atualizar, remover
    - Não podem ser removidos os admins
    - Na remoção usar cascata (ao remover, deletar todos os dados relacionados)
- `Company`: inserir, ler, atualizar, remover
    - Remoção com cascata
- `Schedulle`: inserir, ler, alterar, remover
    - Não deve ser possível alterar um agendamento após passar do horário agendado
    - Não deve ser possível remover um agendamento após passar do horário agendado
- `Photo`: Inserir, ler, atualizar, remover
- `Assessment`: inserir, ler
- `Payment`: inserir, ler

## Banco de dados
Tabelas do banco e sua explicação:

- `users` (usuários): Tabela de usuários do sistema
	- **CLIENT:** é o cliente comum que usa o sistema
	- **ADMIN:** são os administradores do sistema com acesso geral
	- **OWNER:** usuários da empresa (companies)
- `companies` (companhias): Empresas que usam o sistema
	- Chave estrangeira atrelada a um usuário
- `schedulles` (agendamento): Agendamento que um usuário comum faz para uma empresa
	- Chave estrangeira com usuário (que agendou) e a empresa (com quem está sendo agendado)
- `photos` (fotos): Fotos do estabelecimento (companies)
	- Chave estrangeira da empresa que pertence a foto e do usuário que cadastrou o item no banco
- `assessment` (avaliações): avaliação com nota e comentário dos clientes sobre um agendamento
	- Chave estrangeira do usuário que avaliou e do agendamento
	- *Nota: a relação com o estabelecimento que foi avaliado vem por meio do agendamento*
- `payment`: pagamentos do usuário **do estabelecimento** para com os administradores dono do sistema
	- Chave estrangeira do usuário estabelecimento e qual é a companhia dele
