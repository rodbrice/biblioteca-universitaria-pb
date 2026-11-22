// Projeto de Bloco (PB) — TP1
// Aluno : Brice Roduit
//
// Sistema de Gestão de Biblioteca Universitária: primeiro programa standalone.
// Ainda não há classes nem dados guardados. O operador informa a situação e o
// programa aplica as regras de negócio (RN) do documento de requisitos.

using System.Globalization;
using System.Text;

CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
Console.OutputEncoding = Encoding.UTF8;

const int PrazoDias = 14;            // RN01
const int LimiteEmprestimos = 3;     // RN03
const decimal MultaPorDia = 2.00m;   // suposição S01: a especificação não define o valor

int emprestimosAprovados = 0;
int emprestimosRecusados = 0;
int reservasAprovadas = 0;
int reservasRecusadas = 0;
int devolucoes = 0;
int devolucoesComAtraso = 0;
decimal totalMultas = 0m;

Console.WriteLine("Biblioteca Universitária");

int opcao;
do
{
    Console.WriteLine();
    Console.WriteLine("1 - Verificar empréstimo");
    Console.WriteLine("2 - Calcular devolução e multa");
    Console.WriteLine("3 - Verificar reserva");
    Console.WriteLine("4 - Tabela de multas por atraso");
    Console.WriteLine("5 - Resumo da sessão");
    Console.WriteLine("0 - Sair");
    opcao = LerInteiro("Opção: ", 0, 5);
    Console.WriteLine();

    switch (opcao)
    {
        case 1:
        {
            bool cadastroValido = LerSimNao("O membro tem cadastro válido? (s/n): ");
            int ativos = LerInteiro("Quantos empréstimos ativos ele tem? (0 a 3): ", 0, LimiteEmprestimos);
            bool disponivel = LerSimNao("O exemplar está disponível? (s/n): ");
            int conservacao = LerInteiro("Conservação (1-bom, 2-regular, 3-danificado): ", 1, 3);

            bool condicaoAdequada = conservacao != 3;
            bool aprovado = cadastroValido && ativos < LimiteEmprestimos && disponivel && condicaoAdequada;

            if (aprovado)
            {
                DateOnly retirada = LerData("Data da retirada (dd/mm/aaaa): ");
                DateOnly prazo = retirada.AddDays(PrazoDias);
                emprestimosAprovados++;

                Console.WriteLine();
                Console.WriteLine("Empréstimo APROVADO.");
                Console.WriteLine($"Devolver até: {prazo:dd/MM/yyyy}");
                Console.WriteLine($"Empréstimos que o membro ainda pode fazer: {LimiteEmprestimos - (ativos + 1)}");
            }
            else
            {
                emprestimosRecusados++;

                // mostra todos os motivos, e não só o primeiro
                Console.WriteLine();
                Console.WriteLine("Empréstimo RECUSADO:");
                if (!cadastroValido)
                {
                    Console.WriteLine("- membro sem cadastro válido (RN12)");
                }
                if (ativos >= LimiteEmprestimos)
                {
                    Console.WriteLine($"- limite de {LimiteEmprestimos} empréstimos ativos atingido (RN03)");
                }
                if (!disponivel)
                {
                    Console.WriteLine("- exemplar indisponível (RN04)");
                }
                if (!condicaoAdequada)
                {
                    Console.WriteLine("- exemplar danificado, precisa ser regularizado (RN10)");
                }
            }
            break;
        }

        case 2:
        {
            DateOnly retirada = LerData("Data da retirada (dd/mm/aaaa): ");
            DateOnly devolucao = LerData("Data da devolução (dd/mm/aaaa): ");
            while (devolucao < retirada)
            {
                Console.WriteLine("A devolução não pode ser antes da retirada.");
                devolucao = LerData("Data da devolução (dd/mm/aaaa): ");
            }

            DateOnly prazo = retirada.AddDays(PrazoDias);
            int diasComOLivro = devolucao.DayNumber - retirada.DayNumber;
            int diasAtraso = devolucao.DayNumber - prazo.DayNumber;
            devolucoes++;

            Console.WriteLine();
            Console.WriteLine($"Prazo: {prazo:dd/MM/yyyy}");
            Console.WriteLine($"Dias com o exemplar: {diasComOLivro}");

            if (diasAtraso > 0)
            {
                decimal multa = diasAtraso * MultaPorDia;
                devolucoesComAtraso++;
                totalMultas += multa;
                Console.WriteLine($"Devolvido com {diasAtraso} dia(s) de atraso. Multa: R$ {multa:F2}");
            }
            else
            {
                Console.WriteLine("Devolvido dentro do prazo. Sem multa.");
            }
            break;
        }

        case 3:
        {
            bool cadastroValido = LerSimNao("O membro tem cadastro válido? (s/n): ");
            int totalExemplares = LerInteiro("Quantos exemplares o livro tem? ", 0, 1000);
            int emprestados = 0;
            if (totalExemplares > 0)
            {
                emprestados = LerInteiro("Quantos estão emprestados agora? ", 0, totalExemplares);
            }

            Console.WriteLine();
            if (cadastroValido && emprestados > 0)
            {
                reservasAprovadas++;
                Console.WriteLine("Reserva APROVADA. Ela vale para o livro, não para um exemplar específico.");
            }
            else
            {
                reservasRecusadas++;
                Console.WriteLine("Reserva RECUSADA:");
                if (!cadastroValido)
                {
                    Console.WriteLine("- membro sem cadastro válido (RN12)");
                }
                if (totalExemplares == 0)
                {
                    Console.WriteLine("- o livro não tem exemplares cadastrados");
                }
                else if (emprestados == 0)
                {
                    Console.WriteLine("- nenhum exemplar está emprestado (RN06)");
                }
            }
            break;
        }

        case 4:
        {
            int dias = LerInteiro("Mostrar até quantos dias de atraso? (1 a 60): ", 1, 60);
            Console.WriteLine();
            for (int dia = 1; dia <= dias; dia++)
            {
                Console.WriteLine($"{dia,2} dia(s): R$ {dia * MultaPorDia,7:F2}");
            }
            break;
        }

        case 5:
        {
            int pedidos = emprestimosAprovados + emprestimosRecusados;
            Console.WriteLine($"Empréstimos: {emprestimosAprovados} aprovado(s), {emprestimosRecusados} recusado(s)");
            if (pedidos > 0)
            {
                double percentual = (double)emprestimosAprovados / pedidos * 100;
                Console.WriteLine($"Aprovação: {percentual:F1}%");
            }
            Console.WriteLine($"Reservas: {reservasAprovadas} aprovada(s), {reservasRecusadas} recusada(s)");
            Console.WriteLine($"Devoluções: {devolucoes}, sendo {devolucoesComAtraso} com atraso");
            Console.WriteLine($"Total de multas: R$ {totalMultas:F2}");
            break;
        }

        case 0:
            Console.WriteLine("Programa encerrado.");
            break;
    }
} while (opcao != 0);

