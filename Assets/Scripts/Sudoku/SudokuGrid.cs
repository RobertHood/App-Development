using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SudokuGrid : MonoBehaviour
{
    public int columns = 0;
    public int rows = 0;
    public float square_offset = 0.0f;
    public GameObject grid_square;
    public Vector2 start_position = new Vector2(0.0f, 0.0f);
    public float square_scale = 1.0f;
    private List<GameObject> grid_squares_ = new List<GameObject>();
    private int selected_grid_data = -1;



    void Start()
    {
        if (grid_square.GetComponent<GridSquare>() == null)
        {
            Debug.LogError("no grid square");
        }

        StartCoroutine(InitializeGrid());
        
    }

    private IEnumerator InitializeGrid() //delay
    {
        yield return null;

        CreateGrid();
        SetGridNumber("Default");
    }

    private void SetGridNumber(string level)
    {
        selected_grid_data = 0;
        var data = SudokuData.Instance.Sudoku_game[level][selected_grid_data];

        setGridSquareData(data);
        // foreach (var square in grid_squares_)
        // {
        //     square.GetComponent<GridSquare>().SetNumber(Random.Range(0, 10));
        // }
    }

    private void setGridSquareData(SudokuData.SudokuBoardData data)
    {
        for (int i = 0; i < grid_squares_.Count; i++)
        {
            grid_squares_[i].GetComponent<GridSquare>().SetNumber(data.unsolved_data[i]);
        }
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetSquarePosition();
        
    }

    private void SetSquarePosition()
    {
        var square_rect = grid_squares_[0].GetComponent<RectTransform>();
        Vector2 offset = new Vector2();
        offset.x = square_rect.rect.width * square_rect.transform.localScale.x + square_offset; // second square is offsetted by the amount equals to the size of the first square
        offset.y = square_rect.rect.height * square_rect.transform.localScale.y + square_offset;

        int column_number = 0;
        int row_number = 0;

        foreach(GameObject square in grid_squares_)
        {
            if (column_number + 1 > columns)
            {
                row_number++;
                column_number = 0;
            }

            var pos_x_offset = offset.x * column_number;
            var pos_y_offset = offset.y * row_number;

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(start_position.x + pos_x_offset, start_position.y - pos_y_offset);
            column_number++;
        }
    }

    private void SpawnGridSquares()
    {
        int square_index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                grid_squares_.Add(Instantiate(grid_square) as GameObject);
                grid_squares_[grid_squares_.Count - 1].GetComponent<GridSquare>().SetSquareIndex(square_index);
                grid_squares_[grid_squares_.Count - 1].transform.SetParent(this.transform, false);
                grid_squares_[grid_squares_.Count - 1].transform.localScale = new Vector3(square_scale, square_scale, square_scale);

                square_index++;
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
