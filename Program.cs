using Microsoft.EntityFrameworkCore;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.OpenApi.Models;
using Stopwatch;

var builder = WebApplication.CreateBuilder();

var jwt = builder.Configuration.GetSection(nameof(JwtConfiguration));
builder.Services.Configure<JwtConfiguration>(jwt);

builder.Services
    .AddAuthenticationJwtBearer(s => s.SigningKey = jwt.Get<JwtConfiguration>()!.SigningKey)
    .AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdministratorRole", policy =>
            policy.RequireRole("Administrator"));
    })
    .AddFastEndpoints()
    .SwaggerDocument();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Working hours tracker"));
}

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Endpoints.RoutePrefix = "api";
    });

app.Run();

