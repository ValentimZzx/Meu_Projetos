using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

partial class Program
{
    static List<CadastrarProduto> ListaProdutos = new();
    static List<NovoEndereco> ListaEnderecos = new();
    static List<string> EnderecosFisicos = new() { "Prateleira A1", "Prateleira B2", "Prateleira C3" };

    static void Main()
    {
        // Endereço padrão
        NovoEndereco enderecoPadrao = new NovoEndereco("Recebimento");
        ListaEnderecos.Add(enderecoPadrao);

        bool logado = false;
        bool executar = true;

        while (executar && !logado)
        {
            Console.WriteLine("=== Sistema de Estoque ===");
            Console.WriteLine("1 - Login");
            Console.WriteLine("2 - Cadastrar Usuário");
            Console.WriteLine("3 - Sair");
            Console.Write("Escolha uma opção: ");
            string opcaoInicial = Console.ReadLine();

            switch (opcaoInicial)
            {
                case "1":
                    logado = Login();
                    if (!logado)
                    {
                        Console.WriteLine("Login falhou. Tente novamente.");
                    }
                    break;

                case "2":
                    CadastrarUsuario();
                    break;

                case "3":
                    executar = false;
                    Console.WriteLine("Encerrando o sistema...");
                    break;

                default:
                    Console.WriteLine("Opção inválida! Tente novamente.");
                    break;
            }
        }

        if (logado)
        {
            MenuPrincipal(enderecoPadrao);
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }

    static bool Login()
    {
        Console.WriteLine("=== Login ===");
        Console.Write("Login: ");
        string login = Console.ReadLine();

        Console.Write("Senha: ");
        string senha = Console.ReadLine();

        using (var conexao = new SqliteConnection("Data Source=estoque.db"))
        {
            conexao.Open();

            string sql = "SELECT COUNT(*) FROM Usuarios WHERE Login = @login AND Senha = @senha";
            using (var cmd = new SqliteCommand(sql, conexao))
            {
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@senha", senha);

                long count = (long)cmd.ExecuteScalar();
                if (count > 0)
                {
                    Console.WriteLine("Login bem-sucedido!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Login ou senha inválidos.");
                    return false;
                }
            }
        }
    }

    static void CadastrarUsuario()
    {
        Console.WriteLine("=== Cadastro de Novo Usuário ===");

        Console.Write("Nome: ");
        string nome = Console.ReadLine();

        Console.Write("Login: ");
        string login = Console.ReadLine();

        Console.Write("Senha: ");
        string senha = Console.ReadLine();

        using (var conexao = new SqliteConnection("Data Source=estoque.db"))
        {
            conexao.Open();

            string sql = "INSERT INTO Usuarios (Nome, Login, Senha) VALUES (@nome, @login, @senha)";
            using (var cmd = new SqliteCommand(sql, conexao))
            {
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@senha", senha); // Em produção, criptografe isso!

                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Usuário cadastrado com sucesso!");
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine("Erro: " + ex.Message);
                }
            }
        }
    }

