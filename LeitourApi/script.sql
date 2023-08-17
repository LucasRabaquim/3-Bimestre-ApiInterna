drop database if exists dbLeitour;
create database dbLeitour;
use dbLeitour;

create table tbUser(
	userId int primary key auto_increment,
    nameUser varchar(20) not null,
    email varchar(50) not null,
    profilePhoto blob,
    theme decimal(1) not null default 0,
    roleUser Enum('common','admin') not null default 'common',
    activeUser tinyint not null default 1
);

create table tbPost(
	postId int primary key auto_increment,
    userId int,
    foreign key(userId) references tbUser(UserId),
    messagePost varchar(280) not null,
    likes int not null default 0,
    media blob,
    postDate datetime default CURRENT_TIMESTAMP,
    alteratedDate datetime
);
