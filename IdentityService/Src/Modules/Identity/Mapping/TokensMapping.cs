using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityService.Modules.Identity.Models;

namespace IdentityService.Modules.Identity.Mapping;

internal sealed class TokensMapping : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable("Tokens");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuidv7()")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.UserId)
            .HasColumnName("UserId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(p => p.Jwt)
            .HasColumnName("Jwt")
            .HasColumnType("text")   // <- aqui o cuidado com o JWT
            .IsRequired();

        builder.Property(p => p.ExpiresAt)
            .HasColumnName("ExpiresAt")
            .IsRequired();

        // Relação: User 1 -> N Tokens
        builder.HasOne(p => p.User)
            .WithMany()                 // (se você depois criar ICollection<Token> Tokens no User, dá pra trocar aqui)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.ExpiresAt);
    }
}