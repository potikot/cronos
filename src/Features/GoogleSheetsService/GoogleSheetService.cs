using Cronos.Features.GoogleSheetsService.Model;
using Cronos.Infrastructure.Http;
using Cronos.Infrastructure.Http.Model;
using Cronos.Infrastructure.Logger;

namespace Cronos.Features.GoogleSheetsService;

public class GoogleSheetService(
    ILoggerService _logger,
    IHttpService _httpService) : IGoogleSheetService
{
    private const string EXPORT_FORMAT = "tsv";

    public async Task<TableData> GetSheetAsync(string sheetId, int gid = 0)
    {
        if (string.IsNullOrWhiteSpace(sheetId))
        {
            _logger.LogError(this, "Parameter 'sheetId' is null or empty.");
            return null;
        }
        
        string url = BuildUrl(sheetId, gid);
        RequestResult<string> response = await _httpService.GetStringAsync(url);

        if (!response.IsSuccess())
            return null;

        string[] allRows = response.Result.Split('\n');
        List<string> headers = allRows[0].TrimEnd('\r').Split('\t').ToList();
        List<List<string>> rows = allRows[1..]
            .Select(x => x.TrimEnd('\r').Split('\t').ToList())
            .ToList();

        TableData table = new(headers, rows);
        return table;
    }

    public string BuildUrl(string sheetId, int gid = 0)
    {
        return $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format={EXPORT_FORMAT}&gid={gid}";
    }
}