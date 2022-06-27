﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using gameSwapCSharp.Models;

#nullable disable

namespace gameSwapCSharp.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20220624191513_ThirdMigration")]
    partial class ThirdMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("gameSwapCSharp.Models.Game", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Platform")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<string>("Quality")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("gameSwapCSharp.Models.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("ProposedPrice")
                        .HasColumnType("int");

                    b.Property<int>("RecipientId")
                        .HasColumnType("int");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.HasKey("MessageId");

                    b.HasIndex("GameId");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("gameSwapCSharp.Models.Response", b =>
                {
                    b.Property<int>("ResponseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<int>("ProposedPrice")
                        .HasColumnType("int");

                    b.Property<string>("ResponseContent")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ResponseId");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("Responses");
                });

            modelBuilder.Entity("gameSwapCSharp.Models.Swap", b =>
                {
                    b.Property<int>("SwapId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BuyerId")
                        .HasColumnType("int");

                    b.Property<int>("FinalPrice")
                        .HasColumnType("int");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<int>("SellerId")
                        .HasColumnType("int");

                    b.Property<string>("TrackingInfo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("SwapId");

                    b.HasIndex("BuyerId");

                    b.HasIndex("GameId")
                        .IsUnique();

                    b.HasIndex("SellerId");

                    b.ToTable("Swaps");
                });

            modelBuilder.Entity("gameSwapCSharp.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Coins")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PassConfirm")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("gameSwapCSharp.Models.Game", b =>
                {
                    b.HasOne("gameSwapCSharp.Models.User", null)
                        .WithMany("OwnedGames")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("gameSwapCSharp.Models.Message", b =>
                {
                    b.HasOne("gameSwapCSharp.Models.Game", null)
                        .WithMany("Inquiries")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("gameSwapCSharp.Models.User", "Recipient")
                        .WithMany("ReceivedMessages")
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("gameSwapCSharp.Models.User", "Sender")
                        .WithMany("SentMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipient");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("gameSwapCSharp.Models.Response", b =>
                {
                    b.HasOne("gameSwapCSharp.Models.Message", null)
                        .WithMany("Responses")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("gameSwapCSharp.Models.User", null)
                        .WithMany("SentResponses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("gameSwapCSharp.Models.Swap", b =>
                {
                    b.HasOne("gameSwapCSharp.Models.User", "Buyer")
                        .WithMany("BoughtSwaps")
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("gameSwapCSharp.Models.Game", "Game")
                        .WithOne("Swap")
                        .HasForeignKey("gameSwapCSharp.Models.Swap", "GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("gameSwapCSharp.Models.User", "Seller")
                        .WithMany("OwnedSwaps")
                        .HasForeignKey("SellerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Buyer");

                    b.Navigation("Game");

                    b.Navigation("Seller");
                });

            modelBuilder.Entity("gameSwapCSharp.Models.Game", b =>
                {
                    b.Navigation("Inquiries");

                    b.Navigation("Swap")
                        .IsRequired();
                });

            modelBuilder.Entity("gameSwapCSharp.Models.Message", b =>
                {
                    b.Navigation("Responses");
                });

            modelBuilder.Entity("gameSwapCSharp.Models.User", b =>
                {
                    b.Navigation("BoughtSwaps");

                    b.Navigation("OwnedGames");

                    b.Navigation("OwnedSwaps");

                    b.Navigation("ReceivedMessages");

                    b.Navigation("SentMessages");

                    b.Navigation("SentResponses");
                });
#pragma warning restore 612, 618
        }
    }
}
