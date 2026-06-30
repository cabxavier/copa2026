using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalCopa26.Models;

namespace PortalCopa26.Data.Configurations;

public class JogadorConfiguration : IEntityTypeConfiguration<Jogador>
{
    public void Configure(EntityTypeBuilder<Jogador> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.Posicao)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
