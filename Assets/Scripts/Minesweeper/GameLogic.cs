using Unity.VisualScripting;
using UnityEngine;
// using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;


public class GameLogic : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;

    public Button smileyBtn;
    public Button settingBtn;
    public Button flagModeBtn;
    public Sprite smileyNormal;
    public Sprite smileyWin;
    public Sprite smileyLose;
    private Image smileyImg;
    public TMP_Text flagcountTxt;
    public TMP_Text timerTxt;
    
    private float timer;
    private bool timerRunning;

    private Board board;
    private Cell[,] state;
    private bool gameover;
    private bool flagModeOn;

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
        smileyImg = smileyBtn.GetComponentInChildren<Image>();
        flagModeBtn?.onClick.AddListener(ToggleFlagMode);
    }

    private void Start()
    {
        NewGame();
        smileyBtn?.onClick.AddListener(NewGame);
    }

    private void NewGame()
    {
        gameover = false;
        state = new Cell[width, height];
        smileyImg.sprite = smileyNormal;
        GenerateCells();
        GenerateMines();
        GenerateNumbers();
        timer = 0;
        timerRunning = true;
        flagModeOn = false;

        Camera.main.transform.position = new Vector3(width / 4f, height / 4f, -10f);
        board.Draw(state);
        CountFlags();
        UpdateTimerUI();
        UpdateFlagModeUI();
        Debug.Log("New game.");
    }
    
    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }

    private void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while (state[x, y].type == Cell.Type.Mine)
            {
                x++;
                if (x >= width)
                {
                    x = 0;
                    y++;
                    if (y >= height)
                    {
                        y = 0;
                    }
                }
            }
            state[x, y].type = Cell.Type.Mine;
            // state[x, y].revealed = true;
        }
    }

    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    continue;
                }
                cell.number = CountMines(x, y);
                if (cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
                }
                // cell.revealed = true;
                state[x, y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        int cnt = 0;
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (cellX == 0 && cellY == 0)
                {
                    continue;
                }

                int x = cellX + adjacentX;
                int y = cellY + adjacentY;

                if (GetCell(x, y).type == Cell.Type.Mine)
                {
                    cnt++;
                }
            }
        }

        return cnt;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     NewGame();
        // }
        if (!gameover)
        {
            CountFlags();
            if (timerRunning)
            {
                timer += Time.deltaTime;
                UpdateTimerUI();
            }
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Flag(worldPosition);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (flagModeOn)
                {
                    Flag(worldPosition);
                }
                else
                {
                    Reveal(worldPosition);
                }
            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                if (flagModeOn)
                {
                    Flag(worldPosition);
                }
                else
                {
                    Reveal(worldPosition);
                }
            }
        }
    }

    private void ToggleFlagMode()
    {
        flagModeOn = !flagModeOn;
        if (flagModeOn)
        {
            Debug.Log("Flag mode is on.");
        }
        else
        {
            Debug.Log("Flag mode is off.");
        }
        UpdateFlagModeUI();
    }

    private void UpdateFlagModeUI()
    {
        Image flagImg = flagModeBtn.GetComponentInChildren<Image>();
        if (flagModeOn)
        {
            flagImg.color = new Color(0.7f, 0f, 0f);
        }
        else
        {
            flagImg.color = Color.white;
        }
    }

    private void CountFlags()
    {
        int cnt = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Cell cell = state[i, j];
                if (cell.revealed == false && cell.flagged == true)
                {
                    cnt++;
                }
            }
        }
        UpdateFlagUI(cnt);
    }

    private void UpdateFlagUI(int cnt)
    {
        flagcountTxt.text = $"{mineCount - cnt}";
    }

    private void UpdateTimerUI()
    {
        if (timerTxt != null)
        {
            int seconds = Mathf.FloorToInt(timer);
            timerTxt.text = $"{seconds:000}";
        }
    }
    private void Flag(Vector3 worldPosition)
    {
        // Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.revealed)
        {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;

        board.Draw(state);
    }

    private void Reveal(Vector3 worldPosition)
    {
        // Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }


        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;
            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }

        board.Draw(state);
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }
        CountFlags();
        Debug.Log("You win!");
        gameover = true;
        timerRunning = false;
        
        smileyImg.sprite = smileyWin;
    }

    private void Explode(Cell cell)
    {
        gameover = true;
        timerRunning = false;
        Debug.Log("Game Over!");
        smileyImg.sprite = smileyLose;
        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x, cell.position.y] = cell;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }
            }
        }
    }
    
    private void Flood(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        if (cell.type == Cell.Type.Empty)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    Flood(GetCell(cell.position.x + i, cell.position.y + j));
                }
            }
        }
    }

    private Cell GetCell(int x, int y)
    {
        if (isValid(x, y))
        {
            return state[x, y];
        }
        else
        {
            return new Cell();
        }
    }
    
    private bool isValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}