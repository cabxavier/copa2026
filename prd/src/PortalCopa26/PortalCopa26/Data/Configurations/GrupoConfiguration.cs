using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalCopa26.Models;

namespace PortalCopa26.Data.Configurations;

public class GrupoConfiguration : IEntityTypeConfiguration<Grupo>
{
    public void Configure(EntityTypeBuilder<Grupo> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Nome)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(g => g.Nome).IsUnique();

        builder.HasMany(g => g.Selecoes)
            .WithOne(s => s.Grupo!)
            .HasForeignKey(s => s.GrupoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
