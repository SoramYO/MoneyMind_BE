//=============MONEYMIND==============

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using Microsoft.OpenApi.Models;
using MoneyMind_API.Middlewares;
using MoneyMind_BLL.Mapping;
using MoneyMind_BLL.Services.BackgroundServices;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var mlContext = new MLContext();
var modelPath = Path.Combine(AppContext.BaseDirectory, "transaction_model.zip");
var loadedModel = mlContext.Model.Load(modelPath, out _);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MoneyMind_API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Add services to the container.
builder.Services.AddDbContext<MoneyMindDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MoneyMindConnectionString"),
        b => b.MigrationsAssembly("MoneyMind_DAL")));
builder.Services.AddDbContext<MoneyMindAuthDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MoneyMindAuthConnectionString"),
        b => b.MigrationsAssembly("MoneyMind_DAL")));

builder.Services.AddHttpContextAccessor();

//Initial Model AI
builder.Services.AddSingleton(mlContext);
builder.Services.AddSingleton(loadedModel);

//Background Service
builder.Services.AddHostedService<SheetSyncService>();
//Service
builder.Services.Scan(scan => scan
    .FromAssemblyOf<IActivityService>()
    .AddClasses(classes => classes.InNamespaces(
        "MoneyMind_BLL.Services.Implementations"
    ))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);


// Repositories
builder.Services.Scan(scan => scan
    .FromAssemblyOf<IActivityRepository>()
    .AddClasses(classes => classes.InNamespaces(
        "MoneyMind_DAL.Repositories.Implementations"
    ))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);


// AutoMapper configuration
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>() 
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("MoneyMind") 
    .AddEntityFrameworkStores<MoneyMindAuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedEmail = true;
});

// JWT Authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add MB Bank sync services
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates")),
    RequestPath = "/Templates"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets")),
    RequestPath = "/Assets"
});

app.UseCors("AllowAll");

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();
