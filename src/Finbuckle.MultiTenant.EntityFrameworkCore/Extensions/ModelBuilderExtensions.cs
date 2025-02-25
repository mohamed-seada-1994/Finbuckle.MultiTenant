// Copyright 2018-2020 Finbuckle LLC, Andrew White, and Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finbuckle.MultiTenant.EntityFrameworkCore
{
    public static class FinbuckleModelBuilderExtensions
    {
        /// <summary>
        /// Configures any entity's with the [MultiTenant] attribute.
        /// </summary>
        public static ModelBuilder ConfigureMultiTenant(this ModelBuilder modelBuilder)
        {
            // Call IsMultiTenant() to configure the types marked with the MultiTenant Data Attribute
            foreach (var clrType in modelBuilder.Model.GetEntityTypes()
                                                 .Where(et => et.ClrType.HasMultiTenantAttribute())
                                                 .Select(et => et.ClrType))
            {
                modelBuilder.Entity(clrType)
                            .IsMultiTenant();
            }

            return modelBuilder;
        }
    }
}