using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using proiect_practica.Data;
using proiect_practica.Models;
using proiect_practica.Models.DTO;
using System.Security.Cryptography;

namespace proiect_practica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DBcontext db;

        public UserController(DBcontext db)
        {
            this.db = db;
        }
        [HttpPost("Register")]
          
        public IActionResult Register([FromBody] RegisterUserDTO user)
        {
            if(db.Users.Any(x => x.Email == user.Email))
            {
                throw new Exception();
            }
            byte[] passwordHash, passwordKey;
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password));

            }
            User account = new User
            {
                Email = user.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordKey,
                Name = user.Name,
            };
            db.Users.Add(account);
            db.SaveChanges();
                return Ok();
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginUserDTO user)
        {
            var userInfo = db.Users.SingleOrDefault(x => x.Email.Equals(user.Email));
            if(userInfo == null || userInfo.PasswordSalt == null)
            {
                return BadRequest("Emailul nu se regaseste in baza de date!");
            }
            if(!MatchPasswordHash(user.Password, userInfo.PasswordHash, userInfo.PasswordSalt))
            {
                return BadRequest("Parola gresita!");

            }
            return Ok("Autentificarea a avut loc cu succes!");

        }
        private bool MatchPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using(var hmac = new HMACSHA512(salt))
            {
                var passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i=0; i < hash.Length; i++)
                {
                    if (hash[i] != passwordhash[i])
                    {
                        return false;
                    }

                }
                return true;
            }
        }
    }
}
