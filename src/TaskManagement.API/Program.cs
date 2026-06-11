// using Microsoft.EntityFrameworkCore;
// using TaskManagement.API.Extensions;
// using TaskManagement.API.Middleware;
// using TaskManagement.Infrastructure.Data;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddApplicationServices(builder.Configuration);

// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();

// // Swagger
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new()
//     {
//         Title   = "Task Management API",
//         // Version = "v1",
//         // Description = "A Task Management System with Users, Projects, Tasks and Comments."
//     });

//     // Include XML comments from controllers (summary tags)
//     var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
//     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//     if (File.Exists(xmlPath))
//         options.IncludeXmlComments(xmlPath);
// });
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//     });
// });

// var app = builder.Build();

// app.UseStaticFiles();  // make sure this is present
// app.UseSwagger();

// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
//     c.RoutePrefix = string.Empty;
//     c.DocumentTitle = "Task Manager";
//     c.InjectStylesheet("/swagger-ui/custom.css");
// });


// app.UseCors("AllowAll");

// app.UseMiddleware<ExceptionHandlingMiddleware>();

// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate(); // dotnet ef xelit wera rom ar mogviwios
// }

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
//         c.RoutePrefix = string.Empty; // Swagger UI at root: https://localhost:PORT/
//     });
// }

// app.UseHttpsRedirection();
// app.UseAuthorization();
// app.MapControllers();

// app.Run();

// public partial class Program { }
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Extensions;
using TaskManagement.API.Middleware;
using TaskManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Task Management API" });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseStaticFiles(); // serve wwwroot/custom.css etc.
app.UseSwagger();     // generate JSON endpoint

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
    c.RoutePrefix = string.Empty; // UI at root
    c.DocumentTitle = "Task Manager";
    c.InjectStylesheet("/swagger-ui/custom.css"); // place file at wwwroot/swagger-ui/custom.css
});

app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
