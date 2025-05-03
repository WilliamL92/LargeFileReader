using BLL.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Database;
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

            using var scope = ScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LargeFileReaderDbContext>();

            FileEntity? fileEnt = await db.Files.FirstOrDefaultAsync(f => f.Path == dlg.FileName);

            FileInfo info = new FileInfo(dlg.FileName);

            int resultFileRemoved = -1;

            if (fileEnt is not null)
                resultFileRemoved = await DbCRUD.RemoveFileByPathAsync(dlg.FileName, db);

            fileEnt = await DbCRUD.CreateNewFileAsync(Path.GetFileName(dlg.FileName), dlg.FileName, info.Length, info.LastWriteTime, db);

            using var stream = info.OpenText();
            char[] buffer = new char[100];
            int read;
            int position = 0;
            const int batchSize = 1000; // Taille du lot
            List<LineEntity> batch = new List<LineEntity>();

            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                string text = new string(buffer, 0, read);
                batch.Add(new LineEntity
                {
                    IdFile = fileEnt.Id,
                    Text = text,
                    Position = position++
                });

                if (batch.Count >= batchSize)
                {
                    await DbCRUD.CreateNewLinesAsync(batch, db);
                    batch.Clear(); // Videz le lot pour le prochain groupe
                }
            }

            // Insérez les lignes restantes
            if (batch.Count > 0)
                await DbCRUD.CreateNewLinesAsync(batch, db);
        }
    }
}
