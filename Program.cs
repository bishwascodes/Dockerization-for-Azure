using API.Data;
using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.Swashbuckle;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatasyncServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => opt.AddDatasyncControllers());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS \"pgcrypto\"");
    await db.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS \"grocery_list\"");
    await db.Database.EnsureCreatedAsync();
    await db.Database.ExecuteSqlRawAsync("ALTER TABLE IF EXISTS \"grocery_list\".\"GroceryItems\" ALTER COLUMN \"Version\" SET DEFAULT gen_random_bytes(16)");
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
