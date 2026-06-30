using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalCopa26.Models;

namespace PortalCopa26.Data.Configurations;

public class SimulacaoConfiguration : IEntityTypeConfiguration<Simulacao>
{
    public void Configure(EntityTypeBuilder<Simulacao> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Descricao)
            .HasMaxLength(300);

        builder.HasMany(s => s.Jogos)
            .WithOne(sj => sj.Simulacao!)
            .HasForeignKey(sj => sj.SimulacaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
