using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    [Header("Tilemap & Tile Assets")]
    public Tilemap tilemap;
    public TileBase highlightTile;
    public TileBase originalTile;

    [Header("Scoring")]
    [SerializeField] private int scoreMultiplier = 1;
    [SerializeField] private int playerScore;
    public TextMeshProUGUI score;
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverUi;
    private bool isGameOver = false;
    private Vector3 currentDragWorldPos;
    public int minX , maxX;
    public int minY, maxY;

    private List<Vector3Int> previousPreview = new List<Vector3Int>();

    private Vector3Int gridMin = new Vector3Int(-2, -6, 0);
    private Vector3Int gridMax = new Vector3Int(5, 1, 0);

    public Transform placedBlock;

    private Dictionary<Vector3Int, int> gridMap = new Dictionary<Vector3Int, int>();

    private GameObject objectBeingDragged;
    public GameObject blockSpawner;
    private BlockSpawner bs;

    public PlayFabLeaderBoard PlayFabLeaderBoard;
    private void Awake()
    {
        if (gameOverUi != null)
            gameOverUi.SetActive(false);
    }
    void Start()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
        if (bs == null && blockSpawner != null)
            bs = blockSpawner.GetComponent<BlockSpawner>();

        if (gameOverUi != null)
            gameOverUi.SetActive(false);
    }
    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd * scoreMultiplier;
        score.text = playerScore.ToString();
    }

    public void UpdateScore() {
        score.text = playerScore.ToString();
    }
    void Update()
    {
        if (objectBeingDragged == null) return;

        Vector3 mouseWorld = currentDragWorldPos;

        Vector3Int[] previewCells = GetPreviewCells(objectBeingDragged, mouseWorld);

        ClearHighlight();

        List<Vector3Int> validCells = new List<Vector3Int>(previewCells);
        bool canPlace = true;
        foreach (var cell in previewCells)
        {
            if (!IsInsideGrid(cell) || !IsCellFree(cell))
            {
                canPlace = false;
                break;
            }
        }

        if (canPlace)
        {
            SetHighlight(validCells);
        }
    }

    public void StartDrag(GameObject block)
    {
        objectBeingDragged = block;
    }

    public void EndDrag(GameObject block)
    {
        if (objectBeingDragged != block) return;

        Vector3 mouseWorld = currentDragWorldPos;
        mouseWorld.z = 0;
        Vector3Int[] targetCells = GetPreviewCells(block, mouseWorld);

        foreach (var cell in targetCells)
        {
            if (!IsInsideGrid(cell) || !IsCellFree(cell))
            {
                block.transform.position = block.GetComponent<BlockData>().originPos;
                objectBeingDragged = null;
                ClearHighlight();
                return;
            }
        }

        SnapToGrid(block, targetCells);
        objectBeingDragged = null;
        ClearHighlight();
    }
    public void UpdateDragPosition(Vector3 worldPos)
    {
        currentDragWorldPos = worldPos;
    }
    
    private void SnapToGrid(GameObject block, Vector3Int[] targetCells)
    {
        Vector3 firstCellCenter = tilemap.GetCellCenterWorld(targetCells[0]);
        
        Vector3 offset = firstCellCenter - block.GetComponent<BlockData>().cells[0].position;

        block.GetComponent<BlockData>().isLocked = true;
        if (block.GetComponent<Collider2D>() != null)
            block.GetComponent<Collider2D>().enabled = false;

        block.transform.position += offset;
        block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, 0);
        block.transform.SetParent(placedBlock);
        block.transform.localScale = Vector3.one * 0.9f;
        foreach (var cell in targetCells)
            SetGridPosValue(cell, 1);
        addScore(block.transform.childCount);
        CheckAndClear();
        AudioManager.Instance?.PlayPlaceBlock();
    }

    private void ClearHighlight()
    {
        foreach (var pos in previousPreview)
        {
            if (tilemap.GetTile(pos) == highlightTile)
                tilemap.SetTile(pos, originalTile);
        }
        previousPreview.Clear();
    }

    private void SetHighlight(List<Vector3Int> cells)
    {
        foreach (var pos in cells)
        {
            if (tilemap.GetTile(pos) == originalTile)
            {
                tilemap.SetTile(pos, highlightTile);
                previousPreview.Add(pos);
            }
        }
    }

    public Vector3Int[] GetPreviewCells(GameObject block, Vector3 mouseWorld)
    {
        BlockData data = block.GetComponent<BlockData>();
        Vector3Int[] positions = new Vector3Int[data.cells.Count];

        Vector3Int mouseCell = tilemap.WorldToCell(mouseWorld);
        Vector3 cellCenter = tilemap.GetCellCenterWorld(mouseCell);

        Vector3 anchorLocal = data.cells[0].localPosition;

        for (int i = 0; i < data.cells.Count; i++)
        {
            Vector3 localOffset = data.cells[i].localPosition - anchorLocal;
            Vector3 worldOffset = block.transform.TransformVector(localOffset);
            Vector3 worldPos = cellCenter + worldOffset;
            positions[i] = tilemap.WorldToCell(worldPos);
        }
        return positions;
    }

    private void SetGridPosValue(Vector3Int gridPos, int v) => gridMap[gridPos] = v;
    public bool IsCellFree(Vector3Int gridPos) => GetGridPosValue(gridPos) == 0;
    public int GetGridPosValue(Vector3Int gridPos) => gridMap.TryGetValue(gridPos, out int value) ? value : 0;

    public bool IsInsideGrid(Vector3Int pos)
    {
        return pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY;
    }

    private void CheckAndClear()
    {
        HashSet<Vector3Int> cellsToDelete = new HashSet<Vector3Int>();

        CollectFullRows(cellsToDelete);
        CollectFullCols(cellsToDelete);

        if (cellsToDelete.Count > 0)
        {
            ClearCells(cellsToDelete);
            addScore(8 * cellsToDelete.Count / 10);
        }

        if (!CheckAllBlockPlaceable())
        {
            Debug.Log("Game Over: No place for new blocks.");
            GameOver();
        }
    }

    private void CollectFullRows(HashSet<Vector3Int> toDelete)
    {
        for (int y = minY; y <= maxY; y++)
        {
            bool full = true;
            for (int x = minX; x <= maxX; x++)
            {
                if (!gridMap.TryGetValue(new Vector3Int(x, y, 0), out int val) || val == 0)
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                Debug.Log("Row full at y=" + y);
                for (int x = minX; x <= maxX; x++)
                    toDelete.Add(new Vector3Int(x, y, 0));
            }
        }
    }

    private void CollectFullCols(HashSet<Vector3Int> toDelete)
    {
        for (int x = minX; x <= maxX; x++)
        {
            bool full = true;
            for (int y = minY; y <= maxY; y++)
            {
                if (!gridMap.TryGetValue(new Vector3Int(x, y, 0), out int val) || val == 0)
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                Debug.Log("Col full at x=" + x);
                for (int y = minY; y <= maxY; y++)
                    toDelete.Add(new Vector3Int(x, y, 0));
            }
        }
    }

    private void ClearCells(HashSet<Vector3Int> toDelete)
    {
        foreach (BlockData block in FindObjectsByType<BlockData>(FindObjectsSortMode.None))
        {
            var cellsCopy = new List<Transform>(block.cells);
            foreach (Transform cell in cellsCopy)
            {
                Vector3Int cellPos = tilemap.WorldToCell(cell.position);
                if (toDelete.Contains(cellPos) && block.transform.parent == placedBlock)
                {
                    Destroy(cell.gameObject);
                    gridMap[cellPos] = 0;
                    block.cells.Remove(cell);
                }
            }

            if (block.cells.Count == 0)
                Destroy(block.gameObject);
        }
    }

    private bool CheckAllBlockPlaceable()
    {
        List<Vector3Int> availableCells = new List<Vector3Int>();
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (IsCellFree(pos))
                    availableCells.Add(pos);
            }
        }

        if (bs == null && blockSpawner != null) bs = blockSpawner.GetComponent<BlockSpawner>();
        if (bs == null)
        {
            return true;
        }

        List<GameObject> spawnedBlocks = bs.GetCurrentBlocks();
        if (spawnedBlocks == null || spawnedBlocks.Count == 0)
        {
            return true;
        }
        string diagBlock = null;
        Vector3Int diagAnchor = new Vector3Int();
        Vector3Int diagFailCell = new Vector3Int();
        int diagFailVal = -1;
        TileBase diagTile = null;

        foreach (GameObject block in spawnedBlocks)
        {
            if (block == null) continue;

            var bd = block.GetComponent<BlockData>();
            if (bd != null && bd.isLocked)
            {
                continue;
            }

            foreach (Vector3Int cell in availableCells)
            {
                Vector3 cellWorld = tilemap.GetCellCenterWorld(cell);
                Vector3Int[] targetCells = GetPreviewCellsAtGrid(block, cell);
                bool canPlace = true;
                foreach (var c in targetCells)
                {
                    if (!IsInsideGrid(c) || !IsCellFree(c))
                    {
                        canPlace = false;
                        if (diagBlock == null)
                        {
                            diagBlock = block.name;
                            diagAnchor = cell;
                            diagFailCell = c;
                            diagFailVal = GetGridPosValue(c);
                            if (tilemap != null)
                                diagTile = tilemap.GetTile(c);
                        }
                        break;
                    }
                }

                if (canPlace)
                {
                    return true;
                }
            }
        }

        string blockList = string.Join(", ", spawnedBlocks.ConvertAll(b => b!=null?b.name:"null"));
        if (diagBlock != null)
        {
            Debug.Log($"CheckAllBlockPlaceable diagnostic: first failure block={diagBlock}, anchor={diagAnchor}, failCell={diagFailCell}, gridVal={diagFailVal}, tile={diagTile}");
        }
        return false;
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0;

        AudioManager.Instance?.PlayGameOver();
        if (gameOverUi != null) {
            gameOverUi.SetActive(true);
            PlayFabLeaderBoard.sendLeaderboard(playerScore, "block blast"); 
        }
        else
            Debug.LogWarning("GridManager: GameOver called but 'gameOverUi' is not assigned.");
    }
    public void RestartGame(){
        isGameOver = false;
        playerScore = 0;
        UpdateScore();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToHomeScreen()
    {
        SceneManager.LoadScene("All Game Menu");
    }

    public void Continue(GameObject pauseUI)
    {
        pauseUI.SetActive(false);
    }

    public void CallPauseUI(GameObject pauseUI)
    {
        pauseUI.SetActive(true);
    }
    public void ResetBoard()
    {

        if (placedBlock != null)
        {
            for (int i = placedBlock.childCount - 1; i >= 0; i--)
            {
                Destroy(placedBlock.GetChild(i).gameObject);
            }
        }

        gridMap.Clear();

        if (tilemap != null && originalTile != null)
        {
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), originalTile);
                }
            }
        }

        playerScore = 0;
        UpdateScore();
        if (bs == null && blockSpawner != null) bs = blockSpawner.GetComponent<BlockSpawner>();
        bs?.SpawnBlock();
    }

    public Vector3Int[] GetPreviewCellsAtGrid(GameObject block, Vector3Int anchorCell)
    {
        BlockData data = block.GetComponent<BlockData>();
        HashSet<Vector3Int> positions = new HashSet<Vector3Int>();

        List<Transform> sourceCells = null;
        if (data != null && data.cells != null && data.cells.Count > 0)
        {
            sourceCells = data.cells;
        }
        else
        {
            sourceCells = new List<Transform>();
            foreach (Transform child in block.transform)
                sourceCells.Add(child);
        }

        if (sourceCells.Count == 0)
        {
            Debug.LogWarning($"GetPreviewCellsAtGrid: no cells found on '{block.name}'. Returning empty.");
            return System.Array.Empty<Vector3Int>();
        }

        Transform firstCell = sourceCells[0];
        Vector3 localAnchor = firstCell.localPosition;

        foreach (Transform cell in sourceCells)
        {
            Vector3 localOffset = cell.localPosition - localAnchor;

            Vector3 gridOffset = new Vector3(
                localOffset.x / tilemap.cellSize.x,
                localOffset.y / tilemap.cellSize.y,
                0
            );

            Vector3Int offset = new Vector3Int(
                Mathf.RoundToInt(gridOffset.x),
                Mathf.RoundToInt(gridOffset.y),
                0
            );

            Vector3Int target = anchorCell + offset;

            positions.Add(target);
        }

        return new List<Vector3Int>(positions).ToArray();
    }

    public void setScoreMultiplier(int newScoreMultiplier)
    {
        scoreMultiplier += newScoreMultiplier;
    }

    public void ClearBoard()
    {
        if (placedBlock != null)
        {
            for (int i = placedBlock.childCount - 1; i >= 0; i--)
            {
                Destroy(placedBlock.GetChild(i).gameObject);
                addScore(1);
            }
        }

        gridMap.Clear();

        if (tilemap != null && originalTile != null)
        {
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), originalTile);
                }
            }
        }
    }
}
