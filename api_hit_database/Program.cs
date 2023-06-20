using System.Data;
using System.Text;
using System.Text.Json;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json");

// Register database context
builder.Services.AddScoped<DatabaseContext>();

var app = builder.Build();

// GET REQUEST
app.MapGet("/userDetails", async (context) =>
{
    var dbContext = context.RequestServices.GetRequiredService<DatabaseContext>();

    using (var connection = dbContext.GetConnection())
    {
        await connection.OpenAsync();

        using (var command = new MySqlCommand("SELECT * FROM users", connection))
        using (var reader = await command.ExecuteReaderAsync())
        {
            var users = new List<User>();
            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32("id");
                string name = reader.GetString("name");
                string email = reader.GetString("email");
                string age = reader.GetString("age");
                // Retrieve other columns as needed

                var user = new User
                {
                    Id = id,
                    Name = name,
                    Email = email,
                    Age = age
                    // Set other properties as needed
                };

                users.Add(user);
            }

            var json = JsonSerializer.Serialize(users);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    }
});

// POST REQUEST
app.MapPost("/add/userDetails", async (context) =>
{
    var dbContext = context.RequestServices.GetRequiredService<DatabaseContext>();

    // Parse the request body to get the data to be added
    var requestBody = await JsonSerializer.DeserializeAsync<User>(context.Request.Body);

    using (var connection = dbContext.GetConnection())
    {
        await connection.OpenAsync();

        // Create a parameterized SQL command to insert the data
        var command = new MySqlCommand("insert into users(id, name, email, age) values(@id, @name, @email, @age)", connection);
        command.Parameters.AddWithValue("@id", requestBody!.Id);
        command.Parameters.AddWithValue("@name", requestBody.Name);
        command.Parameters.AddWithValue("@email", requestBody.Email);
        command.Parameters.AddWithValue("@age", requestBody.Age);

         // Execute the command
        await command.ExecuteNonQueryAsync();

        await context.Response.WriteAsync("Data Added Successfully");
        
    }
});

// DELETE REQUEST
app.MapDelete("/delete/userDetails/{id}", async (context) => 
{
    var dbContext = context.RequestServices.GetRequiredService<DatabaseContext>();
    var id = context.Request.RouteValues["id"].ToString();

    using(var connection = dbContext.GetConnection()){
        await connection.OpenAsync();

        var command = new MySqlCommand("Delete from users where id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        // Execute the command
        int rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected > 0)
        {
            await context.Response.WriteAsync("Data deleted successfully");
        }
        else
        {
            await context.Response.WriteAsync("No data found for the given ID");
        }
    }

});

// PUT REQUEST
app.MapPut("/update/userDetails/{id}", async (context) =>
{
    var dbContext = context.RequestServices.GetRequiredService<DatabaseContext>();
    var id = context.Request.RouteValues["id"].ToString();

    // Parse the request body to get the updated data
    var requestBody = await JsonSerializer.DeserializeAsync<User>(context.Request.Body);

    using (var connection = dbContext.GetConnection())
    {
        await connection.OpenAsync();

        // Create a parameterized SQL command to update the data
        var command = new MySqlCommand("UPDATE users SET name = @name, email = @email, age = @age WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", requestBody!.Name);
        command.Parameters.AddWithValue("@email", requestBody.Email);
        command.Parameters.AddWithValue("@age", requestBody.Age);

        // Execute the command
        int rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected > 0)
        {
            await context.Response.WriteAsync("Data updated successfully");
        }
        else
        {
            await context.Response.WriteAsync("No data found for the given ID");
        }
    }
});

app.Run();