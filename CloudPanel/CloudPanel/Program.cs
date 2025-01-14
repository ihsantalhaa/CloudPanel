using Amazon.CloudWatchLogs;
using Amazon.S3;
using CloudPanel.WebApi.Dtos.UserDtos;
using CloudPanel.WebApi.Models.DapperContext;
using CloudPanel.WebApi.Repositories.AuthRepository;
using CloudPanel.WebApi.Repositories.BucketRepository;
using CloudPanel.WebApi.Repositories.FileRepository;
using CloudPanel.WebApi.Repositories.GroupFileRepository;
using CloudPanel.WebApi.Repositories.GroupRepository;
using CloudPanel.WebApi.Repositories.GroupUserRepository;
using CloudPanel.WebApi.Repositories.LogRepository;
using CloudPanel.WebApi.Repositories.RoleRepository;
using CloudPanel.WebApi.Repositories.RoleUserRepository;
using CloudPanel.WebApi.Repositories.UserRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, logger) => logger.WriteTo.Console().ReadFrom.Configuration(builder.Configuration));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<Context>();
builder.Services.AddTransient<IFileRepository, FileRepository>();
builder.Services.AddTransient<IGroupRepository, GroupRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IRoleUserRepository, RoleUserRepository>();
builder.Services.AddTransient<IGroupUserRepository, GroupUserRepository>();
builder.Services.AddTransient<IGroupFileRepository, GroupFileRepository>();
builder.Services.AddTransient<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IBucketRepository, BucketRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

builder.Services.AddAWSService<IAmazonCloudWatchLogs>();


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
