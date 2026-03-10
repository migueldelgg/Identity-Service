using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityService.Modules.Identity.Models;

namespace IdentityService.Modules.Identity.Mapping;

internal sealed class UsersMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuidv7()")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasColumnName("Email")
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(p => p.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.IsVerified)
            .HasColumnName("IsVerified")
            .IsRequired();

        // Índices/constraints úteis
        builder.HasIndex(p => p.Email).IsUnique();
    }
}