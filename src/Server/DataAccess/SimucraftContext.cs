using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Simucraft.Server.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Simucraft.Server.DataAccess
{
    public class SimucraftContext : DbContext
    {
        public SimucraftContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<Ruleset> Rulesets { get; set; } 

        public DbSet<Map> Maps { get; set; }

        public DbSet<Character> Characters { get; set; }

        public DbSet<Weapon> Weapons { get; set; }

        public DbSet<Equipment> Equipment { get; set; }

        public DbSet<Spell> Spells { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<InvitedGame> InvitedGames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Character>()
                .Property(c => c.WeaponIds)
                .HasConversion(new ValueConverter<ICollection<Guid>, string>(
                    v => string.Join(",", v),
                    v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => Guid.Parse(s))
                        .ToList()));

            modelBuilder
                .Entity<Character>()
                .Property(c => c.EquipmentIds)
                .HasConversion(new ValueConverter<ICollection<Guid>, string>(
                    v => string.Join(",", v),
                    v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => Guid.Parse(s))
                        .ToList()));

            modelBuilder
                .Entity<Character>()
                .Property(c => c.SpellIds)
                .HasConversion(new ValueConverter<ICollection<Guid>, string>(
                    v => string.Join(",", v),
                    v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => Guid.Parse(s))
                        .ToList()));

            //modelBuilder
            //    .Entity<Character>()
            //    .Property(c => c.SkillIds)
            //    .HasConversion(new ValueConverter<ICollection<Guid>, string>(
            //        v => string.Join(",", v),
            //        v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
            //            .Select(s => Guid.Parse(s))
            //            .ToList()));
        }
    }
}
