using Cronos.Features.GoogleSheetsService.Model;

namespace Cronos.Features.GoogleSheetsService;

public interface IGoogleSheetService
{
    Task<TableData> GetSheetAsync(string sheetId, int gid = 0);
}