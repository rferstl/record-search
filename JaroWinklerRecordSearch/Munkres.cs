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
        public int Nrow;
        public int Ncol;
        public int PathCount;
        public int PathRow0;
        public int PathCol0;
        public int Asgn = 0;
        public int Step;

        public StringBuilder Log = new();
        public void Console_WriteLine(FormattableString formattable) => Log.AppendLine(Invariant(formattable));
        public void Console_Write(FormattableString formattable) => Log.Append(Invariant(formattable));

        private void ResetMaskandCovers()
        {
            for (var r = 0; r < Nrow; r++)
            {
                RowCover[r] = 0;
                for (var c = 0; c < Ncol; c++)
                {
                    M[r, c] = 0;
                }
            }
            for (var c = 0; c < Ncol; c++)
                ColCover[c] = 0;
        }

        //For each row of the cost matrix, find the smallest element and subtract
        //it from every element in its row.  When finished, Go to Step 2.
        private void Step_one(ref int step)
        {
            for (var r = 0; r < Nrow; r++)
            {
                var minInRow = C[r, 0];
                for (var c = 0; c < Ncol; c++)
                    if (C[r, c] < minInRow)
                        minInRow = C[r, c];
                for (var c = 0; c < Ncol; c++)
                    C[r, c] -= minInRow;
            }
            step = 2;
        }

        //Find a zero (Z) in the resulting matrix.  If there is no starred 
        //zero in its row or column, star Z. Repeat for each element in the 
        //matrix. Go to Step 3.
        private void step_two(ref int step)
        {
            for (var r = 0; r < Nrow; r++)
                for (var c = 0; c < Ncol; c++)
                {
                    if (C[r, c] == 0 && RowCover[r] == 0 && ColCover[c] == 0)
                    {
                        M[r, c] = 1;
                        RowCover[r] = 1;
                        ColCover[c] = 1;
                    }
                }
            for (var r = 0; r < Nrow; r++)
                RowCover[r] = 0;
            for (var c = 0; c < Ncol; c++)
                ColCover[c] = 0;
            step = 3;
        }

        //Cover each column containing a starred zero.  If K columns are covered, 
        //the starred zeros describe a complete set of unique assignments.  In this 
        //case, Go to DONE, otherwise, Go to Step 4.
        private void step_three(ref int step)
        {
            for (var r = 0; r < Nrow; r++)
                for (var c = 0; c < Ncol; c++)
                    if (Math.Abs(M[r, c] - 1) < TOLERANCE)
                        ColCover[c] = 1;

            var colcount = 0;
            for (var c = 0; c < Ncol; c++)
                if (ColCover[c] == 1)
                    colcount += 1;
            if (colcount >= Ncol || colcount >= Nrow)
                step = 7;
            else
                step = 4;
        }

        //methods to support step 4
        private void find_a_zero(ref int row, ref int col)
        {
            var r = 0;
            row = -1;
            col = -1;
            var done = false;
            while (!done)
            {
                var c = 0;
                while (true)
                {
                    if (C[r, c] == 0 && RowCover[r] == 0 && ColCover[c] == 0)
                    {
                        row = r;
                        col = c;
                        done = true;
                    }
                    c += 1;
                    if (c >= Ncol || done)
                        break;
                }
                r += 1;
                if (r >= Nrow)
                    done = true;
            }
        }

        private bool star_in_row(int row)
        {
            var tmp = false;
            for (var c = 0; c < Ncol; c++)
                if (Math.Abs(M[row, c] - 1) < TOLERANCE)
                    tmp = true;
            return tmp;
        }

        // ReSharper disable once InconsistentNaming
        public static double TOLERANCE = 1e-10;

        private void find_star_in_row(int row, ref int col)
        {
            col = -1;
            for (var c = 0; c < Ncol; c++)
                if (Math.Abs(M[row, c] - 1) < TOLERANCE)
                    col = c;
        }

        //Find a noncovered zero and prime it.  If there is no starred zero 
        //in the row containing this primed zero, Go to Step 5.  Otherwise, 
        //cover this row and uncover the column containing the starred zero. 
        //Continue in this manner until there are no uncovered zeros left. 
        //Save the smallest uncovered value and Go to Step 6.
        private void step_four(ref int step)
        {
            var row = -1;
            var col = -1;

            var done = false;
            while (!done)
            {
                find_a_zero(ref row, ref col);
                if (row == -1)
                {
                    done = true;
                    step = 6;
                }
                else
                {
                    M[row, col] = 2;
                    if (star_in_row(row))
                    {
                        find_star_in_row(row, ref col);
                        RowCover[row] = 1;
                        ColCover[col] = 0;
                    }
                    else
                    {
                        done = true;
                        step = 5;
                        PathRow0 = row;
                        PathCol0 = col;
                    }
                }
            }
        }

        // methods to support step 5
        private void find_star_in_col(int c, ref int r)
        {
            r = -1;
            for (var i = 0; i < Nrow; i++)
                if (Math.Abs(M[i, c] - 1) < TOLERANCE)
                    r = i;
        }

        private void find_prime_in_row(int r, ref int c)
        {
            for (var j = 0; j < Ncol; j++)
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
            for (var r = 0; r < Nrow; r++)
                RowCover[r] = 0;
            for (var c = 0; c < Ncol; c++)
                ColCover[c] = 0;
        }

        private void erase_primes()
        {
            for (var r = 0; r < Nrow; r++)
                for (var c = 0; c < Ncol; c++)
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
        private void step_five(ref int step)
        {
            var r = -1;
            var c = -1;

            PathCount = 1;
            Path[PathCount - 1, 0] = PathRow0;
            Path[PathCount - 1, 1] = PathCol0;
            var done = false;
            while (!done)
            {
                find_star_in_col(Path[PathCount - 1, 1], ref r);
                if (r > -1)
                {
                    PathCount += 1;
                    Path[PathCount - 1, 0] = r;
                    Path[PathCount - 1, 1] = Path[PathCount - 2, 1];

                    find_prime_in_row(Path[PathCount - 1, 0], ref c);
                    PathCount += 1;
                    Path[PathCount - 1, 0] = Path[PathCount - 2, 0];
                    Path[PathCount - 1, 1] = c;
                }
                else
                    done = true;
            }
            augment_path();
            clear_covers();
            erase_primes();
            step = 3;
        }

        //methods to support step 6
        private void find_smallest(ref double minval)
        {
            for (var r = 0; r < Nrow; r++)
                for (var c = 0; c < Ncol; c++)
                    if (RowCover[r] == 0 && ColCover[c] == 0)
                        if (minval > C[r, c])
                            minval = C[r, c];
        }

        //Add the value found in Step 4 to every element of each covered row, and subtract 
        //it from every element of each uncovered column.  Return to Step 4 without 
        //altering any stars, primes, or covered lines.
        private void step_six(ref int step)
        {
            var minval = double.MaxValue;
            find_smallest(ref minval);
            for (var r = 0; r < Nrow; r++)
                for (var c = 0; c < Ncol; c++)
                {
                    if (RowCover[r] == 1)
                        C[r, c] += minval;
                    if (ColCover[c] == 0)
                        C[r, c] -= minval;
                }
            step = 4;
        }

        private void step_seven(ref int step)
        {
            Console_WriteLine($"\n\n---------Run Complete----------");
        }

        public void RunMunkres(double[,] c)
        {
            Init(c);

            var done = false;
            while (!done)
            {
                ShowCostMatrix();
                ShowMaskMatrix();
                done = RunStep(done);
            }
        }

        private void Init(double[,] c)
        {
            C = c;
            Nrow = C.GetLength(0);
            Ncol = C.GetLength(1);
            M = new double[Nrow, Ncol];
            ColCover = new int[Ncol];
            RowCover = new int[Nrow];
            Path = new int[2 * Math.Max(Nrow, Ncol) + 1, 2];
            ResetMaskandCovers();

            Step = 1;
        }

        private bool RunStep(bool done)
        {
            switch (Step)
            {
                case 1:
                    Step_one(ref Step);
                    break;
                case 2:
                    step_two(ref Step);
                    break;
                case 3:
                    step_three(ref Step);
                    break;
                case 4:
                    step_four(ref Step);
                    break;
                case 5:
                    step_five(ref Step);
                    break;
                case 6:
                    step_six(ref Step);
                    break;
                case 7:
                    step_seven(ref Step);
                    return true;
            }
            return done;
        }

        private void ShowCostMatrix()
        {
            Console_WriteLine($"\n");
            Console_WriteLine($"------------Step {Step}-------------");
            for (var r = 0; r < Nrow; r++)
            {
                Console_WriteLine($"");
                Console_Write($"     ");
                for (var c = 0; c < Ncol; c++)
                {
                    Console_Write($"{C[r, c]} ");
                }
            }
        }

        private void ShowMaskMatrix()
        {
            Console_WriteLine($"");
            Console_Write($"\n    ");
            for (var c = 0; c < Ncol; c++)
                Console_Write($" {ColCover[c]}");
            for (var r = 0; r < Nrow; r++)
            {
                Console_Write($"\n  {RowCover[r]}  ");
                for (var c = 0; c < Ncol; c++)
                {
                    Console_Write($"{M[r, c]} ");
                }
            }
        }

    }

}
