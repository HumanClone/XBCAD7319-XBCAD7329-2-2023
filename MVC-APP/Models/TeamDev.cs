using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVCAPP.Models
{
    public class TeamDev{
        [Key]
        public int DevId {get; set;}
        public string Name {get; set;}
        public string? Surname {get; set;}
    }
}