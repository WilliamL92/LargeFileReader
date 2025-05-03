using System.Reflection.Emit;
using BLL.Models;
using Microsoft.EntityFrameworkCore;

public class LargeFileReaderDbContext : DbContext
{
    public DbSet<FileEntity> Files { get; set; }
    public DbSet<LineEntity> Lines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "LargeFileReader.db");
        options.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<LineEntity>()
            .HasOne(l => l.FileEntity)
            .WithMany(f => f.Lines)
            .HasForeignKey(l => l.IdFile)
            .OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(model);
    }
}