Drop database if exists strikeo_admin ;
create database strikeo_admin;
use strikeo_admin;

create table equipe (
     idequipe int not null auto_increment primary key,
     nom_equipe varchar(50) not null,
     nb_joueur int not null
);

create table joueur (
    idjoueur int not null auto_increment primary key,
    nom_joueur varchar(50) not null,
    prenom_joueur varchar(50) not null,
    age_joueur int not null,
    mail_joueur varchar(50) not null,
    telephone varchar(20) not null,

    idequipe int null,
    foreign key (idequipe) references equipe (idequipe)
 );



create table tournoi (
    idtournoi int not null auto_increment primary key,
    designation varchar (100),
    date_tournoi date not null,
    description text
);

create table participation (
    idparticipation int not null auto_increment primary key,
    date_inscription date not null, 
    statut enum ("en attente", "confirmee", "annulee"),

    idtournoi int not null,
    foreign key (idtournoi) references tournoi (idtournoi),

    idequipe int not null,
    foreign key (idequipe) references equipe (idequipe)
);

-- Table des administrateurs
create table admin (
    idadmin int not null auto_increment primary key,
    identifiant varchar(50) not null unique,
    mot_de_passe varchar(255) not null,
    nom_admin varchar(100),
    date_creation datetime default current_timestamp
);