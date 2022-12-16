using IdentityJwtPoc.API.Configuration;
using IdentityJwtPoc.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddDbContextConfiguration(builder.Configuration);
builder.Services.AddDIConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddAuthenticationConfiguration(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCustomJwtMiddleware();
app.MapControllers();
app.Run();
