#region

#endregion


namespace Scotec.Math;

public class LookupTable2D
{
    private readonly double[] _columns;
    private readonly double[] _rows;
    private readonly double[,] _valueTable;
    private double[]? _columnsArray;
    private double[]? _rowsArray;


    public LookupTable2D(IEnumerable<double> rows, IEnumerable<double> columns, double[,] values)
    {
        _valueTable = values;
        _rows = rows.ToArray();
        _columns = columns.ToArray();
    }

    public LookupTable2D(string filePath, char columnSeparator)
    {
        var rows = new List<double>();
        var columns = new List<double>();

        string? line;
        var lines = new List<string>();
        var file = new StreamReader(filePath);
        if ((line = file.ReadLine()) != null)
        {
            line = line.Substring(line.IndexOf(columnSeparator) + 1);
            var xValues = line.Split(columnSeparator);
            columns.AddRange(xValues.Select(s => double.Parse(s)));
        }

        while ((line = file.ReadLine()) != null)
            lines.Add(line);
        file.Close();

        _valueTable = new double[lines.Count, columns.Count];
        var i = 0;
        foreach (var values in lines.Select(row => row.Split(columnSeparator)))
        {
            rows.Add(double.Parse(values[0]));
            for (var j = 0; j < values.Length - 1; j++)
                _valueTable[i, j] = double.Parse(values[j + 1]);
            for (var j = values.Length - 1; j < columns.Count; j++)
                _valueTable[i, j] = double.NaN;
            ++i;
        }

        _rows = rows.ToArray();
        _columns = columns.ToArray();
    }

    public double[] Rows =>
        // Cache the array for better performance.
        _rowsArray ?? (_rowsArray = _rows.ToArray());

    public double[] Columns =>
        // Cache the array for better performance.
        _columnsArray ?? (_columnsArray = _columns.ToArray());

    public double MinimumRowValue()
    {
        return _rows[0];
    }

    public double MaximumRowValue()
    {
        return _rows[_rows.Length - 1];
    }

    public double MinimumColumnValue()
    {
        return _columns[0];
    }

    public double MaximumColumnValue()
    {
        return _columns[_columns.Length - 1];
    }

    public double GetInterpolatedValue(double rowValue, double columnValue)
    {
        if (rowValue.IsLower(MinimumRowValue()) || rowValue.IsGreater(MaximumRowValue()) || columnValue.IsLower(MinimumColumnValue())
            || columnValue.IsGreater(MaximumColumnValue()))
            return double.NaN;

        //find the row index the value falls within
        var row2 = Array.BinarySearch(_rows, rowValue);
        if (row2 < 0)
            row2 = ~row2;
        var row1 = _rows[row2].IsEqual(rowValue) ? row2 : row2 - 1;

        //find the column index the value falls within
        var column2 = Array.BinarySearch(_columns, columnValue);
        if (column2 < 0)
            column2 = ~column2;
        var column1 = _columns[column2].IsEqual(columnValue) ? column2 : column2 - 1;

        var v11 = _valueTable[row1, column1];
        var v21 = _valueTable[row2, column1];

        //initialize the value points from which the interpolation should be done to be the ones 
        //in the first column found (this exists)
        var v1 = v11;
        var v2 = v21;
        //columns are different (4 value points are found), so: for each row interpolate between the columns
        if (column1 != column2)
        {
            var v12 = _valueTable[row1, column2];
            var v22 = _valueTable[row2, column2];
            //v1 holds the interpolated value for the first row 
            v1 += (v12 - v11) * (columnValue - _columns[column1]) / (_columns[column2] - _columns[column1]);
            //v2 holds the interpolated value for the second row                 
            v2 += (v22 - v21) * (columnValue - _columns[column1]) / (_columns[column2] - _columns[column1]);
        }

        //set initial value to be the  first value point
        var v = v1;
        //rows are different, so interpolate between the value points of the row
        if (row2 != row1)
            v += (v2 - v1) * (rowValue - _rows[row1]) / (_rows[row2] - _rows[row1]);

        return v;
    }

