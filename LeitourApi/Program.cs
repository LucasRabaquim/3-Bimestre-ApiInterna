using LeitourApi.Models;
using LeitourApi.Settings;
using LeitourApi.Services.UserService;
using LeitourApi.Services.PageService;
using LeitourApi.Services.PostService;
using LeitourApi.Services.MsgActionResult;
using LeitourApi.Services.BookApiService;
using LeitourApi.Services.AnnotationService;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using LeitourApi.Services.FollowService;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(AuthSettings.Secret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


var docker = Environment.GetEnvironmentVariable("DOCKER_RUNNING");
Console.WriteLine("teste \n"+docker+"\n teste");
Database db = new();
builder.Services.AddDbContext<LeitourContext>(
    options => { 
     if(docker == "true")
            options.UseMySQL("Server=mysql;port=3306;Database=dbLeitour;User Id=root;Password=12345678;");
        else
            options.UseMySQL("Server=localhost;port=3306;Database=dbLeitour;User Id=root;Password=12345678;");
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

MySqlConnection conn;
if(docker == "true")
    conn = db.Connection("Server=mysql;port=3306;Database=dbLeitour;User Id=root;Password=12345678;");
else
    conn = db.Connection("Server=localhost;port=3306;Database=dbLeitour;User Id=root;Password=12345678;");

conn.Open();
db.CreateDb(conn);
conn.Close();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<MsgActionResultService>();
builder.Services.AddScoped<BookApiService>();
builder.Services.AddScoped<IAnnotationService, AnnotationService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPageService, PageService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
