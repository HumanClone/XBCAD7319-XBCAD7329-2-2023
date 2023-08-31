using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace api.Models
{
    public class TicketDetail
    {
        [Key]
        public int TicketId {get; set;}
        
        [ForeignKey("Category")]
        public string? CategoryId {get; set;}
        
        [ForeignKey("TeamDev")]
        public string? DevId {get; set;}

        [ForeignKey("UserInfo")]
        public int? UserId {get; set;}
        public string? CategoryName {get; set;}
        public DateTime DateIssued {get; set;}
        public string? MessageContent {get; set;}
        public string? Status {get; set;} = "Pending";
        
    }
}