drop database if exists dbLeitour;
create database dbLeitour;
use dbLeitour;

create table tbUser(
	UserId int primary key auto_increment,
    NameUser varchar(20) not null,
    PasswordUser varchar(20) not null,
    Email varchar(50) not null unique,
    profilePhoto blob,
    theme decimal(1) not null default 0,
    roleUser int not null default 0,
    activeUser tinyint not null default 1
);

create table tbPost(
	postId int primary key auto_increment,
    UserId int not null,
    foreign key(UserId) references tbUser(UserId),
    PageId int,
    -- foreign key(UserId) references tbUser(UserId),
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

insert into tbUser(NameUser,PasswordUser,Email) values('Lucas','12345','lucas@gmail.com');
insert into tbUser(NameUser,PasswordUser,Email) values('Daniel','12345','Daniel@gmail.com');
insert into tbUser(NameUser,PasswordUser,Email) values('Banana','12345','Banana@gmail.com');
select * from tbUser;