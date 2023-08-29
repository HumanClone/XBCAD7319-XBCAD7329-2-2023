using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVCAPP.Models
{
    public class Category{
        [Key]
        public int CategoryId {get; set;}
        public string? CategoryName {get; set;}
    }
}