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
    [Migration("20240803210013_delUserVersion")]
    partial class delUserVersion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Compete_POCO_Models.Models.Pay", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Pays");
                });

            modelBuilder.Entity("Compete_POCO_Models.Models.PayEvent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<int>("PayState")
                        .HasColumnType("integer");

                    b.Property<string>("PaymentId")
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("PayEvents");
                });

            modelBuilder.Entity("compete_poco.Models.Chat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("compete_poco.Models.ChatMessage", b =>
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

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("compete_poco.Models.Lobby", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("CodeToConnect")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("EdgeConnectTimeOnFirstMap")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastServerUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MapActions")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MatchFormat")
                        .HasColumnType("integer");

                    b.Property<string>("PickMaps")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PlayersAmount")
                        .HasColumnType("integer");

                    b.Property<bool>("Public")
                        .HasColumnType("boolean");

                    b.Property<int>("ServerId")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<long?>("TeamWinner")
                        .HasColumnType("bigint");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<TimeSpan>("WaitToStartTime")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.HasIndex("ChatId")
                        .IsUnique();

                    b.HasIndex("CreatorId");

                    b.HasIndex("ServerId");

                    b.ToTable("Lobbies");
                });

            modelBuilder.Entity("compete_poco.Models.Match", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<short>("FirstTeamScore")
                        .HasColumnType("smallint");

                    b.Property<long>("LobbyId")
                        .HasColumnType("bigint");

                    b.Property<int>("PlayedMap")
                        .HasColumnType("integer");

                    b.Property<short>("SecondTeamScore")
                        .HasColumnType("smallint");

                    b.Property<long?>("TeamId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("LobbyId");

                    b.HasIndex("TeamId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("compete_poco.Models.Server", b =>
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

                    b.Property<string>("PlayingPorts")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("compete_poco.Models.Team", b =>
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

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("compete_poco.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("AvatarUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastSteamInfoUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Rate")
                        .HasColumnType("double precision");

                    b.Property<long?>("RatePlace")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("SteamId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SteamId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("compete_poco.Models.UserAward", b =>
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

                    b.Property<DateTime>("PayTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("LobbyId");

                    b.HasIndex("UserId");

                    b.ToTable("Awards");
                });

            modelBuilder.Entity("compete_poco.Models.UserBid", b =>
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

            modelBuilder.Entity("compete_poco.Models.UserStat", b =>
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

            modelBuilder.Entity("TeamUser", b =>
                {
                    b.Property<long>("TeamsId")
                        .HasColumnType("bigint");

                    b.Property<long>("UsersId")
                        .HasColumnType("bigint");

                    b.HasKey("TeamsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("TeamUser");
                });

            modelBuilder.Entity("Compete_POCO_Models.Models.Pay", b =>
                {
                    b.HasOne("compete_poco.Models.User", "User")
                        .WithMany("Pays")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Compete_POCO_Models.Models.PayEvent", b =>
                {
                    b.HasOne("compete_poco.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("compete_poco.Models.ChatMessage", b =>
                {
                    b.HasOne("compete_poco.Models.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_poco.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("compete_poco.Models.Lobby", b =>
                {
                    b.HasOne("compete_poco.Models.Chat", "Chat")
                        .WithOne("Lobby")
                        .HasForeignKey("compete_poco.Models.Lobby", "ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_poco.Models.User", "Creator")
                        .WithMany("OwnedLobbies")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_poco.Models.Server", "Server")
                        .WithMany("Lobbies")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("compete_poco.Models.ServerConfig", "Config", b1 =>
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

                    b.Navigation("Creator");

                    b.Navigation("Server");
                });

            modelBuilder.Entity("compete_poco.Models.Match", b =>
                {
                    b.HasOne("compete_poco.Models.Lobby", "Lobby")
                        .WithMany("Matches")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_poco.Models.Team", "Team")
                        .WithMany("WonMatches")
                        .HasForeignKey("TeamId");

                    b.Navigation("Lobby");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("compete_poco.Models.Team", b =>
                {
                    b.HasOne("compete_poco.Models.Chat", "Chat")
                        .WithOne("Team")
                        .HasForeignKey("compete_poco.Models.Team", "ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_poco.Models.Lobby", "Lobby")
                        .WithMany("Teams")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Lobby");
                });

            modelBuilder.Entity("compete_poco.Models.UserAward", b =>
                {
                    b.HasOne("compete_poco.Models.Lobby", "Lobby")
                        .WithMany("Awards")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_poco.Models.User", "User")
                        .WithMany("Awards")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lobby");

                    b.Navigation("User");
                });

            modelBuilder.Entity("compete_poco.Models.UserBid", b =>
                {
                    b.HasOne("compete_poco.Models.Lobby", "Lobby")
                        .WithMany("Bids")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_poco.Models.User", "User")
                        .WithMany("Bids")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lobby");

                    b.Navigation("User");
                });

            modelBuilder.Entity("compete_poco.Models.UserStat", b =>
                {
                    b.HasOne("compete_poco.Models.Match", "Match")
                        .WithMany("Stats")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_poco.Models.User", "User")
                        .WithMany("Stats")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Match");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TeamUser", b =>
                {
                    b.HasOne("compete_poco.Models.Team", null)
                        .WithMany()
                        .HasForeignKey("TeamsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("compete_poco.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("compete_poco.Models.Chat", b =>
                {
                    b.Navigation("Lobby");

                    b.Navigation("Messages");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("compete_poco.Models.Lobby", b =>
                {
                    b.Navigation("Awards");

                    b.Navigation("Bids");

                    b.Navigation("Matches");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("compete_poco.Models.Match", b =>
                {
                    b.Navigation("Stats");
                });

            modelBuilder.Entity("compete_poco.Models.Server", b =>
                {
                    b.Navigation("Lobbies");
                });

            modelBuilder.Entity("compete_poco.Models.Team", b =>
                {
                    b.Navigation("WonMatches");
                });

            modelBuilder.Entity("compete_poco.Models.User", b =>
                {
                    b.Navigation("Awards");

                    b.Navigation("Bids");

                    b.Navigation("OwnedLobbies");

                    b.Navigation("Pays");

                    b.Navigation("Stats");
                });
#pragma warning restore 612, 618
        }
    }
}
