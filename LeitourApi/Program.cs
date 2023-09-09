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

string sqlConnectionString = "Server=mysql;port=3306;Database=dbLeitour;User Id=root;Password=12345678;";

builder.Services.AddDbContext<LeitourContext>(
    options => options.UseMySQL(sqlConnectionString)
);

string script = @"drop database if exists dbLeitour;
create database dbLeitour;
use dbLeitour;

create table tbUser(
	UserId int primary key auto_increment,
    NameUser varchar(20) not null,
    PasswordUser varchar(20) not null,
    Email varchar(50) not null unique,
    profilePhoto blob,
    theme decimal(1) not null default 0,
    roleUser int(3) not null default 0,
    activeUser tinyint not null default 1
);

create table tbPage(
    PageId int primary key auto_increment,
    NamePage varchar(20) not null,
    DescriptionPage varchar(500),
    Cover blob,
    CreatedDate datetime default CURRENT_TIMESTAMP,
    alteratedDate datetime,
    ActivePage tinyint not null default 1
);

create table tbFollowingPage(
    UserId int not null,
    PageId int not null,
    primary key(UserId,PageId),
    foreign key(PageId) references tbPage(PageId),
    foreign key(UserId) references tbUser(UserId),
    RoleUser int(3) not null default 0
);

create table tbPost(
	postId int primary key auto_increment,
    UserId int not null,
    foreign key(UserId) references tbUser(UserId),
    PageId int,
    foreign key(PageId) references tbPage(PageId),
    messagePost varchar(280) not null,
    likes int not null default 0,
    media blob,
    postDate datetime default CURRENT_TIMESTAMP,
    alteratedDate datetime
);

create table tbFollowingList(
    UserId int not null,
    foreign key(UserId) references tbUser(UserId),
    FollowingEmail varchar(50) not null,
    foreign key(FollowingEmail) references tbUser(Email)
);

create table tbBookPage(
    BookKey varchar(25) not null,
    PageId int not null,
    primary key(BookKey,PageId),
    foreign key(PageId) references tbPage(PageId)
);

create table tbSavedBook(
    SavedId int primary key auto_increment,
    UserId int not null,
    foreign key(UserId) references tbUser(UserId),
    BookKey varchar(25) not null,
    public tinyint not null default 0
);

create table tbAnnotation(
    AnnotationId int primary key auto_increment,
    SavedId int not null,
    foreign key(SavedId) references tbSavedBook(SavedId),
    AnnotationText varchar(250) not null,
    alteratedDate datetime not null default CURRENT_TIMESTAMP
);

insert into tbUser(NameUser,PasswordUser,Email) values('Lucas','12345','Lucas@gmail.com');
insert into tbUser(NameUser,PasswordUser,Email) values('Daniel','12345','Daniel@gmail.com');
insert into tbUser(NameUser,PasswordUser,Email) values('Luiz','12345','Luiz@gmail.com');

insert into tbPost(UserId,messagePost) values (1,'Primeira vez usando a plataforma');
insert into tbPost(UserId,messagePost,likes) values (2,'Queria doar alguns livros, alguem tem interesse?',10);

insert into tbFollowingList values (1,'Daniel@gmail.com');
insert into tbFollowingList values (1,'Luiz@gmail.com');
insert into tbFollowingList values (2,'Lucas@gmail.com');

insert into tbPage(NamePage, DescriptionPage) values ('Potter Heads','Uma pagina para a discussão de todos os livros da série harry potter');
insert into tbFollowingPage values(3,1,2);
insert into tbFollowingPage values(1,1,1);
insert into tbFollowingPage values(2,1,0);

insert into tbBookPage values('GjgQCwAAQBAJ',1);
insert into tbBookPage values('qrv6zwEACAAJ',1);
insert into tbBookPage values('qDYQCwAAQBAJ',1);
insert into tbBookPage values('rUIJPwAACAAJ',1);
insert into tbBookPage values('rkr7zwEACAAJ',1);
insert into tbBookPage values('yjUQCwAAQBAJ',1);

insert into tbPost(PageId,UserId,messagePost,likes) values (1,2,'Alguem sabe de uma livraria onde vendem todos os livros?',2);";//File.ReadAllText(@"./src/LeitourApi/db/script.sql");

MySqlConnection conn = new(sqlConnectionString);

conn.Open();
//Server server = new Server(new ServerConnection(conn));
MySqlCommand createDb = new(script, conn);
createDb.ExecuteNonQuery();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
