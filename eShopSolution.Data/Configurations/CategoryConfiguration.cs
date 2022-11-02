using eShopSolution.Data.Entities;
using eShopSolution.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            //   throw new NotImplementedException();
            {
                builder.ToTable("Categories");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.SortOrder).IsRequired();
                builder.Property(x => x.IsShowOnHome).IsRequired();
                builder.Property(x => x.Status).HasDefaultValue(Status.Active);
                builder.Property(x => x.ParentId);
            }
        }
    }
}
