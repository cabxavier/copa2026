using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalCopa26.Models;

namespace PortalCopa26.Data.Configurations;

public class JogoConfiguration : IEntityTypeConfiguration<Jogo>
{
    public void Configure(EntityTypeBuilder<Jogo> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Estadio)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(j => j.Cidade)
            .IsRequired()
            .HasMaxLength(80);

        // Relacionamentos com seleções sem cascata para evitar múltiplos
        // caminhos de exclusão e remoção acidental de seleções.
        builder.HasOne(j => j.SelecaoMandante)
            .WithMany()
            .HasForeignKey(j => j.SelecaoMandanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.SelecaoVisitante)
            .WithMany()
            .HasForeignKey(j => j.SelecaoVisitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Grupo)
            .WithMany(g => g.Jogos)
            .HasForeignKey(j => j.GrupoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
