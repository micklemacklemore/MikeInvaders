using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienShipManager : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    public GameObject prefab;

    [Header("Grid Settings")]
    public int numberOfRows = 5;
    public int numberOfColumns = 5;

    [Header("Spawn Range")]
    // This defines how wide the X range is
    public float xRange = 10f; 
    // This defines how wide the Z range is
    public float zRange = 10f;

    [Header("Ship Global Settings")]
    public float shipSpeed = 1.0f; 
    public float approachDistance = 5.0f; 

    private List<GameObject> spawnedObjects = new List<GameObject>(); 

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan; 
        Vector3 size = new Vector3(xRange, 1, zRange); 
        Vector3 pos = transform.position; 
        Gizmos.DrawWireCube(pos, size); 
    }

    private void Start()
    {
        SpawnGrid();
    }

    public void AdvanceShips()
    {
        Debug.Log("AdvanceShip"); 
        foreach (GameObject obj in spawnedObjects)
        {
            AlienShip ship = obj.GetComponent<AlienShip>(); 
            ship.ChangeDirection(); 
        }
    }

    private void SpawnGrid()
    {
        if (prefab == null)
        {
            Debug.LogWarning("No prefab assigned to GridSpawner on " + gameObject.name);
            return;
        }

        // We’ll use Lerp from -range/2 to +range/2 for a symmetrical spread
        for (int row = 0; row < numberOfRows; row++)
        {
            for (int col = 0; col < numberOfColumns; col++)
            {
                // Normalize progress (0 to 1) along the row/column
                float xPercent = numberOfColumns > 1 
                    ? col / (float)(numberOfColumns - 1) 
                    : 0.5f; // Avoid division by zero if only 1 column

                float zPercent = numberOfRows > 1 
                    ? row / (float)(numberOfRows - 1) 
                    : 0.5f; // Avoid division by zero if only 1 row

                // Determine the position by interpolating between -range/2 and +range/2
                float xPos = Mathf.Lerp(-xRange / 2f, xRange / 2f, xPercent);
                float zPos = Mathf.Lerp(-zRange / 2f, zRange / 2f, zPercent);

                // Construct the final world position
                Vector3 spawnPos = new Vector3(xPos, 0f, zPos) + transform.position;

                // Create the prefab instance
                GameObject clone = Instantiate(prefab, spawnPos, Quaternion.identity);
                // (Optional) parent the spawned object to keep scene hierarchy organized
                clone.transform.SetParent(transform);
                AlienShip clonePrefab = clone.GetComponent<AlienShip>(); 
                clonePrefab.manager = this; 
                clonePrefab.speed = shipSpeed; 
                clonePrefab.targetDistance = approachDistance; 
                spawnedObjects.Add(clone); 
            }
        }
    }
}
