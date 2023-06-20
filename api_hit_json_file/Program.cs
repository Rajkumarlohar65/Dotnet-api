using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// GET REQUEST
app.MapGet("/userDetails", async (context) =>
{
    var json = File.ReadAllText("./data.json");
    await context.Response.WriteAsync(json);
});

// POST REQUEST
app.MapPost("/userDetails/add", async (HttpContext context) =>
        {
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var newUser = JsonConvert.DeserializeObject<User>(requestBody);

            // Read the existing users from the JSON file
            var existingUsers = new List<User>();
            var existingJson = await File.ReadAllTextAsync("./data.json");
            if (!string.IsNullOrEmpty(existingJson))
                existingUsers = JsonConvert.DeserializeObject<List<User>>(existingJson);

            // Add the new user to the existing users
            existingUsers!.Add(newUser!);

            // Write the updated users back to the JSON file
            await File.WriteAllTextAsync("./data.json", JsonConvert.SerializeObject(existingUsers, Formatting.Indented));

            // Send a response with the newly added user
            await context.Response.WriteAsJsonAsync(newUser);
        });

// DELETE REQUEST
app.MapDelete("/userDetails/delete/{id}", async (HttpContext context) =>
{
    // Get the user ID from the route parameters
    var id = context.Request.RouteValues["id"]?.ToString();

    // Read the existing users from the JSON file
    var existingUsers = new List<User>();
    var existingJson = await File.ReadAllTextAsync("./data.json");
    if (!string.IsNullOrEmpty(existingJson))
        existingUsers = JsonConvert.DeserializeObject<List<User>>(existingJson) ?? new List<User>(); // Ensure the list is not null

    // Find the user with the specified ID
    var userToDelete = existingUsers.FirstOrDefault(user => user?.Id.ToString() == id);


    if (userToDelete != null)
    {
        // Remove the user from the existing users
        existingUsers.Remove(userToDelete);

        // Write the updated users back to the JSON file
        await File.WriteAllTextAsync("./data.json", JsonConvert.SerializeObject(existingUsers, Formatting.Indented));

        // Send a response indicating successful deletion
        await context.Response.WriteAsync($"User with ID {id} has been deleted.");
    }
    else
    {
        // Send a response indicating that the user was not found
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync($"User with ID {id} was not found.");
    }
});

// PUT REQUEST
app.MapPut("/userDetails/update/{id}", async (HttpContext context) =>
{
    // Get the user ID from the route parameters
    var id = context.Request.RouteValues["id"]?.ToString();

    // Read the existing users from the JSON file
    var existingUsers = new List<User>();
    var existingJson = await File.ReadAllTextAsync("./data.json");
    if (!string.IsNullOrEmpty(existingJson))
        existingUsers = JsonConvert.DeserializeObject<List<User>>(existingJson) ?? new List<User>(); // Ensure the list is not null

    // Find the user with the specified ID
    var userToUpdate = existingUsers.FirstOrDefault(user => user?.Id.ToString() == id);

    if (userToUpdate != null)
    {
        // Read the updated user details from the request body
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var updatedUser = JsonConvert.DeserializeObject<User>(requestBody);

        // Update the user details
        userToUpdate.Name = updatedUser!.Name;
        userToUpdate.Email = updatedUser.Email;

        // Write the updated users back to the JSON file
        await File.WriteAllTextAsync("./data.json", JsonConvert.SerializeObject(existingUsers, Formatting.Indented));

        // Send a response with the updated user
        await context.Response.WriteAsJsonAsync(userToUpdate);
    }
    else
    {
        // Send a response indicating that the user was not found
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync($"User with ID {id} was not found.");
    }
});


app.Run();