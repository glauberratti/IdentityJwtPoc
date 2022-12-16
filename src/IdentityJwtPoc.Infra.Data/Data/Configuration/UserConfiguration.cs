using IdentityJwtPoc.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityJwtPoc.Infra.Data.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id).ValueGeneratedNever().IsRequired();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Phone).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Active).IsRequired();
            builder.Property(p => p.LastChange).IsRequired();
        }
    }
}
