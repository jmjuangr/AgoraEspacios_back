using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AgoraEspacios.Business.Services;
using AgoraEspacios.Data.Repositories;
using AgoraEspacios.Data;
using AgoraEspacios.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Conexion BD
builder.Services.AddDbContext<EspaciosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
var frontendUrl = builder.Configuration["Frontend:Url"] ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Repositorios y servicios
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CategoriaEspacioRepository>();
builder.Services.AddScoped<CategoriaEspacioService>();
builder.Services.AddScoped<EspacioRepository>();
builder.Services.AddScoped<EspacioService>();
builder.Services.AddScoped<ReservaRepository>();
builder.Services.AddScoped<ReservaService>();
// Servicio para poder llamar al webhook de n8n
builder.Services.AddHttpClient<IN8nService, N8nService>(client =>
{
    // No espero demasiado si n8n no responde
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "AgoraEspacios API", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Introduce tu token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt");
var claveSecreta = jwtConfig["Key"];

if (string.IsNullOrWhiteSpace(claveSecreta))
{
    throw new InvalidOperationException("No se ha configurado Key JWT");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveSecreta))
    };
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//permite activar swagger en prod
var swaggerEnabled = app.Environment.IsDevelopment()
    || builder.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Migraciones y creo user admin por defecto
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EspaciosDbContext>();
    db.Database.Migrate();

    var adminPassword = builder.Configuration["AdminUser:Password"] ?? "admin123";
    var admin = db.Usuarios.FirstOrDefault(u => u.Email == "admin@agoraespacios.com");

    if (admin == null)
    {
        admin = new Usuario
        {
            Nombre = "Administrador",
            Email = "admin@agoraespacios.com",
            Nif = "73018330T",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Rol = "Admin"
        };

        db.Usuarios.Add(admin);
        db.SaveChanges();
    }

    var n8nEmail = builder.Configuration["N8nAdmin:Email"] ?? "n8n@agoraespacios.com";
    var n8nPassword = builder.Configuration["N8nAdmin:Password"];
    var n8nNif = builder.Configuration["N8nAdmin:Nif"];

    // Usuario admin para que n8n pueda llamar a la API
    if (!string.IsNullOrWhiteSpace(n8nPassword) && !string.IsNullOrWhiteSpace(n8nNif))
    {
        var n8nAdmin = db.Usuarios.FirstOrDefault(u => u.Email == n8nEmail);

        if (n8nAdmin == null)
        {
            n8nAdmin = new Usuario
            {
                Nombre = "n8n",
                Email = n8nEmail,
                Nif = n8nNif.Trim().ToUpperInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(n8nPassword),
                Rol = "Admin"
            };

            db.Usuarios.Add(n8nAdmin);
            db.SaveChanges();
        }
        else if (n8nAdmin.Rol != "Admin")
        {
            n8nAdmin.Rol = "Admin";
            db.SaveChanges();
        }
    }
}

app.Run();
