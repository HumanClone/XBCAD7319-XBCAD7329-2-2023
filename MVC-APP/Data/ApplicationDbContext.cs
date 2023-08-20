using Microsoft.EntityFrameworkCore;
using MVCAPP.Models;

namespace MVCAPP.Data
{
public class ApplicationDbContext: DbContext 
{
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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