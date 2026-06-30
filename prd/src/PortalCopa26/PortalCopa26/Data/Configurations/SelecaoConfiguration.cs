using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalCopa26.Models;

namespace PortalCopa26.Data.Configurations;

public class SelecaoConfiguration : IEntityTypeConfiguration<Selecao>
{
    public void Configure(EntityTypeBuilder<Selecao> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nome)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(s => s.Codigo)
            .IsRequired()
            .HasMaxLength(3);

        builder.HasIndex(s => s.Codigo).IsUnique();

        builder.Property(s => s.BandeiraUrl)
            .HasMaxLength(300);

        builder.Property(s => s.Tecnico)
            .HasMaxLength(100);

        builder.HasMany(s => s.Jogadores)
            .WithOne(j => j.Selecao!)
            .HasForeignKey(j => j.SelecaoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.RankingFifa)
            .WithOne(r => r.Selecao!)
            .HasForeignKey<RankingFifa>(r => r.SelecaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