// Funções de leitura: repetem a pergunta até a resposta ser válida.

static string LerTexto(string mensagem)
{
    Console.Write(mensagem);
    string? linha = Console.ReadLine();
    if (linha == null) // a entrada acabou (Ctrl+Z ou fim do arquivo)
    {
        Console.WriteLine();
        Console.WriteLine("Entrada encerrada.");
        Environment.Exit(0);
    }
    return linha.Trim();
}

static int LerInteiro(string mensagem, int minimo, int maximo)
{
    while (true)
    {
        if (int.TryParse(LerTexto(mensagem), out int valor) && valor >= minimo && valor <= maximo)
        {
            return valor;
        }
        Console.WriteLine($"Digite um número inteiro de {minimo} a {maximo}.");
    }
}

static bool LerSimNao(string mensagem)
{
    while (true)
    {
        string resposta = LerTexto(mensagem).ToLower();
        if (resposta == "s")
        {
            return true;
        }
        if (resposta == "n")
        {
            return false;
        }
        Console.WriteLine("Responda com s ou n.");
    }
}

static DateOnly LerData(string mensagem)
{
    while (true)
    {
        bool valida = DateOnly.TryParseExact(LerTexto(mensagem), "dd/MM/yyyy",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly data);
        if (valida && data.Year >= 2000 && data.Year <= 2100)
        {
            return data;
        }
        Console.WriteLine("Data inválida. Use dd/mm/aaaa, com ano de 2000 a 2100.");
    }
}
