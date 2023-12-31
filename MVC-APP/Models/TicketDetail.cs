﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace MVCAPP.Models
{
    public class TicketDetail
    {
        [Key]
        public int TicketId {get; set;}
        
        [ForeignKey("Category")]
        public string? CategoryId {get; set;}
        
        [ForeignKey("TeamDev")]
        public string? DevId {get; set;}
         public string? Links {get;set;}
        public string? Notes {get;set;}
        public int? Priority {get;set;}

        [ForeignKey("UserInfo")]
        public int? UserId {get; set;}
        public string? CategoryName {get; set;}
        public DateTime DateIssued {get; set;}
        public string? MessageContent {get; set;}
        public string? Status {get; set;} = "Pending";
        
    }
}