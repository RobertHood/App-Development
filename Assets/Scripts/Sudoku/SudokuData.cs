using System.Collections.Generic;
using UnityEngine;

public class SudokuGridData : MonoBehaviour
{
    public static List<SudokuData.SudokuBoardData> GetData(int k = 40)
    {
        List<SudokuData.SudokuBoardData> data = new List<SudokuData.SudokuBoardData>();
        int[,] solvedGrid = new int[9, 9];
        fillDiagonal(solvedGrid);
        fillRemaining(solvedGrid, 0, 0);
   
        int[,] unsolvedGrid = (int[,])solvedGrid.Clone();
        removeKDigits(unsolvedGrid, k);

        int[] solvedFlat = new int[81];
        int[] unsolvedFlat = new int[81];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                int index = i * 9 + j;
                solvedFlat[index] = solvedGrid[i, j];
                unsolvedFlat[index] = unsolvedGrid[i, j];
            }
        }

        data.Add(new SudokuData.SudokuBoardData(unsolvedFlat, solvedFlat));
        return data;
    }

    static bool unUsedInBox(int[,] grid, int rowStart, int colStart, int num)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid[rowStart + i, colStart + j] == num)
                    return false;
            }
        }
        return true;
    }

    static void fillBox(int[,] grid, int row, int col)
    {
        System.Random rand = new System.Random();
        int num;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                do
                {
                    num = rand.Next(1, 10);
                } while (!unUsedInBox(grid, row, col, num));
                grid[row + i, col + j] = num;
            }
        }
    }

    static bool unUsedInRow(int[,] grid, int i, int num)
    {
        for (int j = 0; j < 9; j++)
        {
            if (grid[i, j] == num)
                return false;
        }
        return true;
    }

    static bool unUsedInCol(int[,] grid, int j, int num)
    {
        for (int i = 0; i < 9; i++)
        {
            if (grid[i, j] == num)
                return false;
        }
        return true;
    }

    static bool checkIfSafe(int[,] grid, int i, int j, int num)
    {
        return unUsedInRow(grid, i, num) &&
               unUsedInCol(grid, j, num) &&
               unUsedInBox(grid, i - i % 3, j - j % 3, num);
    }

    static void fillDiagonal(int[,] grid)
    {
        for (int i = 0; i < 9; i += 3)
        {
            fillBox(grid, i, i);
        }
    }

    static bool fillRemaining(int[,] grid, int i, int j)
    {
        if (i == 9)
            return true;

        if (j == 9)
            return fillRemaining(grid, i + 1, 0);

        if (grid[i, j] != 0)
            return fillRemaining(grid, i, j + 1);

        for (int num = 1; num <= 9; num++)
        {
            if (checkIfSafe(grid, i, j, num))
            {
                grid[i, j] = num;
                if (fillRemaining(grid, i, j + 1))
                    return true;
                grid[i, j] = 0;
            }
        }
        return false;
    }

    static void removeKDigits(int[,] grid, int k)
    {
        System.Random rand = new System.Random();
        while (k > 0)
        {
            int cellId = rand.Next(81);
            int i = cellId / 9;
            int j = cellId % 9;
            if (grid[i, j] != 0)
            {
                grid[i, j] = 0;
                k--;
            }
        }
    }
}



public class SudokuData : MonoBehaviour
{
    public static SudokuData Instance;

    public struct SudokuBoardData
    {
        public int[] unsolved_data;
        public int[] solved_data;

        public SudokuBoardData(int[] unsolved, int[] solved) : this()
        {
            this.unsolved_data = unsolved;
            this.solved_data = solved;
        }
    };

    public Dictionary<string, List<SudokuBoardData>> Sudoku_game = new Dictionary<string, List<SudokuBoardData>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(this);
    }
    void Start()
    {
        Sudoku_game.Add("Default", SudokuGridData.GetData());
    }
}
