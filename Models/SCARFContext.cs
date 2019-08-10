using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AUTO_ARCHIVE.Models
{
    public partial class SCARFContext : DbContext
    {
        public SCARFContext()
        {
        }

        public SCARFContext(DbContextOptions<SCARFContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Auto> Auto { get; set; }
        public virtual DbSet<Garage> Garage { get; set; }
        public virtual DbSet<TempAuto> TempAuto { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<Auto>(entity =>
            {
                entity.ToTable("AUTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AuctionDate)
                    .IsRequired()
                    .HasColumnName("AUCTION DATE")
                    .HasMaxLength(250);

                entity.Property(e => e.BidAmountC).HasColumnName("BID AMOUNT (C$)");

                entity.Property(e => e.Comments).HasColumnType("xml");

                entity.Property(e => e.Cyl)
                    .HasColumnName("CYL")
                    .HasMaxLength(250);

                entity.Property(e => e.Duplicate).HasColumnName("DUPLICATE");

                entity.Property(e => e.FifthImage).HasColumnName("Fifth Image");

                entity.Property(e => e.FirstImage).HasColumnName("First Image");

                entity.Property(e => e.FourthImage).HasColumnName("Fourth Image");

                entity.Property(e => e.Make)
                    .HasColumnName("MAKE")
                    .HasMaxLength(250);

                entity.Property(e => e.MileageKm).HasColumnName("MILEAGE (Km)");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasColumnName("MODEL")
                    .HasMaxLength(250);

                entity.Property(e => e.SecondImage).HasColumnName("Second Image");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasMaxLength(250);

                entity.Property(e => e.ThirdImage).HasColumnName("Third Image");

                entity.Property(e => e.Vin)
                    .IsRequired()
                    .HasColumnName("VIN")
                    .HasMaxLength(250);

                entity.Property(e => e.Year)
                    .IsRequired()
                    .HasColumnName("YEAR")
                    .HasMaxLength(4);
            });

            modelBuilder.Entity<Garage>(entity =>
            {
                entity.HasKey(e => e.UserOid);

                entity.ToTable("GARAGE");

                entity.Property(e => e.UserOid)
                    .HasColumnName("UserOID")
                    .HasMaxLength(250)
                    .ValueGeneratedNever();

                entity.Property(e => e.SharedGarage).HasColumnType("xml");

                entity.Property(e => e.UserGarage).HasColumnType("xml");
            });

            modelBuilder.Entity<TempAuto>(entity =>
            {
                entity.HasKey(e => new { e.Vin, e.AuctionDate })
                    .HasName("PK_TEMP_AUTO_db");

                entity.ToTable("TEMP_AUTO");

                entity.Property(e => e.Vin)
                    .HasColumnName("VIN")
                    .HasMaxLength(250);

                entity.Property(e => e.AuctionDate)
                    .HasColumnName("AUCTION DATE")
                    .HasMaxLength(20);

                entity.Property(e => e.BidAmountC).HasColumnName("BID AMOUNT (C$)");

                entity.Property(e => e.Cyl)
                    .HasColumnName("CYL")
                    .HasMaxLength(250);

                entity.Property(e => e.MileageKm).HasColumnName("MILEAGE (Km)");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasColumnName("MODEL")
                    .HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasMaxLength(250);

                entity.Property(e => e.Year).HasColumnName("YEAR");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__tmp_ms_x__8D49EF0CEE22095F");

                entity.ToTable("USERS");

                entity.Property(e => e.UserId).HasColumnName("User ID");

                entity.Property(e => e.ActivationCode).HasColumnName("Activation Code");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnName("Date Of Birth")
                    .HasColumnType("datetime");

                entity.Property(e => e.EmailId)
                    .IsRequired()
                    .HasColumnName("Email ID")
                    .HasMaxLength(256);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("First Name")
                    .HasMaxLength(50);

                entity.Property(e => e.IsEmailVerified).HasColumnName("Is Email Verified");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("Last Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.ResetPasswordCode)
                    .HasColumnName("Reset Password Code")
                    .HasMaxLength(100);
            });
        }
    }
}
