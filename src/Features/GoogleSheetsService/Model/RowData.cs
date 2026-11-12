using Cronos.Utilities;

namespace Cronos.Features.GoogleSheetsService.Model;

public class RowData
{
    private TableData _table;
    private List<string> _cells;

    public IReadOnlyList<string> Cells => _cells;
    
    public RowData(TableData table, List<string> cells)
    {
        _table = table;
        _cells = cells;
    }

    public string this[int columnIndex]
    {
        get
        {
            if (!_cells.InRange(columnIndex))
                return null;
            
            return _cells[columnIndex];
        }
    }

    public string this[string header]
    {
        get
        {
            int index = _table.IndexOfColumn(header);
            return this[index];
        }
    }
}