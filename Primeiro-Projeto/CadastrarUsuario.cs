using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
void CadastrarUsuario()
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


