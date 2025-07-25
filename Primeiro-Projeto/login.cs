using Microsoft.Data.Sqlite;

public class Login
{
    public bool FazerLogin()
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
}
