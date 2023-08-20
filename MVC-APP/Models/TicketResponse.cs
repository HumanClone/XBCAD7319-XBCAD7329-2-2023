using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace MVCAPP.Models
{
    public class TicketResponse
    {
        [Key]
        public int ResponseId {get; set;}
        [Required]
        [ForeignKey("TicketDetail")]
        public string TicketId {get; set;}
        [Required]
        [ForeignKey("TeamDev")]
        public string DevId {get; set;}
        [Required]
        [ForeignKey("UserLogin")]
        public string Email {get; set;}
        public string? ResponseMessage {get; set;}
    }
}