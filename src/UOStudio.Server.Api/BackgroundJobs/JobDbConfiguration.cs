using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal sealed class JobDbConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.HasKey(b => b.Id);
        }
    }
}
