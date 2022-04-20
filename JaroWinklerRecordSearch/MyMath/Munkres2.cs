using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace JaroWinklerRecordSearch.MyMath
{
    /// <summary>
	///   Hungarian method for solving the assignment problem, also known
	///   as the Kuhn–Munkres algorithm or Munkres assignment algorithm. 
	/// </summary>
	/// 
	///   
	/// <remarks>
	/// <para>
	///   The Hungarian method is a combinatorial optimization algorithm that solves the assignment
	///   problem in polynomial time and which anticipated later primal-dual methods. It was developed
	///   and published in 1955 by Harold Kuhn, who gave the name "Hungarian method" because the algorithm 
	///   was largely based on the earlier works of two Hungarian mathematicians: Dénes Kőnig and Jenő
	///   Egerváry.</para>
	/// <para>
	///   James Munkres reviewed the algorithm in 1957 and observed that it is (strongly) polynomial. 
	///   Since then the algorithm has been known also as the Kuhn–Munkres algorithm or Munkres assignment
	///   algorithm.The time complexity of the original algorithm was O(n^4), however Edmonds and Karp, and 
	///   independently Tomizawa noticed that it can be modified to achieve an O(n^3) running time. Ford and 
	///   Fulkerson extended the method to general transportation problems. In 2006, it was discovered that
	///   Carl Gustav Jacobi had solved the assignment problem in the 19th century, and the solution had been
	///   published posthumously in 1890 in Latin.</para>
	/// 
	/// <para>
	///   This code has been based on the original MIT-licensed code by R.A. Pilgrim, available in 
	///   http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html, and on the BSD-licensed code
	///   by Yi Cao, available in http://fr.mathworks.com/matlabcentral/fileexchange/20652-hungarian-algorithm-for-linear-assignment-problems--v2-3-
	/// </para>
	///   
	/// <para>
	///   References:
	///   <list type="bullet">
	///     <item><description><a href="http://fr.mathworks.com/matlabcentral/fileexchange/20652-hungarian-algorithm-for-linear-assignment-problems--v2-3-">
	///       Yi Cao (2011). Hungarian Algorithm for Linear Assignment Problems (V2.3). Available in http://fr.mathworks.com/matlabcentral/fileexchange/20652-hungarian-algorithm-for-linear-assignment-problems--v2-3- </a></description></item>
	///     <item><description><a href="http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html">
	///       R. A. Pilgrim (2000). Munkres' Assignment Algorithm Modified for 
	///       Rectangular Matrices. Available in http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html </a></description></item>
	///     <item><description><a href="https://en.wikipedia.org/wiki/Hungarian_algorithm">
	///       Wikipedia contributors. "Hungarian algorithm." Wikipedia, The Free Encyclopedia.
	///       Wikipedia, The Free Encyclopedia, 23 Jan. 2016. </a></description></item>
	///   </list>
	/// </para>   
	/// </remarks>
	/// 
	/// <example>
	/// <code source="Unit Tests\Accord.Tests.Math\Optimization\MunkresTest.cs" region="doc_example" />
	/// </example>
	/// 
	[PublicAPI]
	public class Munkres2
	{

        private readonly double[,] _costMatrix;
        private readonly double[,] _validCost;
		private readonly bool[,] _stars;
		private readonly bool[] _rowCover;
		private readonly bool[] _colCover;
		internal readonly int[] StarZ;
		internal readonly int[] PrimeZ;

        private int _pathRow0;
		private int _pathCol0;

		private readonly int _n;

		/// <summary>
		///   Gets the minimum values across the cost matrix's rows.
		/// </summary>
		/// 
		public double[] MinRow { get; private set; } = null!;

        /// <summary>
		///   Gets the minimum values across the cost matrix's columns.
		/// </summary>
		/// 
		public double[] MinCol { get; private set; } = null!;

        /// <summary>
		///   Gets or sets the tolerance value used when performing cost 
		///   comparisons. Default is 1e-10. If the algorithm takes too
		///   much time to finish, try decreasing this value.
		/// </summary>
		/// 
		public double Tolerance { get; set; } = 1e-10;

        /// <summary>
        ///   Gets or sets the cost matrix for this assignment algorithm. This is
        ///   a (W x T) matrix where N corresponds to the <see cref="NumberOfWorkers"/>
        ///   and T to the <see cref="NumberOfTasks"/>.
        /// </summary>
        /// 
        /// <value>The cost matrix.</value>
        /// 
        public double[,] CostMatrix => _costMatrix;

        /// <summary>
		/// Gets or sets the number of variables in this optimization problem
		/// (<see cref="NumberOfTasks"/> * <see cref="NumberOfWorkers"/>).
		/// </summary>
		/// 
		public int NumberOfVariables
		{
			get => NumberOfTasks * NumberOfWorkers;
            set => throw new InvalidOperationException();
        }

		/// <summary>
		///   Gets the number of variables (free parameters)
		///   in the optimization problem. In the assigment
		///   problem, this gives the number of jobs (or tasks)
		///   to be performed.
		/// </summary>
		/// 
		/// <value>The number of tasks in the assignment problem.</value>
		/// 
		public int NumberOfTasks => _costMatrix.Columns();

        /// <summary>
		///   Gets or sets the number of workers in the assignment algorithm.
		///   The workers are the entites that can be assigned jobs according
		///   to the costs in the <see cref="CostMatrix"/>.
		/// </summary>
		/// 
		/// <value>The number of workers.</value>
		/// 
		public int NumberOfWorkers => _costMatrix.Rows();

        /// <summary>
		/// Gets the current solution found, the values of
		/// the parameters which optimizes the function.
		/// </summary>
		/// 
		/// <value>The solution.</value>
		/// 
		public double[] Solution { get; set; }

        /// <summary>
		///   Gets the output of the function at the current <see cref="Solution" />.
		/// </summary>
		/// 
		/// <value>The value.</value>
		/// 
		public double Value { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Munkres"/> class.
		/// </summary>
		/// 
		/// <param name="costMatrix">The cost matrix where each row represents
		///   a worker, each column represents a task, and each individual element
		///   represents how much it costs for a particular worker to receive (be
		///   assigned) a particular task.
		///   Preprocesses the cost matrix to check for infinities and invalid values.</param>
		/// 
		public Munkres2(double[,] costMatrix)
        {
            _costMatrix = costMatrix;
            Solution = new double[NumberOfWorkers];
            _n = Math.Max(_costMatrix.Rows(), _costMatrix.Columns());
            var max = _costMatrix.MaxAbs();
            _validCost = _costMatrix.ToSquare(_n, 10.0 * max);
            _rowCover = new bool[_n];
            _colCover = new bool[_n];
            _stars = new bool[_n,_n];
            StarZ = Vector.Create(_n, -1);
            PrimeZ = Vector.Create(_n, -1);
        }

		/// <summary>
		///   Finds the minimum value of a function. The solution vector
		///   will be made available at the <see cref="Solution" /> property.
		/// </summary>
		/// 
		/// <returns>Returns <c>true</c> if the method converged to a <see cref="Solution" />.
		/// In this case, the found value will also be available at the <see cref="Value" />
		/// property.</returns>
		/// 
		public static bool Minimize(double[,] costMatrix)
        {
            var munkres = new Munkres2(costMatrix.Copy());
			return munkres.Run();
		}


		/// <summary>
		///   Finds the maximum value of a function. The solution vector
		///   will be made available at the <see cref="Solution" /> property.
		/// </summary>
		/// 
		/// <returns>Returns <c>true</c> if the method converged to a <see cref="Solution" />.
		/// In this case, the found value will also be available at the <see cref="Value" />
		/// property.</returns>
		/// 
		public bool Maximize(double[,] costMatrix)
		{
            var munkres = new Munkres2(costMatrix.Multiply(-1));
            return munkres.Run();
		}

		public bool Run()
		{
			var step = 1;
			while (step > 0)
			{
				step = RunStep(step);
			}
			return true;
		}


		internal int RunStep(int step)
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


        /// <summary>
		///  For each row of the cost matrix, find the smallest element
		///  and subtract it from every element in its row.
		/// </summary>
		/// 
		/// <returns>Go to step 2.</returns>
		/// 
		private int Step1()
		{
			MinRow = _validCost.Min(1);
			MinCol = _validCost.Subtract(MinRow, VectorType.ColumnVector).Min(0);

			return 2;
		}

		/// <summary>
		///   Find a zero (Z) in the resulting matrix. If there is no starred 
		///   zero in its row or column, star Z. Repeat for each element in the 
		///   matrix.
		/// </summary>
		/// 
		/// <returns>Go to step 3.</returns>
		/// 
		private int Step2()
		{
            FindZeros(_validCost, MinRow, MinCol, _stars, out _, Tolerance);

			for (var j = 0; j < _stars.GetLength(1); j++)
			{
				for (var i = 0; i < _stars.GetLength(0); i++)
				{
					if (_stars[i,j])
					{
						StarZ[i] = j;
						_stars.SetColumn(j, false);
						_stars.SetRow(i, false);
					}
				}
			}

			return 3;
		}

		internal static bool[,] FindZeros(double[,] mC, double[] minRow, double[] minCol, bool[,] zeros, out double[,] min, double atol = 1e-10)
		{
			min = mC.CreateAs();
			for (var r = 0; r < minRow.Length; r++)
				for (var c = 0; c < minCol.Length; c++)
					min[r,c] = minRow[r] + minCol[c];

			return Elementwise.Equals(mC, min, zeros, atol);
		}

		/// <summary>
		///   Cover each column containing a starred zero. If K columns are covered, 
		///   the starred zeros describe a complete set of unique assignments. In this 
		///   case, go to DONE, otherwise, go to Step 4.
		/// </summary>
		/// 
		/// <returns>If K columns are covered, returns 7. Otherwise, returns 4.</returns>
		/// 
		private int Step3()
		{
			_colCover.Clear();
			_rowCover.Clear();
			PrimeZ.Clear();

			var done = true;
            foreach (var j in StarZ)
            {
                if (j >= 0)
                    _colCover[j] = true;
                else
                    done = false;
            }

			return done ? 7 : 4;
        }


		/// <summary>
		///   Find a noncovered zero and prime it. If there is no starred zero 
		///   in the row containing this primed zero, Go to Step 5. Otherwise, 
		///   cover this row and uncover the column containing the starred zero. 
		///   Continue in this manner until there are no uncovered zeros left. 
		///   Save the smallest uncovered value and Go to Step 6.        
		/// </summary>
		/// 
		/// <returns>Goes to step 5 or 6.</returns>
		/// 
		private int Step4()
		{
			var zeros = FindAllZeros();

			while (zeros.Count > 0)
			{
				_pathRow0 = zeros[0].Item1;
				_pathCol0 = zeros[0].Item2;
				PrimeZ[_pathRow0] = _pathCol0;

				var stz = StarZ[_pathRow0];
				if (stz == -1)
					return 5;

				_rowCover[_pathRow0] = true;
				_colCover[stz] = false;

				zeros.RemoveAll(x => x.Item1 == _pathRow0);

				// Update
				for (var r = 0; r < Math.Min(_costMatrix.GetLength(0), _rowCover.Length); r++)
				{
					if (_rowCover[r])
						continue;

					var a = _costMatrix[r,stz];
					var b = MinRow[r] + MinCol[stz];

					if (Math.Abs(a - b) < Tolerance)
						zeros.Add((r, stz));
				}
			}

			return 6;
		}

		private List<(int, int)> FindAllZeros()
		{
			var zeros = new List<(int, int)>();
			for (var c = 0; c < _colCover.Length; c++)
			{
				if (_colCover[c])
					continue;

				for (var r = 0; r < _rowCover.Length; r++)
				{
					if (_rowCover[r])
						continue;

					var a = _validCost[r,c];
					var b = MinCol[c] + MinRow[r];

					if (Math.Abs(a - b) < Tolerance)
						zeros.Add((r, c));
				}
			}

			return zeros;
		}

		/// <summary>
		///   Construct a series of alternating primed and starred zeros as follows.  
		///   Let Z0 represent the uncovered primed zero found in Step 4. Let Z1 denote 
		///   the starred zero in the column of Z0 (if any). Let Z2 denote the primed zero 
		///   in the row of Z1 (there will always be one). Continue until the series 
		///   terminates at a primed zero that has no starred zero in its column.  
		///   Unstar each starred zero of the series, star each primed zero of the series, 
		///   erase all primes and uncover every line in the matrix.  
		/// </summary>
		/// 
		/// <returns>Return to Step 3.</returns>
		/// 
		private int Step5()
		{
			var rowZ1 = Array.IndexOf(StarZ, _pathCol0);
			StarZ[_pathRow0] = _pathCol0;

			while (rowZ1 >= 0)
			{
				StarZ[rowZ1] = -1;
				_pathCol0 = PrimeZ[rowZ1];
				_pathRow0 = rowZ1;
				rowZ1 = Array.IndexOf(StarZ, _pathCol0);
				StarZ[_pathRow0] = _pathCol0;
			}

			return 3;
		}


		/// <summary>
		///   Add the value found in Step 4 to every element of each covered row, and subtract 
		///   it from every element of each uncovered column.  
		/// </summary>
		/// 
		/// <returns>Return to step 4.</returns>
		/// 
		private int Step6()
		{
			var minval = FindSmallest();

			for (var r = 0; r < _rowCover.Length; r++)
				if (_rowCover[r])
					MinRow[r] -= minval;

			for (var c = 0; c < _colCover.Length; c++)
				if (!_colCover[c])
					MinCol[c] += minval;

			return 4;
		}

		//methods to support step 6
		private double FindSmallest()
		{
			var minval = double.PositiveInfinity;
			for (var c = 0; c < _colCover.Length; c++)
			{
				if (_colCover[c])
					continue;

				for (var r = 0; r < _rowCover.Length; r++)
				{
					if (_rowCover[r])
						continue;

					var v = _validCost[r,c] - (MinRow[r] + MinCol[c]);

					if (v < minval)
						minval = v;
				}
			}

			return minval;
		}

		private int Step7()
		{
			// DONE: Assignment pairs are indicated by the positions of the starred zeros in the 
			// cost matrix.If C(i, j) is a starred zero, then the element associated with row i 
			// is assigned to the element associated with column j.
			//
			//                     (http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html)
			double value = 0;

			for (var i = 0; i < _costMatrix.Rows(); i++)
			{
				Solution[i] = double.NaN;
				var j = StarZ[i];

				if (j >= 0 && j < _costMatrix.Columns())
				{
					Solution[i] = j;
					value += _costMatrix[i,j];
				}
			}
			Value = value;
			return -1;
		}
    }
}