using System;
using System.Data;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.Helper
{
    [PublicAPI]
    public static class DataTableExtensions
    {
        public static DataTable ToDataTable<TR, TC>(this (TR[] rowKeys, TC[] colKeys, double[,] m) crm, Func<TC, string> termLookup)
        {
            var (rowKeys, colKeys, m) = crm;
            var table = new DataTable();
            table.Columns.Add("pos", typeof(string));
            for (var ri = 0; ri < rowKeys.Length; ri++)
            {
                var dr = table.NewRow();
                dr["pos"] = $"{rowKeys[ri]}";
                for (var ci = 0; ci < colKeys.Length; ci++)
                {
                    var c = termLookup(colKeys[ci]); //
                    if (!table.Columns.Contains(c))
                        table.Columns.Add(c, typeof(double));
                    dr[c] = m[ri, ci];
                }
                table.Rows.Add(dr);
            }
            return table;
        }
    }
}
