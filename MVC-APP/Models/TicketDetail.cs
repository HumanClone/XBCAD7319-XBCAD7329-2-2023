using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace MVCAPP.Models
{
    public class TicketDetail
    {
        [Key]
        public int TicketId {get; set;}
        [Required]
        [ForeignKey("Category")]
        public string? CategoryId {get; set;}
        [ForeignKey("Category")]
        public string? CategoryName {get; set;}
        [Required]
        [ForeignKey("TeamDev")]
        public string? DevId {get; set;}
        public DateTime DateIssued {get; set;}
        public string? MessageContent {get; set;}
        public string? Status {get; set;}
        
    }
}