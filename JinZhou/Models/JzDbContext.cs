using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JinZhou.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace JinZhou.Models
{
    public class JzDbContext : DbContext
    {
        public JzDbContext(DbContextOptions<JzDbContext> options):base(options)
        {
            
        }

        public DbSet<BasicToken> BasicTokens { get; set; }

        public DbSet<AppAuthInfo> AppAuths { get; set; }

        public DbSet<AuthorizerInfo> AuthorizerInfos { get; set; }

        public DbSet<WxUserInfo> WxUserInfos
        {
            get;
            set;
        }

    }
}
