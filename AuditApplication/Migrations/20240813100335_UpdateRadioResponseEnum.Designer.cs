﻿// <auto-generated />
using System;
using AuditApplication.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AuditApplication.Migrations
{
    [DbContext(typeof(AuditContext))]
    [Migration("20240813100335_UpdateRadioResponseEnum")]
    partial class UpdateRadioResponseEnum
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-preview.6.24327.4");

            modelBuilder.Entity("AuditApplication.Models.Audit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AuditName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CompletionDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Audits");
                });

            modelBuilder.Entity("AuditApplication.Models.AuditQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuditSectionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuditSectionId");

                    b.ToTable("AuditQuestions");
                });

            modelBuilder.Entity("AuditApplication.Models.AuditSection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuditId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AuditId");

                    b.ToTable("AuditSections");
                });

            modelBuilder.Entity("AuditApplication.Models.AuditTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AuditTemplates");
                });

            modelBuilder.Entity("AuditApplication.Models.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SectionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("AuditApplication.Models.QuestionResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AttachmentPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("AuditId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuestionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RadioAnswer")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TextAnswer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuditId");

                    b.HasIndex("QuestionId");

                    b.ToTable("QuestionResponses");
                });

            modelBuilder.Entity("AuditApplication.Models.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuditTemplateId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AuditTemplateId");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("AuditApplication.Models.AuditQuestion", b =>
                {
                    b.HasOne("AuditApplication.Models.AuditSection", null)
                        .WithMany("Questions")
                        .HasForeignKey("AuditSectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuditApplication.Models.AuditSection", b =>
                {
                    b.HasOne("AuditApplication.Models.Audit", null)
                        .WithMany("Sections")
                        .HasForeignKey("AuditId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuditApplication.Models.Question", b =>
                {
                    b.HasOne("AuditApplication.Models.Section", "Section")
                        .WithMany("Questions")
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Section");
                });

            modelBuilder.Entity("AuditApplication.Models.QuestionResponse", b =>
                {
                    b.HasOne("AuditApplication.Models.Audit", "Audit")
                        .WithMany("QuestionResponses")
                        .HasForeignKey("AuditId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuditApplication.Models.Question", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Audit");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("AuditApplication.Models.Section", b =>
                {
                    b.HasOne("AuditApplication.Models.AuditTemplate", "AuditTemplate")
                        .WithMany("Sections")
                        .HasForeignKey("AuditTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuditTemplate");
                });

            modelBuilder.Entity("AuditApplication.Models.Audit", b =>
                {
                    b.Navigation("QuestionResponses");

                    b.Navigation("Sections");
                });

            modelBuilder.Entity("AuditApplication.Models.AuditSection", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("AuditApplication.Models.AuditTemplate", b =>
                {
                    b.Navigation("Sections");
                });

            modelBuilder.Entity("AuditApplication.Models.Section", b =>
                {
                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
