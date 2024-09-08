
using asg_form.Controllers.Store;
using asg_form.Controllers.Team;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Cryptography;
using System.Text;

namespace asg_form.Controllers
{
    class FormConfig : IEntityTypeConfiguration<form>
    {
        class forcomConfig : IEntityTypeConfiguration<comform.com_form>
        {
            public void Configure(EntityTypeBuilder<comform.com_form> builder)
            {
                builder.ToTable("F_ComForm");
                builder.Property(e => e.Id).IsRequired();
                builder.Property(a => a.introduction).IsRequired();
                builder.Property(a => a.Com_Cocial_media).IsRequired();
                builder.Property(a => a.comSex).IsRequired();
                builder.Property(a => a.UserId);
                builder.Property(a => a.idv_id).IsRequired();
                builder.Property(a => a.Com_qq).IsRequired();
                builder.Property(a => a.Status).IsRequired();
            }
        }



        public void Configure(EntityTypeBuilder<form> builder)
        {
            builder.ToTable("F_form");
            builder.Property(e => e.team_name).IsRequired();
            builder.Property(e => e.team_tel).IsRequired();
            builder.Property(e => e.team_password).IsRequired();
            builder.Property(e => e.time).IsRequired();
            builder.Property(e => e.piaoshu).IsRequired();
              builder.HasOne<T_events>(c => c.events).WithMany(a => a.forms).IsRequired();
          
        }
    }


    class TeamConfig : IEntityTypeConfiguration<T_Team>
    {
        public void Configure(EntityTypeBuilder<T_Team> builder)
        {
            builder.ToTable("F_Team");
            builder.Property(e => e.team_name).IsRequired();
            builder.Property(e => e.team_tel).IsRequired();
            builder.Property(e => e.team_password).IsRequired();
            builder.Property(e => e.time).IsRequired();
            builder.Property(e => e.piaoshu).IsRequired();
            builder.HasMany<T_events>(a => a.Events).WithMany(a => a.Teams).UsingEntity(j => j.ToTable("T_Teams_Player"));

        }
    }

    class PlayerConfig : IEntityTypeConfiguration<T_Player>
    {
        public void Configure(EntityTypeBuilder<T_Player> builder)
        {
            builder.ToTable("F_Player");
          
        }
    }



