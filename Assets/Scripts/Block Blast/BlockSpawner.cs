using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BlockSpawner : MonoBehaviour
{
    
    public List<GameObject> blockArrays = new List<GameObject>();

    private Vector3 firstSlot = new Vector3((float)-1.5, (float)-3.5, -1);
    private Vector3 secondSlot = new Vector3(0, (float)-3.5, -1);
    private Vector3 thirdSlot = new Vector3((float)1.5, (float)-3.5, -1);
    public List<GameObject> options;
    public List<GameObject> currentBlocks = new List<GameObject>();

    private GameObject GenerateSmartBlock(GridManager gm)
    {

        List<GameObject> candidates = new List<GameObject>(blockArrays);

        float totalWeight = 0f;
        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (var prefab in candidates)
        {

            int cellCount = 0;
            foreach (Transform child in prefab.transform)
                cellCount++;
            float w = 1f / Mathf.Max(cellCount, 1); 
            weights[prefab] = w;
            totalWeight += w;
        }


        float rand = Random.value * totalWeight;
        foreach (var kv in weights)
        {
            rand -= kv.Value;
            if (rand <= 0)
                return kv.Key;
        }
        return candidates[0];
    }

    private bool IsBlockPlaceable(GridManager gm, GameObject prefab)
    {

        for (int x = gm.minX; x <= gm.maxX; x++)
        for (int y = gm.minY; y <= gm.maxY; y++)
        {
            Vector3Int cell = new Vector3Int(x, y, 0);
            if (!gm.IsCellFree(cell)) continue;


            Vector3Int[] testCells = gm.GetPreviewCellsAtGrid(prefab, cell);
            bool valid = true;
            foreach (var c in testCells)
            {
                if (!gm.IsInsideGrid(c) || !gm.IsCellFree(c))
                {
                    valid = false;
                    break;
                }
            }
            if (valid) return true;
        }
        return false;
}

    public void SpawnBlock()

    {
        currentBlocks.Clear();
        GridManager gm = FindAnyObjectByType<GridManager>();


        HashSet<GameObject> uniqueCheck = new HashSet<GameObject>(blockArrays);
        if (uniqueCheck.Count < 3)
        {
            Debug.LogWarning("BlockSpawner < 3");
        }

        Vector3[] slots = { firstSlot, secondSlot, thirdSlot };
        HashSet<GameObject> usedPrefabs = new HashSet<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            GameObject prefab = null;
            int attempts = 0;


            while (attempts < 20)
            {
                attempts++;
                var candidate = GenerateSmartBlock(gm);

			    if (usedPrefabs.Contains(candidate)) continue;
                if (IsBlockPlaceable(gm, candidate))
                {
                    prefab = candidate;
                    break;
                }
            }


            if (prefab == null)
            {

                List<GameObject> remaining = new List<GameObject>();
                foreach (var p in blockArrays)
                {
                    if (!usedPrefabs.Contains(p))
                        remaining.Add(p);
                }

                if (remaining.Count > 0)
                {
                    prefab = remaining[Random.Range(0, remaining.Count)];
                }
                else
                {
                    Debug.LogWarning("BlockSpawner: Không còn prefab khác để đảm bảo 3 khối khác nhau. Dừng spawn lô này.");
                    return;
                }
            }
            GameObject block = Instantiate(prefab, transform);
            block.transform.position = slots[i];
            currentBlocks.Add(block);
            usedPrefabs.Add(prefab);
        }
    }
    void Start()
    {
        
        SpawnBlock();
    }

    
    void Update()
    {
        if (currentBlocks.Count == 0) return;

        bool allLocked = true;

        for (int i = currentBlocks.Count - 1; i >= 0; i--)
        {
            GameObject block = currentBlocks[i];

            if (block == null)
            {
                currentBlocks.RemoveAt(i);
                continue;
            }

            BlockData data = block.GetComponent<BlockData>();
            if (data != null && data.isLocked)
            {
                currentBlocks.RemoveAt(i);
            }
            else {
                allLocked = false;
            }
        }

        if (currentBlocks.Count == 0)
        {
            SpawnBlock();
        }
    }

    public List<GameObject> GetCurrentBlocks()
    {
        List<GameObject> unplacedBlocks = new List<GameObject>();
        foreach (var block in currentBlocks)
        {
            if (block != null && !block.GetComponent<BlockData>().isLocked)
            {
                unplacedBlocks.Add(block);
            }
        }
        return unplacedBlocks;
    }
}
