using FileConsumer.Services.Authorization;
using FileConsumer.Services.DocumentService;
using FileConsumer.Services.Validators;
using FileConsumer.Utilities;
using FileConsumer.Utilities.Mappers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using System.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

#region Configure Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileConsumer API", Version = "v1" });
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Basic scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});


#endregion

#region Dependency Injection

builder.Services.AddAuthentication("BasicAuthentication")
.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddSingleton<ResourceManager>(provider =>
{
    return new ResourceManager("FileConsumer.MessagesResource", typeof(Program).Assembly);
});

builder.Services.Configure<BasicAuthSettings>(builder.Configuration.GetSection("BasicAuthSettings"));
builder.Services.AddSingleton<IUserMessagesProvider, UserMessagesProvider>();
builder.Services.AddScoped<IDocumentMapper, DocumentMapper>();
builder.Services.AddScoped<IDocumentService, DocumentServiceRecursive>();
//builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddTransient<IFileValidatorService, FileValidatorService>();

#endregion

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
}

app.MapControllers();
app.Run();