using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Models;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public static class DbCRUD
    {
        public async static Task<FileEntity> CreateNewFileAsync(string filename, string Path, long Size, DateTime lastModified, LargeFileReaderDbContext db)
        {
            FileEntity fileEnt =  new FileEntity
            {
                Name = filename,
                Path = Path,
                Size = Size,
                LastModified = lastModified
            };
            db.Files.Add(fileEnt);
            await db.SaveChangesAsync();
            return fileEnt;
        }

        public static async Task<int> CreateNewLinesAsync(List<LineEntity> lines, LargeFileReaderDbContext db)
        {
            if (lines == null || lines.Count == 0)
                return 0;

            await db.Lines.AddRangeAsync(lines);
            return await db.SaveChangesAsync();
        }

        public static async Task<FileEntity?> GetFileByIdAsync(int id, LargeFileReaderDbContext db)
        {
            return await db.Files.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async static Task<int> RemoveFileByPathAsync(string path, LargeFileReaderDbContext db)
        {
            FileEntity? fileEnt = await db.Files
                .FirstOrDefaultAsync(f => f.Path == path);
            if (fileEnt is null)
                return 0;
            db.Files.Remove(fileEnt);
            return await db.SaveChangesAsync();
        }
    }
}
