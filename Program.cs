using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Services;
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
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<RecognitionService>();
//add Recognition service







var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
