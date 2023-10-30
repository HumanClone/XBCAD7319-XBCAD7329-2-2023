using api.email;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Resources;
using System.Text.RegularExpressions;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController:ControllerBase
    {
        
        private readonly XbcadDb2Context _context;

        public UsersController(XbcadDb2Context context)
        {
            _context = context;
            
        }


        //end point that adds a user when given the details make sure email doesnt already exist in Userlogin table if it does not then add the user and thier login to the respective tables, use bcrypt to check the hashes 
        [HttpPost("add")]
        public async Task<IActionResult> addUser([FromBody]UserInfo user)
        {
            //check if the email already exists in the userlogin table
            var email = _context.UserLogins.Where(x=>x.Email==user.Email).FirstOrDefault();
            if(email!=null)
            {
                return BadRequest("Email already exists");
            }
            else
            {
                //add the user to the userlogin table
                _context.UserInfos.Add(user);
                _context.SaveChanges();
                return Ok("User added");
            }
        }


        /// How to Secure Passwords with BCrypt.NET
        /// [
        ///  var passwordHash = BCrypt.HashPassword("Password123!");
        /// ]
        /// https://code-maze.com/dotnet-secure-passwords-bcrypt/
        /// Acccessed[1 September 2023]
        [HttpPost("addlogin")]
        public async Task<IActionResult> addCredentials([FromBody]UserLogin user)
        {
            //check if the email already exists in the userlogin table
            var email = _context.UserLogins.Where(x=>x.Email==user.Email).FirstOrDefault();
            if(email!=null)
            {
                return BadRequest("Email already exists");
            }
            else
            {
                //hash the password
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                //add the user to the userlogin table
                _context.UserLogins.Add(user);
                _context.SaveChanges();
                return Ok("User added");
            }
        }


        //login:end point that will get the email and encrypted pasword and if successfull return the user object from db but check if the email exists in the dev table first, if it exists then return the devteam object
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin cred)
        {
           //check if the user exists in the devteam table
            var dev = _context.TeamDevs.Where(x=>x.Email==cred.Email).FirstOrDefault();
            if(dev!=null)
            {
                var password = _context.UserLogins.Where(x=>x.Email==cred.Email).FirstOrDefault().Password;
                if(BCrypt.Net.BCrypt.Verify(cred.Password, password))
                {
                        //return the user object
                    return Ok(dev);
                }
                else
                {
                    return BadRequest("Incorrect password");
                }
            }
            else
            {
                var user = _context.UserInfos.Where(x=>x.Email==cred.Email).FirstOrDefault();
                if(user!=null)
                {
                    var password = _context.UserLogins.Where(x=>x.Email==cred.Email).FirstOrDefault().Password;
                    //check if the password matches the hash
                    if(BCrypt.Net.BCrypt.Verify(cred.Password, password))
                    {
                        //return the user object
                        return Ok(user);

                    }
                    else
                    {
                        return BadRequest("Incorrect password");
                    }
                }
                else
                {
                    return BadRequest("User does not exist");
                }
            }
        }

        // delete user when given the user object and delete thier object from the userlogin table by using the email
        [HttpDelete("remove")]
        public async Task<IActionResult> removeUser(int? userID)
        {
            //get the users email from the userlogin table
            var user = _context.UserInfos.Where(x=>x.UserId==userID).FirstOrDefault();
            string email = user.Email;

            //remove the user from the userlogin table
            _context.UserLogins.Remove(_context.UserLogins.Where(x=>x.Email==email).FirstOrDefault());
            _context.UserInfos.Remove(user);
            _context.SaveChanges();
            return Ok("User removed");
        }

        [HttpGet("users")]
        public async Task<List<UserInfo>> users()
        {
            return _context.UserInfos.ToList<UserInfo>();
        }

        [HttpGet("user")]
        public async Task<UserInfo> user(string? userId)
        {
            int id=int.Parse(userId);
            var user =  _context.UserInfos.Where(user => user.UserId==id).First();
            return user;
        }


        [HttpGet("userEmail")]
        public async Task<UserInfo> userEmail(string? email)
        {
            var user =  _context.UserInfos.Where(user => user.Email==email).First();
            return user;
        }


        [HttpGet("devs")]
        public async Task<List<TeamDev>> devs()
        {
            return _context.TeamDevs.ToList();
        }

        //Get dev email with id
        [HttpGet("devEmail")]
        public async Task<string> devEmail(string? devId)
        {
            int id=int.Parse(devId);
            var dev =  _context.TeamDevs.Where(dev => dev.DevId==id).First();
            return dev.Email;
        }

    }

}