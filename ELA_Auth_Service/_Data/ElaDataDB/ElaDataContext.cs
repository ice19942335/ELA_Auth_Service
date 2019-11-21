using ELA_Auth_Service._Data.ElaDataDB.Migrations;
using ELA_Auth_Service._Data.ElaDataDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ELA_Auth_Service._Data.ElaDataDB
{
    public partial class ElaDataContext : DbContext
    {
        public ElaDataContext()
        {
        }

        public ElaDataContext(DbContextOptions<ElaDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answers> Answers { get; set; }
        public virtual DbSet<AnswersType> AnswersType { get; set; }
        public virtual DbSet<BunchLessonTasks> BunchLessonTasks { get; set; }
        public virtual DbSet<Lessons> Lessons { get; set; }
        public virtual DbSet<Migration> Migration { get; set; }
        public virtual DbSet<TaskTypes> TaskTypes { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<Theme> Theme { get; set; }
        public virtual DbSet<UserLessonStats> UserLessonStats { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Words> Words { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answers>(entity =>
            {
                entity.ToTable("answers");

                entity.HasIndex(e => e.AnswersTypeId)
                    .HasName("fk_answers_answers_type");

                entity.HasIndex(e => e.TasksId)
                    .HasName("fk_answers_tasks");

                entity.HasIndex(e => e.WordsId)
                    .HasName("fk_answers_words");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AnswersTypeId)
                    .HasColumnName("answers_type_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TasksId)
                    .HasColumnName("tasks_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WordsId)
                    .HasColumnName("words_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.AnswersType)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.AnswersTypeId)
                    .HasConstraintName("fk_answers_answers_type");

                entity.HasOne(d => d.Tasks)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.TasksId)
                    .HasConstraintName("fk_answers_tasks");

                entity.HasOne(d => d.Words)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.WordsId)
                    .HasConstraintName("fk_answers_words");
            });

            modelBuilder.Entity<AnswersType>(entity =>
            {
                entity.ToTable("answers_type");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<BunchLessonTasks>(entity =>
            {
                entity.ToTable("bunch_lesson_tasks");

                entity.HasIndex(e => e.LessonsId)
                    .HasName("fk_bunch_les_tasks_lessons");

                entity.HasIndex(e => e.TasksId)
                    .HasName("fk_bunch_les_tasks_tasks");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LessonsId)
                    .HasColumnName("lessons_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TasksId)
                    .HasColumnName("tasks_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Lessons)
                    .WithMany(p => p.BunchLessonTasks)
                    .HasForeignKey(d => d.LessonsId)
                    .HasConstraintName("fk_bunch_les_tasks_lessons");

                entity.HasOne(d => d.Tasks)
                    .WithMany(p => p.BunchLessonTasks)
                    .HasForeignKey(d => d.TasksId)
                    .HasConstraintName("fk_bunch_les_tasks_tasks");
            });

            modelBuilder.Entity<Lessons>(entity =>
            {
                entity.ToTable("lessons");

                entity.HasIndex(e => e.ThemeId)
                    .HasName("fk_lessons_theme");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ImgUrl)
                    .HasColumnName("img_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PointsToPay)
                    .HasColumnName("points_to_pay")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ThemeId)
                    .HasColumnName("theme_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.ThemeId)
                    .HasConstraintName("fk_lessons_theme");
            });

            modelBuilder.Entity<Migration>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PRIMARY");

                entity.ToTable("migration");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasColumnType("varchar(180)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ApplyTime)
                    .HasColumnName("apply_time")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<TaskTypes>(entity =>
            {
                entity.ToTable("task_types");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.ToTable("tasks");

                entity.HasIndex(e => e.TaskTypesId)
                    .HasName("fk_tasks_task_types");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TaskTypesId)
                    .HasColumnName("task_types_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.TaskTypes)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.TaskTypesId)
                    .HasConstraintName("fk_tasks_task_types");
            });

            modelBuilder.Entity<Theme>(entity =>
            {
                entity.ToTable("theme");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<UserLessonStats>(entity =>
            {
                entity.ToTable("user_lesson_stats");

                entity.HasIndex(e => e.LessonsId)
                    .HasName("fk_user_lesson_stats_lessons1");

                entity.HasIndex(e => e.UsersId)
                    .HasName("fk_user_lesson_stats_users1");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ComplTasksCounter)
                    .HasColumnName("compl_tasks_counter")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LessonsId)
                    .HasColumnName("lessons_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UsersId)
                    .IsRequired()
                    .HasColumnName("users_id")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Lessons)
                    .WithMany(p => p.UserLessonStats)
                    .HasForeignKey(d => d.LessonsId)
                    .HasConstraintName("fk_user_lesson_stats_lessons1");

                entity.HasOne(d => d.Users)
                    .WithMany(p => p.UserLessonStats)
                    .HasForeignKey(d => d.UsersId)
                    .HasConstraintName("fk_user_lesson_stats_users1");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("users");

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .HasColumnType("varchar(36)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BirthDate)
                    .HasColumnName("birth_date")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ImgUrl)
                    .HasColumnName("imgUrl")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Points)
                    .HasColumnName("points")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RegDate)
                    .HasColumnName("reg_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Words>(entity =>
            {
                entity.ToTable("words");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Audio)
                    .HasColumnName("audio")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Eng)
                    .IsRequired()
                    .HasColumnName("eng")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ImgUrl)
                    .HasColumnName("img_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Rus)
                    .IsRequired()
                    .HasColumnName("rus")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Transcript)
                    .HasColumnName("transcript")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
