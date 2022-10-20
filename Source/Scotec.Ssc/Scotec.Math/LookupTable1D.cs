#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion


namespace Scotec.Math
{
    public enum OutOfRangeBehaviour
    {
        ThrowException,

        ReturnNull,

        ReturnNaN,

        ReturnMinOrMax
    }
    public class LookupTable1D
    {
        private readonly double[]_rows;
        private readonly double[] _valueTable;

        public LookupTable1D( IEnumerable<double> rows, double[] values )
        {
            _valueTable = values;
            _rows = rows.ToArray();
        }

        public LookupTable1D( string filePath, char columnSeparator )
        {
            var rows = new List<double>();

            string? line;
            var lines = new List<string>();
            var file = new StreamReader( filePath )!;

            while( (line = file.ReadLine()) != null )
                lines.Add( line );
            
            file.Close();

            _valueTable = new double[lines.Count];
            var i = 0;
            foreach( var values in lines.Select( row => row.Split( columnSeparator ) ) )
            {
                rows.Add( double.Parse( values[0] ) );
                _valueTable[i] = values.Length > 1 ? double.Parse( values[1] ) : double.NaN;
                ++i;
            }

            _rows = rows.ToArray();
        }

        public double MinimumRowValue()
        {
            return _rows[0];
        }

        public double MaximumRowValue()
        {
            return _rows[_rows.Length - 1];
        }

 
        public double GetInterpolatedValue( double rowValue, OutOfRangeBehaviour outOfRangeBehaviour)
        {
            var minRowValue = MinimumRowValue();
            var maxRowValue = MaximumRowValue();
            if (rowValue.IsLower(minRowValue) || rowValue.IsGreater(maxRowValue))
            {
                switch (outOfRangeBehaviour)
                {
                    case OutOfRangeBehaviour.ReturnNaN:
                        return double.NaN;
                    case OutOfRangeBehaviour.ThrowException:
#pragma warning disable S112 // General exceptions should never be thrown
                        throw new IndexOutOfRangeException($"{nameof(rowValue)}: {rowValue}");
#pragma warning restore S112 // General exceptions should never be thrown
                    case OutOfRangeBehaviour.ReturnMinOrMax:
                        rowValue = rowValue.IsLower(minRowValue) ? minRowValue : maxRowValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(outOfRangeBehaviour), outOfRangeBehaviour, null);
                }
            }

            var row2 = Array.BinarySearch(_rows, rowValue);
            if (row2 < 0)
                row2 = ~row2;

            var row1 = _rows[row2].IsEqual( rowValue) ? row2 : row2 - 1;

            if( row2 == row1 )
                return _valueTable[row1];

            return _valueTable[row1] + (_valueTable[row2] - _valueTable[row1]) * (rowValue - _rows[row1]) / (_rows[row2] - _rows[row1]);
        }

        public double GetInterpolatedValue(double rowValue)
        {
            return GetInterpolatedValue(rowValue, OutOfRangeBehaviour.ReturnNaN);
        }


        public double[] GetInterpolatedValues(double[] rowValues)
        {
            return GetInterpolatedValues(rowValues, OutOfRangeBehaviour.ReturnNaN);
        }

        public double[] GetInterpolatedValues( double[] rowValues, OutOfRangeBehaviour outOfRangeBehaviour)
        {
            var length = rowValues.Length;
            var results = new double[length];
            var startIndex = 0;
            var lastValue = double.MaxValue;
            var minRowValue = MinimumRowValue();
            var maxRowValue = MaximumRowValue();

            for (var i = 0; i < length; i++)
            {
                var rowValue = rowValues[i];

                if (rowValue.IsLower(minRowValue) || rowValue.IsGreater(maxRowValue))
                {
                    switch (outOfRangeBehaviour)
                    {
                        case OutOfRangeBehaviour.ReturnNaN:
                            results[i] = double.NaN;
                            continue;
                        case OutOfRangeBehaviour.ThrowException:
#pragma warning disable S112 // General exceptions should never be thrown
                            throw new IndexOutOfRangeException($"{nameof(rowValue)}: {rowValue}");
#pragma warning restore S112 // General exceptions should never be thrown
                        case OutOfRangeBehaviour.ReturnMinOrMax:
                            rowValue = rowValue.IsLower(minRowValue) ? minRowValue : maxRowValue;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(outOfRangeBehaviour), outOfRangeBehaviour, null);
                    }
                }

                if(rowValue.IsLower(lastValue))
                    startIndex = 0;

                var row2 = Array.FindIndex(_rows, startIndex, r => rowValue.IsLowerOrEqual(r));
                var row1 = _rows[row2].IsEqual(rowValue) ? row2 : row2 - 1;

                startIndex = row2;
                lastValue = rowValue;

                if (row2 == row1)
                    results[i] = _valueTable[row1];
                else
                    results[i] = _valueTable[row1] + (_valueTable[row2] - _valueTable[row1]) * (rowValue - _rows[row1]) / (_rows[row2] - _rows[row1]);
            }

            return results;
        }
    }
}