    class RoleConfig : IEntityTypeConfiguration<role>
    {
        public void Configure(EntityTypeBuilder<role> builder)
        {
            builder.ToTable("F_role");
            builder.Property(e => e.role_id).IsRequired();
            builder.Property(e => e.role_lin).IsRequired();
            builder.Property(e => e.role_name).IsRequired();
            builder.HasOne<form>(c => c.form).WithMany(a => a.role).IsRequired();

        }
    }
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            return query.Skip(pageIndex * pageSize).Take(pageSize);
        }
    }
    class STOREConfig : IEntityTypeConfiguration<StoreinfoDB>
    {
        public void Configure(EntityTypeBuilder<StoreinfoDB> builder)
        {
            builder.ToTable("T_Storeinfo");
            builder.HasOne<StoreDB>(c => c.Store).WithMany(a => a.buyer).IsRequired();

        }
    }
    class STORConfig : IEntityTypeConfiguration<StoreDB>
    {
        public void Configure(EntityTypeBuilder<StoreDB> builder)
        {
            builder.ToTable("T_Store");
        

        }
    }

    class newsConfig : IEntityTypeConfiguration<T_news>
    {
        public void Configure(EntityTypeBuilder<T_news> builder)
        {
            builder.ToTable("F_news");
            builder.Property(e => e.FormName).IsRequired();
            builder.Property(e => e.msg).IsRequired();
            builder.Property(e => e.Title).IsRequired();
            builder.Property(e => e.time).IsRequired();
            builder.Property(e => e.Type).IsRequired();
        }
    }

    class blogConfig : IEntityTypeConfiguration<blog.blog_db>
    {
        public void Configure(EntityTypeBuilder<blog.blog_db> builder)
        {
            builder.ToTable("F_blog");
            builder.Property(e => e.title).IsRequired();
            builder.Property(e => e.msg).IsRequired();
            builder.Property(e => e.formuser).IsRequired();
            builder.Property(e => e.pushtime).IsRequired();
        }
    }


    class schgameConfig : IEntityTypeConfiguration<schedule.team_game>
    {
        public void Configure(EntityTypeBuilder<schedule.team_game> builder)
        {
            builder.ToTable("F_game");
            builder.Property(e => e.team1_name).IsRequired();
            builder.Property(e => e.team1_piaoshu).IsRequired();
            builder.Property(e => e.team2_name).IsRequired();
            builder.Property(e => e.team2_piaoshu).IsRequired();
            builder.Property(e => e.opentime).IsRequired();
         
        }
    }


    class schlogConfig : IEntityTypeConfiguration<schedule.schedule_log>
    {
        public void Configure(EntityTypeBuilder<schedule.schedule_log> builder)
        {
            builder.ToTable("F_achlog");
            builder.Property(e => e.userid).IsRequired();
            builder.Property(e => e.win);
            builder.HasOne<schedule.team_game>(e => e.team).WithMany(o=>o.logs).IsRequired();
        }
    }
    class EventsConfig : IEntityTypeConfiguration<T_events>
    {
        public void Configure(EntityTypeBuilder<T_events> builder)
        {
            builder.ToTable("F_events");
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.is_over);
            builder.Property(e => e.name);
            builder.Property(e => e.opentime);
         
        }
    }
    class ChampionConfig : IEntityTypeConfiguration<Champion.T_Champion>
    {
        public void Configure(EntityTypeBuilder<Champion.T_Champion> builder)
        {
            builder.ToTable("F_Champion");
            builder.Property(e => e.Id).IsRequired();
            builder.HasOne(a => a.events);
            builder.Property(e => e.msg);
            builder.HasOne(a=>a.events);

        }
    }


    class T_friendConfig : IEntityTypeConfiguration<T_Friend>
    {
        public void Configure(EntityTypeBuilder<T_Friend> builder)
        {
            builder.ToTable("F_Friend");
            builder.Property(e => e.id).IsRequired();
            builder.Property(e => e.comMsg);
            builder.Property(e => e.comType);
            builder.Property(e => e.comTime);
            builder.Property(e => e.account);
            builder.Property(e => e.headTel);
            builder.Property(e => e.headTel);

        }
    }

    class configConfig : IEntityTypeConfiguration<T_config>
    {
        public void Configure(EntityTypeBuilder<T_config> builder)
        {
            builder.ToTable("T_Config");
            builder.Property(e => e.Id).IsRequired();
            builder.Property(a => a.Title);
            builder.Property(a => a.msg);
            builder.Property(e => e.Substance);


        }
    }



    class TestDbContext : DbContext
    {
        public DbSet<T_Team> Teams { get; set; }
        public DbSet<T_Player> Players { get; set; }

        public DbSet<form> Forms { get; set; }
        public DbSet<role> Roles { get; set; }
        public DbSet<T_news> news { get; set; }
        public DbSet<blog.blog_db> blogs { get; set; }
        public DbSet<schedule.schedule_log> schlogs { get; set; }
        public DbSet<schedule.team_game> team_Games { get; set; }
        public DbSet<T_events> events { get; set; }
        public DbSet<Champion.T_Champion> Champions { get; set; }
        public DbSet<comform.com_form> com_Forms { get; set; }
        public DbSet<T_Friend> T_Friends { get; set; }
        public DbSet<StoreDB> T_Store { get; set; }
        public DbSet<StoreinfoDB> T_Storeinfo { get; set; }
        public DbSet<T_config> T_config { get; set; }
        public DbSet<TaskDB> T_Task { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connStr = @"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=true";
            optionsBuilder.UseSqlServer(connStr);
        
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

    }


    public class IDBcontext : IdentityDbContext<User, Role, long>
    {
        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }
        public IDBcontext(DbContextOptions<IDBcontext> opt) : base(opt)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }


    }


}