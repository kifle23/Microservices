using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureDatabase(builder.Services, builder.Configuration, builder.Environment);
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();

static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
{
    if (environment.IsProduction())
    {
        var connectionString = configuration.GetConnectionString("PlatformsConn");
        Console.WriteLine($"--> Using SqlServer Db with ConnectionString: {connectionString}");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
    }
    else
    {
        Console.WriteLine("--> Using InMem Db");
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));
    }
}
