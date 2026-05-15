using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ETLService.Security;
using ETLService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// =============================
// Servicios básicos
// =============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// =============================
// JWT SERVICE
// =============================
builder.Services.AddScoped<JwtService>();
builder.Services.AddSingleton<DbHelper>();

// =============================
// CONFIGURACIÓN JWT
// =============================
var keyString = builder.Configuration["JwtSettings:Key"];

if (string.IsNullOrEmpty(keyString))
{
    throw new Exception("JWT Key no está configurada en appsettings.json");
}

var key = Encoding.UTF8.GetBytes(keyString);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ClockSkew = TimeSpan.Zero
    };
});

// =============================
// SWAGGER CON JWT
// =============================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Seguridad",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese el token así: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// =============================
// CORS
// =============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// =============================
// Swagger
// =============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =============================
// Middleware
// =============================
app.UseHttpsRedirection();

app.UseCors("AllowAll");

//TRACKING 
app.UseMiddleware<RequestTrackingMiddleware>();

//AUTH
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();