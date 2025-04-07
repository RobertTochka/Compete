﻿// <auto-generated />
using System;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;


#nullable disable

namespace compete_poco.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240506164145_addingDbSts")]
    partial class addingDbSts
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("compete_platform.Models.Chat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Chat");
                });

            modelBuilder.Entity("compete_platform.Models.ChatMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("SendTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("UserId");

                    b.ToTable("ChatMessage");
                });

            modelBuilder.Entity("compete_platform.Models.Lobby", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<int>("MatchFormat")
                        .HasColumnType("integer");

                    b.Property<string>("PickMaps")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PlayersAmount")
                        .HasColumnType("integer");

                    b.Property<bool>("Public")
                        .HasColumnType("boolean");

                    b.Property<long>("ServerId")
                        .HasColumnType("bigint");

                    b.Property<int?>("ServerId1")
                        .HasColumnType("integer");

                    b.Property<decimal>("SingleBid")
                        .HasColumnType("numeric");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<long?>("TeamWinner")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ChatId")
                        .IsUnique();

                    b.HasIndex("ServerId1");

                    b.ToTable("Lobbies");
                });

            modelBuilder.Entity("compete_platform.Models.Match", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("LobbyId")
                        .HasColumnType("bigint");

                    b.Property<int>("PlayedMap")
                        .HasColumnType("integer");

                    b.Property<long?>("TeamId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("LobbyId");

                    b.HasIndex("TeamId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("compete_platform.Models.Server", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("compete_platform.Models.Team", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<long>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<long>("LobbyId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChatId")
                        .IsUnique();

                    b.HasIndex("LobbyId");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("compete_platform.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("SteamId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("TeamId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("compete_platform.Models.UserAward", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Award")
                        .HasColumnType("numeric");

                    b.Property<int>("AwardType")
                        .HasColumnType("integer");

                    b.Property<long>("LobbyId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("LobbyId");

                    b.HasIndex("UserId");

                    b.ToTable("UserAward");
                });

            modelBuilder.Entity("compete_platform.Models.UserBid", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Bid")
                        .HasColumnType("numeric");

                    b.Property<long>("LobbyId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("LobbyId");

                    b.HasIndex("UserId");

                    b.ToTable("Bids");
                });

            modelBuilder.Entity("compete_platform.Models.UserStat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("Assists")
                        .HasColumnType("integer");

                    b.Property<int>("Deaths")
                        .HasColumnType("integer");

                    b.Property<int>("Headshots")
                        .HasColumnType("integer");

                    b.Property<int>("Kills")
                        .HasColumnType("integer");

                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("UserId");

                    b.ToTable("UserStats");
                });

            modelBuilder.Entity("compete_platform.Models.ChatMessage", b =>
                {
                    b.HasOne("compete_platform.Models.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_platform.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("compete_platform.Models.Lobby", b =>
                {
                    b.HasOne("compete_platform.Models.Chat", "Chat")
                        .WithOne("Lobby")
                        .HasForeignKey("compete_platform.Models.Lobby", "ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_platform.Models.Server", "Server")
                        .WithMany("Lobbies")
                        .HasForeignKey("ServerId1");

                    b.OwnsOne("compete_platform.Models.ServerConfig", "Config", b1 =>
                        {
                            b1.Property<long>("LobbyId")
                                .HasColumnType("bigint");

                            b1.Property<int>("FreezeTime")
                                .HasColumnType("integer");

                            b1.Property<bool>("FriendlyFire")
                                .HasColumnType("boolean");

                            b1.HasKey("LobbyId");

                            b1.ToTable("Lobbies");

                            b1.WithOwner()
                                .HasForeignKey("LobbyId");
                        });

                    b.Navigation("Chat");

                    b.Navigation("Config")
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("compete_platform.Models.Match", b =>
                {
                    b.HasOne("compete_platform.Models.Lobby", "Lobby")
                        .WithMany("Matches")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_platform.Models.Team", "Team")
                        .WithMany("WonMatches")
                        .HasForeignKey("TeamId");

                    b.Navigation("Lobby");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("compete_platform.Models.Team", b =>
                {
                    b.HasOne("compete_platform.Models.Chat", "Chat")
                        .WithOne("Team")
                        .HasForeignKey("compete_platform.Models.Team", "ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_platform.Models.Lobby", "Lobby")
                        .WithMany("Teams")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Lobby");
                });

            modelBuilder.Entity("compete_platform.Models.User", b =>
                {
                    b.HasOne("compete_platform.Models.Team", "Team")
                        .WithMany("Users")
                        .HasForeignKey("TeamId");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("compete_platform.Models.UserAward", b =>
                {
                    b.HasOne("compete_platform.Models.Lobby", "Lobby")
                        .WithMany("Awards")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_platform.Models.User", "User")
                        .WithMany("Awards")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lobby");

                    b.Navigation("User");
                });

            modelBuilder.Entity("compete_platform.Models.UserBid", b =>
                {
                    b.HasOne("compete_platform.Models.Lobby", "Lobby")
                        .WithMany("Bids")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_platform.Models.User", "User")
                        .WithMany("Bids")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lobby");

                    b.Navigation("User");
                });

            modelBuilder.Entity("compete_platform.Models.UserStat", b =>
                {
                    b.HasOne("compete_platform.Models.Match", "Match")
                        .WithMany("Stats")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_platform.Models.User", "User")
                        .WithMany("Stats")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Match");

                    b.Navigation("User");
                });

            modelBuilder.Entity("compete_platform.Models.Chat", b =>
                {
                    b.Navigation("Lobby");

                    b.Navigation("Messages");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("compete_platform.Models.Lobby", b =>
                {
                    b.Navigation("Awards");

                    b.Navigation("Bids");

                    b.Navigation("Matches");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("compete_platform.Models.Match", b =>
                {
                    b.Navigation("Stats");
                });

            modelBuilder.Entity("compete_platform.Models.Server", b =>
                {
                    b.Navigation("Lobbies");
                });

            modelBuilder.Entity("compete_platform.Models.Team", b =>
                {
                    b.Navigation("Users");

                    b.Navigation("WonMatches");
                });

            modelBuilder.Entity("compete_platform.Models.User", b =>
                {
                    b.Navigation("Awards");

                    b.Navigation("Bids");

                    b.Navigation("Stats");
                });
#pragma warning restore 612, 618
        }
    }
}
