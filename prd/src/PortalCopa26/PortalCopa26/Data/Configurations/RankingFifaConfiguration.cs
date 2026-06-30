using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalCopa26.Models;

namespace PortalCopa26.Data.Configurations;

public class RankingFifaConfiguration : IEntityTypeConfiguration<RankingFifa>
{
    public void Configure(EntityTypeBuilder<RankingFifa> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Pontuacao)
            .HasColumnType("decimal(7,2)");

        builder.HasIndex(r => r.SelecaoId).IsUnique();
    }
}
