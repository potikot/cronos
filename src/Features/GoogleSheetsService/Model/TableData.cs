using Cronos.Utilities;

namespace Cronos.Features.GoogleSheetsService.Model;

public class TableData
{
    private readonly List<string> _headers;
    private readonly List<RowData> _rows;
    
    public IReadOnlyList<string> Headers => _headers;
    public IReadOnlyList<RowData> Rows => _rows;

    public TableData(List<string> headers, List<List<string>> rows)
    {
        _headers = headers;
        _rows = [];
        foreach (List<string> row in rows)
            _rows.Add(new RowData(this, row));
    }
    
    public TableData(List<List<string>> rows)
    {
        _headers = rows[1];
        _rows = [];
        for (int i = 1; i < rows.Count; i++)
            _rows.Add(new RowData(this, rows[i]));
    }
    
    public int IndexOfColumn(string header)
    {
        return _headers.IndexOf(header);
    }

    public List<string> GetColumn(string header)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(header);
        
        int colimnIndex = IndexOfColumn(header);
        if (colimnIndex == -1)
            throw new ArgumentException($"Header '{header}' not found");
        
        return GetColumn(colimnIndex);
    }
    
    public List<string> GetColumn(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, _headers.Count, nameof(index));
        
        List<string> column = new(_rows.Count);
        foreach (RowData row in _rows)
            column.Add(row[index]);
        
        return column;
    }
    
    public void ForEach(Action<string> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        foreach (RowData row in _rows)
            foreach (string cell in row.Cells)
                action(cell);
    }
    
    public void ForEach(string header, Action<string> action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(header);
        ArgumentNullException.ThrowIfNull(action);
        
        int colimnIndex = IndexOfColumn(header);
        foreach (RowData row in _rows)
            action(row[colimnIndex]);
    }
    
    public string this[int rowIndex, int columnIndex]
    {
        get
        {
            if (!_rows.InRange(rowIndex)
                || !_rows[rowIndex].Cells.InRange(columnIndex))
                return null;
            
            return _rows[rowIndex][columnIndex];
        }
    }

    public string this[int rowIndex, string header]
    {
        get
        {
            int columnIndex = IndexOfColumn(header);
            return this[rowIndex, columnIndex];
        }
    }
}