    // Seu MenuPrincipal() e demais métodos seguem exatamente iguais
    static void MenuPrincipal(NovoEndereco enderecoPadrao)
    {
        bool continuar = true;
        Console.WriteLine("Pressione qualquer tecla para sair...");
        Console.ReadKey();

        while (continuar)
        {
            Console.WriteLine("\n=== Controle de Estoque ===\n");
            Console.WriteLine("1 - Cadastrar novo produto");
            Console.WriteLine("2 - Movimentar quantidade");
            Console.WriteLine("3 - Consulta de produtos");
            Console.WriteLine("4 - Cadastrar Endereço");
            Console.WriteLine("5 - Movimentar produto para endereço físico");
            Console.WriteLine("6 - Sair");


            Console.Write("\nEscolha uma opção: ");
            int opcao = Convert.ToInt32(Console.ReadLine());

            switch (opcao)
            {
                case 1:
                    Console.WriteLine("\n=== Cadastro de Produto ===");

                    Console.Write("Digite o Id: ");
                    int id = Convert.ToInt32(Console.ReadLine());

                    Console.Write("Nome do produto: ");
                    string nome = Console.ReadLine();

                    Console.Write("Valor: ");
                    double preco = Convert.ToDouble(Console.ReadLine());

                    Console.Write("Quantidade: ");
                    int quantidade = Convert.ToInt32(Console.ReadLine());

                    CadastrarProduto novoProduto = new(id, nome, preco, quantidade, "Aguardando movimentação", enderecoPadrao);
                    ListaProdutos.Add(novoProduto);
                    Console.WriteLine($"Produto '{nome}' cadastrado com sucesso no endereço: {enderecoPadrao.Codigo}.");
                    break;

                case 2:
                    Console.WriteLine("\n=== Movimentar Quantidade ===");

                    Console.Write("Digite o ID do produto: ");
                    int idMov = Convert.ToInt32(Console.ReadLine());

                    var prod = ListaProdutos.Find(p => p.Id == idMov);
                    if (prod == null)
                    {
                        Console.WriteLine("Produto não encontrado.");
                        break;
                    }

                    Console.WriteLine($"Produto selecionado: {prod.Nome} | Quantidade atual: {prod.Quantidade}");
                    Console.WriteLine("1 - Adicionar");
                    Console.WriteLine("2 - Remover");
                    int tipoMov = Convert.ToInt32(Console.ReadLine());

                    Console.Write("Quantidade: ");
                    int qnt = Convert.ToInt32(Console.ReadLine());

                    if (tipoMov == 1)
                    {
                        prod.Quantidade += qnt;
                        Console.WriteLine("Quantidade adicionada.");
                    }
                    else if (tipoMov == 2)
                    {
                        if (qnt > prod.Quantidade)
                            Console.WriteLine("Erro: estoque insuficiente.");
                        else
                        {
                            prod.Quantidade -= qnt;
                            Console.WriteLine("Quantidade removida.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Opção inválida.");
                    }
                    break;

                case 3:
                    Console.WriteLine("\n=== Consulta de Produtos ===");

                    if (ListaProdutos.Count == 0)
                        Console.WriteLine("Nenhum produto cadastrado.");
                    else
                    {
                        foreach (var p in ListaProdutos)
                        {
                            Console.WriteLine($"ID: {p.Id} | Nome: {p.Nome} | Quantidade: {p.Quantidade} | Endereço: {p.Endereco.Codigo} | Status: {p.Status}");
                        }
                    }
                    break;

                case 4:
                    Console.WriteLine("\n=== Cadastrar Novo Endereço ===");

                    Console.Write("Código do endereço: ");
                    string codEnd = Console.ReadLine();

                    NovoEndereco novoEnd = new NovoEndereco(codEnd);
                    ListaEnderecos.Add(novoEnd);
                    EnderecosFisicos.Add(codEnd);

                    Console.WriteLine($"Endereço '{codEnd}' cadastrado com sucesso.");
                    break;

                case 5:
                    Console.WriteLine("\n=== Movimentar Produto para Endereço ===");

                    var aguardando = ListaProdutos.Where(p => p.Endereco.Codigo == "Recebimento").ToList();

                    if (aguardando.Count == 0)
                    {
                        Console.WriteLine("Nenhum produto aguardando movimentação.");
                        break;
                    }

                    foreach (var p in aguardando)
                    {
                        Console.WriteLine($"ID: {p.Id} | Nome: {p.Nome} | Quantidade: {p.Quantidade}");
                    }

                    Console.Write("Digite o ID do produto para movimentar: ");
                    int idMovFisico = Convert.ToInt32(Console.ReadLine());
                    var prodMov = aguardando.FirstOrDefault(p => p.Id == idMovFisico);

                    if (prodMov != null)
                    {
                        Console.WriteLine("Endereços disponíveis:");
                        for (int i = 0; i < EnderecosFisicos.Count; i++)
                        {
                            Console.WriteLine($"{i + 1} - {EnderecosFisicos[i]}");
                        }

                        Console.Write("Escolha o número do endereço: ");
                        int idxEnd = Convert.ToInt32(Console.ReadLine());

                        if (idxEnd >= 1 && idxEnd <= EnderecosFisicos.Count)
                        {
                            string codigoSelecionado = EnderecosFisicos[idxEnd - 1];
                            var enderecoSelecionado = ListaEnderecos.FirstOrDefault(e => e.Codigo == codigoSelecionado);

                            if (enderecoSelecionado != null)
                            {
                                prodMov.Endereco = enderecoSelecionado;
                                prodMov.Status = "Armazenado";
                                Console.WriteLine("Produto movimentado com sucesso!");
                            }
                            else
                            {
                                Console.WriteLine("Endereço não encontrado.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Endereço inválido.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Produto não encontrado.");
                    }
                    break;

                case 6:
                    continuar = false;
                    Console.WriteLine("Encerrando o sistema...");
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
        }
    }
}

public class CadastrarProduto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public double Preço { get; set; }
    public int Quantidade { get; set; }
    public string Status { get; set; }
    public NovoEndereco Endereco { get; set; }

    public CadastrarProduto(int id, string nome, double preco, int quantidade, string status, NovoEndereco endereco)
    {
        Id = id;
        Nome = nome;
        Preço = preco;
        Quantidade = quantidade;
        Status = status;
        Endereco = endereco;
    }
}

public class NovoEndereco
{
    public string Codigo { get; set; }

    public NovoEndereco(string codigo)
    {
        Codigo = codigo;
    }
}
