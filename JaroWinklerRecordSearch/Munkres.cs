using System;
using System.Text;
using static System.FormattableString;

namespace Munkres
{
    // This is the original code from http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html
    // with minimal modifications. The only included modifications aim to increase the debugability
    // of the original code and did not alter its behavior.

    public class MunkresProgram
    {
        public double[,] C = {};
        public double[,] M = {};
        public int[,] Path = {}; // int[2 * MAX + 1, 2]
        public int[] RowCover = {};
        public int[] ColCover = {};
        public int NRow;
        public int NCol;
        public int PathCount;
        public int PathRow0;
        public int PathCol0;

        public StringBuilder Log = new();
        public void Console_WriteLine(FormattableString formattable) => Log.AppendLine(Invariant(formattable));
        public void Console_Write(FormattableString formattable) => Log.Append(Invariant(formattable));

        private void ResetMaskAndCovers()
        {
            for (var r = 0; r < NRow; r++)
            {
                RowCover[r] = 0;
                for (var c = 0; c < NCol; c++)
                {
                    M[r, c] = 0;
                }
            }
            for (var c = 0; c < NCol; c++)
                ColCover[c] = 0;
        }

        //For each row of the cost matrix, find the smallest element and subtract
        //it from every element in its row.  When finished, Go to Step 2.
        private int Step1()
        {
            for (var r = 0; r < NRow; r++)
            {
                var minInRow = C[r, 0];
                for (var c = 0; c < NCol; c++)
                    if (C[r, c] < minInRow)
                        minInRow = C[r, c];
                for (var c = 0; c < NCol; c++)
                    C[r, c] -= minInRow;
            }
            return 2;
        }

        //Find a zero (Z) in the resulting matrix.  If there is no starred 
        //zero in its row or column, star Z. Repeat for each element in the 
        //matrix. Go to Step 3.
        private int Step2()
        {
            for (var r = 0; r < NRow; r++)
                for (var c = 0; c < NCol; c++)
                {
                    if (C[r, c] == 0 && RowCover[r] == 0 && ColCover[c] == 0)
                    {
                        M[r, c] = 1;
                        RowCover[r] = 1;
                        ColCover[c] = 1;
                    }
                }
            for (var r = 0; r < NRow; r++)
                RowCover[r] = 0;
            for (var c = 0; c < NCol; c++)
                ColCover[c] = 0;
            return 3;
        }

        //Cover each column containing a starred zero.  If K columns are covered, 
        //the starred zeros describe a complete set of unique assignments.  In this 
        //case, Go to DONE, otherwise, Go to Step 4.
        private int Step3()
        {
            for (var r = 0; r < NRow; r++)
                for (var c = 0; c < NCol; c++)
                    if (Math.Abs(M[r, c] - 1) < TOLERANCE)
                        ColCover[c] = 1;

            var colCount = 0;
            for (var c = 0; c < NCol; c++)
                if (ColCover[c] == 1)
                    colCount += 1;
            if (colCount >= NCol || colCount >= NRow)
                return 7;
            return 4;
        }

        //methods to support step 4
        private (int row,int col) FindZero()
        {
            for (var r = 0; r < NRow; r++)
                for (var c = 0; c < NCol; c++)
                    if (C[r, c] == 0 && RowCover[r] == 0 && ColCover[c] == 0)
                        return (r, c);
            return (-1, -1);
        }

        private bool StarInRow(int row)
        {
            for (var c = 0; c < NCol; c++)
                if (Math.Abs(M[row, c] - 1) < TOLERANCE)
                    return true;
            return false;
        }

        // ReSharper disable once InconsistentNaming
        public static double TOLERANCE = 1e-10;

        private int FindStarInRow(int row)
        {
            var col = -1;
            for (var c = 0; c < NCol; c++)
                if (Math.Abs(M[row, c] - 1) < TOLERANCE)
                    col = c;
            return col;
        }

        //Find a noncovered zero and prime it.  If there is no starred zero 
        //in the row containing this primed zero, Go to Step 5.  Otherwise, 
        //cover this row and uncover the column containing the starred zero. 
        //Continue in this manner until there are no uncovered zeros left. 
        //Save the smallest uncovered value and Go to Step 6.
        private int Step4()
        {
            for (;;)
            {
                var (row, col) = FindZero();
                if (row == -1)
                    return 6;

                M[row, col] = 2;
                var col2 = FindStarInRow(row);
                if (col2 >= 0)
                {
                    RowCover[row] = 1;
                    ColCover[col2] = 0;
                }
                else
                {
                    PathRow0 = row;
                    PathCol0 = col;
                    return 5;
                }
            }
        }

        // methods to support step 5
        private void find_star_in_col(int c, ref int r)
        {
            r = -1;
            for (var i = 0; i < NRow; i++)
                if (Math.Abs(M[i, c] - 1) < TOLERANCE)
                    r = i;
        }

