﻿using Exelor.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exelor.Infrastructure.Data.Config
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(
            EntityTypeBuilder<Role> builder)
        {
            builder
                .HasMany(a => a.Users)
                .WithOne(a => a.Role)
                .HasForeignKey(a => a.RoleId);

            builder
                .Property("_permissionsInRole")
                .HasColumnName("PermissionsInRole")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}