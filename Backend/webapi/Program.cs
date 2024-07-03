using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Microsoft.AspNetCore.Http.StatusCodes;
using webapi.DL.Repositories;

using webapi.Models;
using webapi.DL.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IBaseRepository<UserInfo>, BaseRepository<UserInfo>>();
builder.Services.AddTransient<IBaseRepository<FriendsList>, BaseRepository<FriendsList>>();
builder.Services.AddTransient<IBaseRepository<FriendInvites>, BaseRepository<FriendInvites>>();
builder.Services.AddTransient<IGetUsername, GetUsername>();
builder.Services.AddTransient<IUsersRepository, UsersRepository>();

//Database setup
string ThatsTimeData = string.Empty;
string Accounts = string.Empty;

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    ThatsTimeData = builder.Configuration.GetConnectionString("DataConnection");
    Accounts = builder.Configuration.GetConnectionString("IdentityConnection");
}
else
{
    ThatsTimeData = builder.Configuration.GetConnectionString("DataConnection");
    Accounts = builder.Configuration.GetConnectionString("IdentityConnection");
}

builder.Services.AddDbContext<IdentityContext>(opts =>
    opts.UseSqlServer(Accounts, b => b.MigrationsAssembly("webapi")));

builder.Services.AddDbContext<DataContext>(opts =>
    opts.UseSqlServer(ThatsTimeData, b => b.MigrationsAssembly("webapi")));

//User account configs
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>();
builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.Password.RequiredLength = 6;
    opts.Password.RequireNonAlphanumeric = true;
    opts.Password.RequireLowercase = true;
    opts.Password.RequireUppercase = true;
    opts.Password.RequireDigit = true;
    opts.User.RequireUniqueEmail = true;
    opts.User.AllowedUserNameCharacters = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_";
});


//Jwt configuration starts here
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
});
//Jwt configuration ends here

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = Status307TemporaryRedirect;
        options.HttpsPort = 5000;
    });
}

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseDefaultFiles();
app.UseSpaStaticFiles();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "wwwroot";
    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
    }
});

var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<DataContext>().Database.Migrate();
scope.ServiceProvider.GetRequiredService<IdentityContext>().Database.Migrate();

app.Run();