        private void find_prime_in_row(int r, ref int c)
        {
            for (var j = 0; j < NCol; j++)
                if (Math.Abs(M[r, j] - 2) < TOLERANCE)
                    c = j;
        }

        private void augment_path()
        {
            for (var p = 0; p < PathCount; p++)
                if (Math.Abs(M[Path[p, 0], Path[p, 1]] - 1) < TOLERANCE)
                    M[Path[p, 0], Path[p, 1]] = 0;
                else
                    M[Path[p, 0], Path[p, 1]] = 1;
        }

        private void clear_covers()
        {
            for (var r = 0; r < NRow; r++)
                RowCover[r] = 0;
            for (var c = 0; c < NCol; c++)
                ColCover[c] = 0;
        }

        private void erase_primes()
        {
            for (var r = 0; r < NRow; r++)
                for (var c = 0; c < NCol; c++)
                    if (Math.Abs(M[r, c] - 2) < TOLERANCE)
                        M[r, c] = 0;
        }


        //Construct a series of alternating primed and starred zeros as follows.  
        //Let Z0 represent the uncovered primed zero found in Step 4.  Let Z1 denote 
        //the starred zero in the column of Z0 (if any). Let Z2 denote the primed zero 
        //in the row of Z1 (there will always be one).  Continue until the series 
        //terminates at a primed zero that has no starred zero in its column.  
        //Unstar each starred zero of the series, star each primed zero of the series, 
        //erase all primes and uncover every line in the matrix.  Return to Step 3.
        private int Step5()
        {
            var r = -1;
            var c = -1;

            PathCount = 1;
            Path[PathCount - 1, 0] = PathRow0;
            Path[PathCount - 1, 1] = PathCol0;
            for (;;)
            {
                find_star_in_col(Path[PathCount - 1, 1], ref r);
                if (r <= -1)
                    break;
                PathCount += 1;
                Path[PathCount - 1, 0] = r;
                Path[PathCount - 1, 1] = Path[PathCount - 2, 1];

                find_prime_in_row(Path[PathCount - 1, 0], ref c);
                PathCount += 1;
                Path[PathCount - 1, 0] = Path[PathCount - 2, 0];
                Path[PathCount - 1, 1] = c;
            }
            augment_path();
            clear_covers();
            erase_primes();
            return 3;
        }

        //methods to support step 6
        private void find_smallest(ref double minval)
        {
            for (var r = 0; r < NRow; r++)
                for (var c = 0; c < NCol; c++)
                    if (RowCover[r] == 0 && ColCover[c] == 0)
                        if (minval > C[r, c])
                            minval = C[r, c];
        }

        //Add the value found in Step 4 to every element of each covered row, and subtract 
        //it from every element of each uncovered column.  Return to Step 4 without 
        //altering any stars, primes, or covered lines.
        private int Step6()
        {
            var minval = double.MaxValue;
            find_smallest(ref minval);
            for (var r = 0; r < NRow; r++)
                for (var c = 0; c < NCol; c++)
                {
                    if (RowCover[r] == 1)
                        C[r, c] += minval;
                    if (ColCover[c] == 0)
                        C[r, c] -= minval;
                }
            return 4;
        }

        private int Step7()
        {
            Console_WriteLine($"\n\n---------Run Complete----------");
            return -1;
        }

        public void RunMunkres(double[,] c)
        {
            Init(c);

            var step = 0;
            while (step >= 0)
            {
                ShowCostMatrix(step);
                ShowMaskMatrix();
                step = RunStep(step);
            }
        }

        private void Init(double[,] c)
        {
            C = c;
            NRow = C.GetLength(0);
            NCol = C.GetLength(1);
            M = new double[NRow, NCol];
            ColCover = new int[NCol];
            RowCover = new int[NRow];
            Path = new int[2 * Math.Max(NRow, NCol) + 1, 2];
            ResetMaskAndCovers();
        }

        private int RunStep(int step)
        {
            return step switch
            {
                1 => Step1(),
                2 => Step2(),
                3 => Step3(),
                4 => Step4(),
                5 => Step5(),
                6 => Step6(),
                7 => Step7(),
                _ => throw new InvalidOperationException()
            };
        }

        private void ShowCostMatrix(int step)
        {
            Console_WriteLine($"\n");
            Console_WriteLine($"------------Step {step}-------------");
            for (var r = 0; r < NRow; r++)
            {
                Console_WriteLine($"");
                Console_Write($"     ");
                for (var c = 0; c < NCol; c++)
                {
                    Console_Write($"{C[r, c]} ");
                }
            }
        }

        private void ShowMaskMatrix()
        {
            Console_WriteLine($"");
            Console_Write($"\n    ");
            for (var c = 0; c < NCol; c++)
                Console_Write($" {ColCover[c]}");
            for (var r = 0; r < NRow; r++)
            {
                Console_Write($"\n  {RowCover[r]}  ");
                for (var c = 0; c < NCol; c++)
                {
                    Console_Write($"{M[r, c]} ");
                }
            }
        }

    }

}
