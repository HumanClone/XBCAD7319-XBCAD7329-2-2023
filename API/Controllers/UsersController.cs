using api.email;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Resources;
using System.Text.RegularExpressions;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController:ControllerBase
    {
        
        private readonly StudentSupportXbcadContext _context;

        public UsersController(StudentSupportXbcadContext context)
        {
            _context = context;
            
        }


        //TODO:end point that adds a user when given the details make sure email doesnt already exist in Userlogin table if it does not then add the user and thier login to the respective tables
        [HttpPost("add")]
        public async Task<IActionResult> addUser([FromBody]UserInfo user)
        {
            return Ok("User added");
        }


        //TODO:to add a login to the 
        [HttpPost("addlogin")]
        public async Task<IActionResult> addCredentials([FromBody]UserLogin user)
        {
            return null;
        }

         //TODO:login:end point that will get the email and encrypted pasword and if successfull return the user object from db but check if the email exists in the dev table first, if it exists then return the devteam object
        [HttpGet("Login")]
        public async  Task<IActionResult> login(string? email,string?password)
        {
            return null;
        }

        //TODO: delete user when given the user object and delete thier object from the userlogin table by using the email
        [HttpDelete("remove")]
        public async Task<IActionResult> removeUser(string? userID)
        {
            return null;
        }

    }

}