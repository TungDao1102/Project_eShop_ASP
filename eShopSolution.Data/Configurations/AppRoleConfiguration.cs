﻿using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopSolution.Data.Configurations
{
    public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            //  throw new NotImplementedException();
            builder.ToTable("AppRoles");
            builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
        }
    }
}
