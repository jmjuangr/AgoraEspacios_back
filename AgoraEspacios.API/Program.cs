using Microsoft.EntityFrameworkCore;
using AgoraEspacios.Data;
using AgoraEspacios.Data.Repositories;
using AgoraEspacios.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Conexión a base de datos 
builder.Services.AddDbContext<EspaciosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controladores
builder.Services.AddControllers();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapControllers();

// Migraciones automáticas al arrancar (como en GestApp)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EspaciosDbContext>();
    db.Database.Migrate();
}

app.Run();
