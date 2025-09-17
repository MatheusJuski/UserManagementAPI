using UserManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Repositories;
using UserManagementAPI.Services;


var builder = WebApplication.CreateBuilder(args);


// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// IUserRepository for solicitado em Services ou Controllers, o UserRepository será injetado automaticamente
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "UsermanagementAPI Funcionando!");

app.Run();


