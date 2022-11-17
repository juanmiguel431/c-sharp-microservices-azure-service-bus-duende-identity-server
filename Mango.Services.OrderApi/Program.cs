using Mango.MessageBus;
using Mango.Services.OrderApi;
using Mango.Services.OrderApi.DbContexts;
using Mango.Services.OrderApi.Extensions;
using Mango.Services.OrderApi.Messaging;
using Mango.Services.OrderApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Mango.services.OrderApi", Description = "Order Api", Version = "V1"});
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = $"Enter 'Bearer' [space] and your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    var openApiSecurityScheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        },
        Scheme = "OAuth2",
        Name = "Bearer",
        In = ParameterLocation.Header
    };
    
    var openApiSecurityRequirement = new OpenApiSecurityRequirement { { openApiSecurityScheme, new List<string>() } };
    
    c.AddSecurityRequirement(openApiSecurityRequirement);
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.Authority = builder.Configuration["ServiceUrls:IdentityApi"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "mango");
    });
});

var mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddHostedService<RabbitMqConsumer>();

builder.Services.AddSingleton<IOrderRepository>(new OrderRepository(optionBuilder.Options));
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

var serviceBusConnectionString = builder.Configuration["AzureServiceBus:ConnectionString"];
builder.Services.AddSingleton<IMessageBus>(new AzureServiceBusMessageBus(serviceBusConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseAzureServiceBusConsumer();

app.Run();