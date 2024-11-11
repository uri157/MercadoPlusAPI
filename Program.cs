using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


var builder = WebApplication.CreateBuilder(args);

try
{
    // Configuración de CORS para permitir el origen del servidor frontend con SSL
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowHttpServerOrigin", policy =>
        {
            policy.WithOrigins("https://34.229.139.218/", "https://mercadoplus.xyz") // Asegúrate de incluir "https" para SSL
                  .AllowAnyMethod()                         // Permite todos los métodos HTTP
                  .AllowAnyHeader()                         // Permite todos los encabezados
                  .AllowCredentials();                      // Permite el uso de credenciales
        });
    });

    // Agregamos los controladores
    builder.Services.AddControllers();

    // Configuración de Swagger para la documentación de la API
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "MercadoLibre API", Version = "v1" });

        // Configuración de seguridad para JWT
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Introduce el token JWT en este formato: Bearer {token}"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    // Configuración del contexto de base de datos
    builder.Services.AddSqlServer<DbContext>(builder.Configuration.GetConnectionString("cnMercadoLibre"));

    // Inyección de dependencias para los servicios específicos de la aplicación
    builder.Services.AddScoped<IAccountService, AccountDbService>();
    builder.Services.AddScoped<ICardService, CardDbService>();
    builder.Services.AddScoped<ICardTypeService, CardTypeDbService>();
    builder.Services.AddScoped<ICategoryService, CategoryDbService>();
    builder.Services.AddScoped<INotificationService, NotificationDbService>();
    builder.Services.AddScoped<IProductStateService, ProductStateDbService>();
    builder.Services.AddScoped<IPublicationStateService, PublicationStateDbService>();
    builder.Services.AddScoped<IPhotoService, PhotoDbService>();
    builder.Services.AddScoped<IColorService, ColorDbService>();
    builder.Services.AddScoped<IPhotoPublicationService, PhotoPublicationDbService>();
    builder.Services.AddScoped<ITransactionService, TransactionDbService>();
    builder.Services.AddScoped<IShoppingCartService, ShoppingCartDbService>();
    builder.Services.AddScoped<IPublicationVisitedService, PublicationVisitedDbService>();
    builder.Services.AddScoped<IMailService, MailService>();
    // builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>();
    builder.Services.AddScoped<IPublicationService, PublicationDbService>();
    builder.Services.AddScoped<IWishedArticleService, WishedArticleDbService>();



    // Configuración de Identity para autenticación y autorización
    builder.Services.AddDbContext<DbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("cnMercadoLibre")));

    builder.Services.AddIdentity<User, IdentityRole<int>>()
        .AddEntityFrameworkStores<DbContext>()
        .AddDefaultTokenProviders();

    // Configuración de JWT para autenticación
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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

    var app = builder.Build();
    // Redirecciona automáticamente las solicitudes HTTP a HTTPS
    app.UseHttpsRedirection();

    // Configuración de Swagger
    app.UseSwagger();
    app.UseSwaggerUI();

    // Aplica la política de CORS configurada anteriormente
    app.UseCors("AllowHttpServerOrigin");

    // Configuración de autenticación y autorización
    app.UseAuthentication();
    app.UseAuthorization();

    // Mapeo de controladores
    app.MapControllers();

    // Crear roles al iniciar la aplicación
    // await CreateRoles(app.Services);

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

// // Función para crear roles
// async Task CreateRoles(IServiceProvider serviceProvider)
// {
//     using (var scope = serviceProvider.CreateScope())
//     {
//         var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
//         string[] roleNames = { "admin", "User" };
//         foreach (var roleName in roleNames)
//         {
//             if (!await roleManager.RoleExistsAsync(roleName))
//             {
//                 await roleManager.CreateAsync(new IdentityRole<int>(roleName));
//             }
//         }
//     }
// }