    /// <summary>
    ///     Finds the next available interpolated value in the values table which is closer to the value of
    ///     <paramref name="rowValue" />.
    ///     The existing column value of that particular value will also being returned as an out parameter to
    ///     <paramref name="nextAvailableColumnValue" />.
    /// </summary>
    /// <param name="rowValue">The value for which the search is done.</param>
    /// <param name="nextAvailableColumnValue">The returned column value corresponding to the particular searched value.</param>
    /// <returns>The interpolated row value. Returns double.NaN if nothing is found.</returns>
    public double FirstAvailableValue(double rowValue, out double nextAvailableColumnValue)
    {
        //initialize variables
        nextAvailableColumnValue = double.NaN;

        //check if out of limits
        if (rowValue.IsLower(MinimumRowValue()) || rowValue.IsGreater(MaximumRowValue()))
            return double.NaN;

        //find in which row the rowValue stops being less than or equal the value in the array
        var row = Array.BinarySearch(_rows, rowValue);
        if (row < 0)
            row = ~row;

        var row1 = row == 0 ? row : row - 1; //if this is the first row in the _rows array, assign this also as the second row
        var row2 = row;

        //get the columns count
        var columnsCount = _valueTable.GetLength(1);
        // find the next NOT double.NaN value in this particular row
        var column = -1;
        for (var i = 0; i < columnsCount; i++)
            if (!double.IsNaN(_valueTable[row2, i]))
            {
                column = i;
                break;
            }

        // no value could be resolved...row contains only nan!
        if (column == -1) return double.NaN;
        //get the value of this particular column
        nextAvailableColumnValue = _columns[column];

        //interpolate the value            
        var v = _valueTable[row1, column] + (rowValue - _rows[row1]) * (_valueTable[row2, column] - _valueTable[row1, column]) / (_rows[row2] - _rows[row1]);

        return v;
    }

    public double LastAvailableValue(double rowValue, out double lastAvailableColumnValue)
    {
        //initialize variables
        lastAvailableColumnValue = double.NaN;

        //check if out of limits
        if (rowValue.IsLower(MinimumRowValue()) || rowValue.IsGreater(MaximumRowValue()))
            return double.NaN;

        //find in which row the rowValue stops being less than or equal the value in the array
        var row = Array.BinarySearch(_rows, rowValue);
        if (row < 0)
            row = ~row;

        var row1 = row == 0 ? row : row - 1; //if this is the first row in the _rows array, assign this also as the second row
        var row2 = row;


        //get the columns count
        var columnsCount = _valueTable.GetLength(1);
        // find the last NOT double.NaN value in this particular row
        var column = -1;
        for (var i = 0; i < columnsCount; i++)
        {
            if (double.IsNaN(_valueTable[row1, i]))
                break;

            column = i;
        }

        // no value could be resolved...row contains only nan!
        if (column == -1) return double.NaN;
        //get the value of this particular column
        lastAvailableColumnValue = _columns[column];

        //interpolate the value            
        var v = _valueTable[row1, column] + (rowValue - _rows[row1]) * (_valueTable[row2, column] - _valueTable[row1, column]) / (_rows[row2] - _rows[row1]);

        return v;
    }


    /// <summary>
    ///     Gets the minimum value of a given column value in the row with minimum value
    /// </summary>
    /// <param name="colValue">Value of column in which to search</param>
    /// <returns></returns>
    public double GetMinimumValueInColumn(double colValue)
    {
        var columnIndex = Array.FindIndex(_columns, c => c.Equals(colValue));

        var minimum = _valueTable[0, columnIndex];

        for (var rowIndex = 0; rowIndex < _rows.Length; rowIndex++)
            if (_valueTable[rowIndex, columnIndex] < minimum)
                minimum = _valueTable[rowIndex, columnIndex];

        return minimum;
    }

    public double GetRow(double colValue, double value)
    {
        if (colValue.IsLower(MinimumColumnValue()) || colValue.IsGreater(MaximumColumnValue()))
            return double.NaN;

        //var colIndex = _columns.ToList().IndexOf( _columns.Where( v => v.IsGreaterOrEqual( colValue ) ).Min() );
        var colIndex = Array.FindIndex(_columns, v => v.IsGreaterOrEqual(colValue));

        var valuesGreater = (from rowIndex in Enumerable.Range(0, _rows.Length)
            let valueAtRowCol = _valueTable[rowIndex, colIndex]
            where valueAtRowCol.IsGreaterOrEqual(value)
            select new { rowIndex, valueAtRowCol }).ToArray();


        if (!valuesGreater.Any())
            return double.NaN;

        return _rows[valuesGreater.First(v => v.valueAtRowCol.IsEqual(valuesGreater.Min(t => t.valueAtRowCol))).rowIndex];
    }

    public double GetColumn(double rowValue, double value)
    {
        if (rowValue.IsLower(_rows.Min()) || rowValue.IsGreater(_rows.Max()))
            return double.NaN;

        var rowIndex = Array.FindIndex(_rows, v => v.IsGreaterOrEqual(rowValue));

        var valuesGreater = (from colIndex in Enumerable.Range(0, _columns.Length)
            let valueAtRowCol = _valueTable[rowIndex, colIndex]
            where valueAtRowCol.IsGreaterOrEqual(value)
            select new { colIndex, valueAtRowCol }).ToArray();

        return !valuesGreater.Any()
            ? double.NaN
            : _columns[valuesGreater.First(v => v.valueAtRowCol.IsEqual(valuesGreater.Min(t => t.valueAtRowCol))).colIndex];
    }
}