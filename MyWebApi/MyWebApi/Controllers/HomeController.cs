using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myWebApi.data;
using myWebApi.models;

namespace myWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly MyDbContext dbContext;

        public UserController(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var query = "SELECT * FROM Users";
            var users = await dbContext.Users.FromSqlRaw(query).ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
        var query = $"INSERT INTO Users (Id, Name, Email, Age) VALUES ({user.Id}, '{user.Name}', '{user.Email}', {user.Age});";
            await dbContext.Database.ExecuteSqlRawAsync(query);

            return Ok(user);
        }

        [HttpPut]
        [Route("id")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var query = $"UPDATE Users SET Name = '{updatedUser.Name}', Email = '{updatedUser.Email}', Age = {updatedUser.Age} WHERE Id = {id}";
            await dbContext.Database.ExecuteSqlRawAsync(query);

            return Ok(updatedUser);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var query = $"DELETE FROM Users WHERE Id = {id}";
            await dbContext.Database.ExecuteSqlRawAsync(query);

            return NoContent();
        }
    }
}