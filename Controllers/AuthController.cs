using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayPart2Final.backend;
using RaceDayPart2Final.models;

namespace RaceDayPart2Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public enum UserRole 
        { 
            Organiser,
            Participants,
        
        }
        [HttpPost("Register")]
        public string RegisterInfo(string fullName, string email, string phone, string password, UserRole role)
        {
            // hash the password before storing
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            Authentication store = new Authentication();
            string result = store.RegisterUserInfo(fullName, email, phone, hashedPassword, role.ToString());

            return result;
        }

        [HttpPost("Login")]
        public string LoginUser(string email, string password)
        {
            Authentication use = new Authentication();
            string reply = use.LogInInfo(email, password);
            return reply;
        }
    }
}