using Database.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BLL;

namespace EditorSqLite.Pages.Components
{
    public partial class Menu : ComponentBase
    {
        [Inject] private IServiceProvider Services { get; set; }
        [Inject] private IServiceScopeFactory ScopeFactory { get; set; }

        private async Task SaveFile()
        {
            ImportFile importFile = new ImportFile();
            OpenFileDialog? dlg = importFile.OpenFileBox();

            if(dlg is null)
                return;

            string filePath = dlg.FileName;
            FileInfo info = new FileInfo(filePath);

            using var scope = ScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LargeFileReaderDbContext>();

            var fileEnt = await db.Files.FirstOrDefaultAsync(f => f.Path == filePath);

            if (fileEnt is null)
            {
                fileEnt = new FileEntity
                {
                    Name = Path.GetFileName(filePath),
                    Path = filePath
                };
                db.Files.Add(fileEnt);
            }
            else
            {
                db.Lines.RemoveRange(db.Lines.Where(l => l.IdFile == fileEnt.Id));
            }

            fileEnt.Size = info.Length;
            fileEnt.LastModified = info.LastWriteTime;

            await db.SaveChangesAsync();

            using var stream = info.OpenText();
            char[] buffer = new char[100];
            int read;
            int position = 0;
            var toInsert = new List<LineEntity>();

            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var text = new string(buffer, 0, read);
                toInsert.Add(new LineEntity
                {
                    IdFile = fileEnt.Id,
                    Text = text,
                    Position = position++
                });
            }

            db.Lines.AddRange(toInsert);
            await db.SaveChangesAsync();
        }
    }
}
