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
        public string? DevId {get; set;}
        public string? sender {get; set;}
        public DateTime? date {get;set;}
        public string? ResponseMessage {get; set;}
    }
}