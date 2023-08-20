using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace MVCAPP.Models
{
    public class UserInfo{
        [Key]
        public int UserId {get; set;}
        [Required]
        [ForeignKey("UserLogin")]
        public string Email {get; set;}
        public string? Name {get; set;} 
        public string? PhoneNumber {get; set;}
    }
}