using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MPTC_API.Controllers.Attendance;

using MPTC_API.Data;
using MPTC_API.Hub;
using MPTC_API.Models.Attendance;
using MPTC_API.Services;
using MPTC_API.Services.Attendance;
using MPTC_API.Services.Authentication;

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

//Add db context for postgresql
builder.Services.AddDbContext<MptcContext>(options =>
    options.UseNpgsql("Host=localhost;Database=mptc_db;Username=postgres;Password=root;"));

// Add services to the container.
// Add Identity services
builder.Services.AddIdentity<Member, IdentityRole>(options =>
{
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
        options.User.RequireUniqueEmail = true;

})
    .AddEntityFrameworkStores<MptcContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1); // Set your desired expiration time
});
    
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder
            .WithOrigins("http://localhost:3000") // Allow frontend origin
            .AllowCredentials() // Allow credentials
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<RecognitionService>();
builder.Services.AddSingleton<GlobalService>();
builder.Services.AddSingleton<ClockInController>(); // Register ClockInController
builder.Services.AddSingleton<ClockOutController>(); // Register ClockOutController

//register mongodb service
// Register MongoDB client
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var connectionString = "mongodb://localhost:27017/MPTC_db"; // MongoDB connection string
    return new MongoClient(connectionString);
});
builder.Services.AddHostedService<CameraStreamingService>(); // Register the background service
builder.Services.AddScoped<AttendanceService>();

builder.Services.AddSignalR(); // Add SignalR service






var app = builder.Build();

app.UseCors("AllowLocalhost"); // Enable the CORS policy


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(2),
};
app.UseWebSockets(webSocketOptions);

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();
app.UseRouting(); 
app.UseEndpoints(endpoints =>
{
    // Map the SignalR hub to an endpoint
    endpoints.MapHub<AttendanceHub>("/attendanceHub");

    // Other endpoints...
});

app.Run();


public class CameraStreamingService : BackgroundService
{
    private readonly ClockInController _clockInController;
    private readonly ClockOutController _clockOutController;


    public CameraStreamingService(ClockInController cameraController, ClockOutController clockOutController)
    {
        _clockInController = cameraController;
        _clockOutController = clockOutController;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Automatically start streaming when the service starts
        Task.Run( () => _clockInController.Index());
        Task.Run( () => _clockOutController.Index());
    }
}

