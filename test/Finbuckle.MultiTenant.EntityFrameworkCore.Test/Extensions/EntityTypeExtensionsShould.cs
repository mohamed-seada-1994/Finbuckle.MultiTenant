﻿//    Copyright 2018-2020 Finbuckle LLC, Andrew White, and Contributors
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Finbuckle.MultiTenant.EntityFrameworkCore.Test.Extensions
{
    public class EntityTypeExtensionShould : IDisposable
    {
        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions options) : base(options)
            {
            }

            // ReSharper disable once MemberHidesStaticFromOuterClass
            // ReSharper disable once UnusedMember.Local
            DbSet<MyMultiTenantThing> MyMultiTenantThing { get; set; }
            // ReSharper disable once MemberHidesStaticFromOuterClass
            // ReSharper disable once UnusedMember.Local
            DbSet<MyThing> MyThing { get; set; }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                builder.Entity<MyMultiTenantThing>().IsMultiTenant();
                builder.Entity<MyMultiTenantChildThing>();
            }
        }

        public class MyMultiTenantThing
        {
            public int Id { get; set; }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public class MyThing
        {
            public int Id { get; set; }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public class MyMultiTenantChildThing : MyMultiTenantThing
        {
        
        }
        
        private readonly SqliteConnection _connection = new SqliteConnection("DataSource=:memory:");

        public void Dispose()
        {
            _connection.Dispose();
        }

        private DbContext GetDbContext()
        {
            _connection.Open(); 
            var options = new DbContextOptionsBuilder()
                .UseSqlite(_connection)
                .Options;
            return new TestDbContext(options);
        }

        [Fact]
        public void ReturnTrueOnIsMultiTenantOnIfMultiTenant()
        {
            var db = GetDbContext();

            Assert.True(db.Model.FindEntityType(typeof(MyMultiTenantThing)).IsMultiTenant());
        }
        
        [Fact]
        public void ReturnTrueOnIsMultiTenantOnIfAncestorIsMultiTenant()
        {
            var db = GetDbContext();

            Assert.True(db.Model.FindEntityType(typeof(MyMultiTenantChildThing)).IsMultiTenant());
        }

        [Fact]
        public void ReturnFalseOnIsMultiTenantOnIfNotMultiTenant()
        {
            var db = GetDbContext();

            Assert.False(db.Model.FindEntityType(typeof(MyThing)).IsMultiTenant());
        }
    }
}