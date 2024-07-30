using pdf_search.Data;
using pdf_search.Services.Interfaces;
using pdf_search.Services.Implmentations;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("PsqlConnectionInfo");
var logger = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
}).CreateLogger<Program>();

logger.LogInformation("Using connection string: {ConnectionString}", connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("PsqlConnectionInfo"))
);
builder.Services.AddScoped<ISubmissionService, SubmissionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAllOrigins");

app.Use(async (context, next) =>
{

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
    

    await next.Invoke();
    
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
