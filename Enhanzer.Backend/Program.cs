using Enhanzer.Backend.Models;
using Enhanzer.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Default Angular CLI port
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure HttpClient for AuthService
builder.Services.AddHttpClient<IAuthService, AuthService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Ensure Location_Details exists if DB is completely new
    dbContext.Database.EnsureCreated();
    
    // Safely create Purchase Bill tables if they don't exist yet
    dbContext.Database.ExecuteSqlRaw(@"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PurchaseBills' and xtype='U')
        BEGIN
            CREATE TABLE PurchaseBills (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                CreatedAt DATETIME2 NOT NULL,
                TotalItems INT NOT NULL,
                TotalQuantity INT NOT NULL,
                TotalCost DECIMAL(18,2) NOT NULL,
                TotalSelling DECIMAL(18,2) NOT NULL
            );
        END
        
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PurchaseBillItems' and xtype='U')
        BEGIN
            CREATE TABLE PurchaseBillItems (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                PurchaseBillId INT NOT NULL FOREIGN KEY REFERENCES PurchaseBills(Id) ON DELETE CASCADE,
                ItemName NVARCHAR(MAX) NOT NULL,
                Batch NVARCHAR(MAX) NOT NULL,
                StandardCost DECIMAL(18,2) NOT NULL,
                StandardPrice DECIMAL(18,2) NOT NULL,
                Margin DECIMAL(18,2) NOT NULL,
                Qty INT NOT NULL,
                FreeQty INT NOT NULL,
                Discount DECIMAL(18,2) NOT NULL,
                TotalCost DECIMAL(18,2) NOT NULL,
                TotalSelling DECIMAL(18,2) NOT NULL
            );
        END
    ");
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
