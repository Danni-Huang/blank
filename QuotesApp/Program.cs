using Microsoft.EntityFrameworkCore;
using QuotesApp.Extensions;
using QuotesApp.Models;
using QuotesApp.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<QuoteContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuoteContext")));

builder.Services.AddScoped<IAuthService, AuthService>();

// configure json serializer to use reference handling
// this Preserve format Json as $id and $values
/*builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});*/

// Add services to the container.
builder.Services.AddControllersWithViews();

// use our service extensions methods:
builder.Services.ConfigureCors();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);

// Add CORS support, first here as a DI service:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowQuoteClients", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// and finally say that we are using CORS here specifying the CORS policy by name:
app.UseCors("AllowQuoteClients");
app.UseCors("QuoteApiCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
