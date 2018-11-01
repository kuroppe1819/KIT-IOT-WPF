drop database if exists kobayashi_iot;
drop user if exists sopackuser@localhost;
create database kobayashi_iot;
use kobayashi_iot;
create user sopackuser@localhost identified by 'sopa-0001';
grant all on kobayashi_iot.* to sopackuser@localhost;
create table current (
    id bigint unsigned not null primary key auto_increment,
    area int unsigned not null,
    datetime datetime not null,
    current_switch boolean not null default 0
);
create table environment (
    id bigint unsigned not null primary key auto_increment,
    area int unsigned not null,
    datetime datetime not null,
    temperature int,
    humidity int,
    illumination int
);
create table dust (
    id bigint unsigned not null primary key auto_increment,
    area int unsigned not null,
    datetime datetime not null,
    dust int
);
