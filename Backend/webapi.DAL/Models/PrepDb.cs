using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace webapi.Models;

public static class PrepDb
{
    public static void PrepMemberRoles(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope);
        }
    }

    private static void SeedData(IServiceScope serviceScope)
    {
        Console.WriteLine("--> Preparing database...");
        
        DataContext context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
        serviceScope.ServiceProvider.GetRequiredService<IdentityContext>().Database.Migrate();
        
        context.Database.Migrate();
        if (context.MemberRoles.SingleOrDefault(obj => obj.RoleName == "Undefined") == null)
        {
            context.MemberRoles.AddRange(
                new MemberRole() { RoleName = "Undefined"}, 
                new MemberRole() { RoleName = "Creator"}, 
                new MemberRole() { RoleName = "Moderator"},
                new MemberRole() {RoleName = "Member"}
            );

            context.SaveChanges();
        }
    }
}