namespace FeWoLearning.Exercises.Beginner;

// Exercise 027 — Matrix Indexer (reference solution).
public class MatrixIndexer
{
    private readonly int[,] _values;

    public MatrixIndexer(int rows, int cols) => _values = new int[rows, cols];

    public int Rows => _values.GetLength(0);

    public int Cols => _values.GetLength(1);

    public int this[int row, int col]
    {
        get => _values[row, col];
        set => _values[row, col] = value;
    }
}
