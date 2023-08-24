using api.email;
using api.Models;
using Microsoft.AspNetCore.Mvc;
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


        //TODO:end point that adds a user when given the details
        [HttpPost("add")]
        public async Task<IActionResult> addUser([FromBody]UserInfo user)
        {
            return Ok("User added");
        }

         //TODO:login:end point that will get the email and encrypted pasword and if successfull      return the user object from db 
        [HttpGet("Login")]
        public async Task<IActionResult> login(string? email,string?password)
        {
            return null;
        }

        //TODO:delete user when given the user object
        [HttpDelete("remove")]
        public async Task<IActionResult> removeUser([FromBody]UserInfo user)
        {
            return null;
        }

    }

}