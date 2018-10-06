﻿// <auto-generated />
using System;
using JinZhou.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JinZhou.Migrations
{
    [DbContext(typeof(JzDbContext))]
    partial class JzDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("JinZhou.Models.DbEntities.AppAuthInfo", b =>
                {
                    b.Property<string>("AppId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(80);

                    b.Property<bool>("Authorized");

                    b.Property<string>("AuthorizerAppId")
                        .HasMaxLength(80);

                    b.Property<string>("Code")
                        .HasMaxLength(160);

                    b.Property<DateTime>("CreateOn");

                    b.Property<DateTime>("ExpiredTime");

                    b.Property<DateTime>("LastUpdateOn");

                    b.HasKey("AppId");

                    b.ToTable("AppAuths");
                });

            modelBuilder.Entity("JinZhou.Models.DbEntities.BasicToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessToken")
                        .HasMaxLength(160);

                    b.Property<int>("AccessTokenExpiresIn");

                    b.Property<DateTime>("AccessTokenRefreshOn");

                    b.Property<string>("PreAuthCode")
                        .HasMaxLength(80);

                    b.Property<int>("PreAuthCodeExpiresIn");

                    b.Property<DateTime>("PreAuthCodeRefreshOn");

                    b.Property<string>("Ticket")
                        .HasMaxLength(100);

                    b.Property<DateTime>("TicketRefreshOn");

                    b.HasKey("Id");

                    b.ToTable("BasicTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
