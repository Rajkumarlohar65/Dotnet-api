using MySql.Data.MySqlClient;
class DatabaseContext
{
    private readonly string connectionString;

    public DatabaseContext(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectionString);
    }
}