using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Models
{
public partial class StudentSupportXbcadContext: DbContext 
{
    
    public StudentSupportXbcadContext(DbContextOptions<StudentSupportXbcadContext> options)
    :base(options){

    }
    public DbSet<UserLogin>UserLogin{get; set;}
    public DbSet<UserInfo>UserInfo{get; set;}
    public DbSet<Category>Categories{get; set;}
    public DbSet<TicketDetail>TicketDetails{get; set;}
    public DbSet<TeamDev>TeamDevs{get; set;}
    public DbSet<TicketResponse>TicketResponses{get; set;}

}
}