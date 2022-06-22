using JaroWinklerRecordSearch.Model;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch
{
    [PublicAPI]
    public readonly struct ScoreMatrix
    {
        public ScoreMatrix(FieldPos[] rowKeys, int[] colKeys, double[,] m)
        {
            RowKeys = rowKeys;
            ColKeys = colKeys;
            M = m;
        }

        public FieldPos[] RowKeys { get; }
        public int[] ColKeys { get; }
        public double[,] M { get; }

        public bool IsAmbiguousScoreMatrix()
        {
            var (nRows, nCols) = (M.GetLength(0), M.GetLength(1));
            var rowHasValue = new bool[nRows];
            var colHasValue = new bool[nCols];
            for (var r = 0; r < nRows; r++)
            {
                for (var c = 0; c < nCols; c++)
                {
                    if (M[r, c] == 0.0)
                        continue;
                    if (rowHasValue[r] || colHasValue[c])
                        return false;
                    rowHasValue[r] = true;
                    colHasValue[c] = true;
                }
            }
            return true;
        }
		
        public void Deconstruct(out FieldPos[] rowKeys, out int[] colKeys, out double[,] m)
        {
            rowKeys = RowKeys;
            colKeys = ColKeys;
            m = M;
        }
    }
}