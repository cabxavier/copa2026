using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalCopa26.Models;

namespace PortalCopa26.Data.Configurations;

public class SimulacaoJogoConfiguration : IEntityTypeConfiguration<SimulacaoJogo>
{
    public void Configure(EntityTypeBuilder<SimulacaoJogo> builder)
    {
        builder.HasKey(sj => sj.Id);

        // Referencia o jogo oficial sem cascata: simular nunca remove o Jogo.
        builder.HasOne(sj => sj.Jogo)
            .WithMany()
            .HasForeignKey(sj => sj.JogoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Um jogo aparece no máximo uma vez por simulação.
        builder.HasIndex(sj => new { sj.SimulacaoId, sj.JogoId }).IsUnique();
    }
}
