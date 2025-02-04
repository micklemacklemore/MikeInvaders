using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class AlienShipManager : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    public GameObject alienShipLowPrefab;
    public GameObject alienShipMidPrefab;
    public GameObject alienShipHighPrefab;
    public GameObject playerShipPrefab; 
    public GameObject ufoPrefab; 
    public GameObject ufoSpawner; 

    [Header("UI Prefab")]
    public TMP_Text textScore; 
    public TMP_Text textLives; 
    public TMP_Text textYouWin; 

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
    public float shootFrequency = 5f; // The target time for the timer
    public float UFOFrequency = 10f; 
    public float shootSpeed = 1200f;

    [SerializeField] private Camera camera2D; 
    [SerializeField] private Camera camera3D; 

    [SerializeField] private GameObject[] walls; 
    [SerializeField] private Collector[] collectors; 

    private Camera activeCamera;

    private AlienShip[,] gridShips; 
    private List<AlienShip> bottomRow; 

    private PlayerShip player; 

    private AudioSource audioSource; 

    private float timer = 0f; // The time elapsed
    private float ufoTimer = 0f; 

    private int totalScore; 
    private int lives = 3; 

    private int shipsLeft; 

    private bool gameRunning; 

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan; 
        Vector3 size = new Vector3(xRange, 1, zRange); 
        Vector3 pos = transform.position; 
        Gizmos.DrawWireCube(pos, size); 
    }

    private void Start()
    {
        gridShips = new AlienShip[numberOfRows, numberOfColumns]; 
        audioSource = GetComponent<AudioSource>(); 

        GameManager.EnsureInstance(); 
        totalScore = GameManager.Instance.Score; 

        textScore.text = "Score: " + totalScore; 
        textLives.text = "Lives: " + lives; 
        SpawnPlayer(); 
        SpawnGrid();
        bottomRow = GetBottomRow(); 

        // Set the initial active camera
        activeCamera = camera2D;
        camera2D.gameObject.SetActive(true);
        camera3D.gameObject.SetActive(false);

        foreach (Collector collector in collectors)
        {
            collector.Manager = this; 
        }

        gameRunning = true; 
    }

    public void KillAll()
    {
        foreach (AlienShip ship in gridShips)
        {
            if (ship is not null)
            {
                ship.Die(null, false);
                shipsLeft = 0;  
            }
        }
    }

    void SwitchCamera()
    {
        if (activeCamera == camera2D)
        {
            camera2D.gameObject.SetActive(false);
            camera3D.gameObject.SetActive(true);
            activeCamera = camera3D;
        }
        else
        {
            camera3D.gameObject.SetActive(false);
            camera2D.gameObject.SetActive(true);
            activeCamera = camera2D;
        }
    }

    private void Update()
    { 
        if (Input.GetKeyDown(KeyCode.C)) // Press 'C' to switch
        {
            SwitchCamera();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); 
        }

        if (gameRunning)
        {
            timer += Time.deltaTime; 
            ufoTimer += Time.deltaTime; 

            if (timer >= shootFrequency)
            {
                int random = Random.Range(0, 3); 

                AlienShip ship = null; 

                if (random < 2)
                {
                    ship = GetClosestShipToPlayer(player.gameObject.transform.position.x);
                }
                else
                {
                    int idx = Random.Range(0, bottomRow.Count);
                    ship = bottomRow[idx];
                }

                ship.GetComponent<Renderer>().material.SetColor("_Color", Color.red); 
                ship.Shoot(shootSpeed); 

                timer = 0; 
            }

            if (ufoTimer >= UFOFrequency)
            {
                SpawnUFO(); 

                ufoTimer = 0; 
            }

            if (lives == 0)
            {
                TriggerGameOver(); 
            } 
            else if (shipsLeft == 0)
            {
                TriggerWin(); 
            }
        }
        
    }

    private void SpawnUFO()
    {
        GameObject clone = Instantiate(ufoPrefab, ufoSpawner.transform.position, Quaternion.AngleAxis(90, new Vector3(0, 0, 1f)));
        AlienShip ship = clone.GetComponent<AlienShip>(); 
        ship.speed = 8f;
        ship.score = 300; 
        ship.ufo = true; 
        ship.manager = this; 
    }

    private void TriggerGameOver()
    {
        Debug.Log("Game Over"); 
        Destroy(player); 
        foreach (AlienShip alien in gridShips)
        {
            if (alien != null)
            {
                alien.Die(null, false); 
            }
        }

        gameRunning = false; 
        GameManager.Instance.Score = totalScore; 
        GameManager.Instance.LoadGameOver(); 
    }

    private void TriggerWin()
    {
        textYouWin.text = "You Win!";
        gameRunning = false; 

        GameManager.Instance.Score = totalScore; 
        GameManager.Instance.LoadNextWave(); 
    }

    private AlienShip GetClosestShipToPlayer(float playerX)
    {
        AlienShip closestShip = null;
        float closestDistance = Mathf.Infinity;

        foreach (AlienShip ship in bottomRow)
        {
            float distance = Mathf.Abs(ship.transform.position.x - playerX);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestShip = ship;
            }
        }

        return closestShip;
    }

    private void DebugGetBottomRow(Color color)
    {
        foreach (AlienShip ship in bottomRow)
        {
            var render = ship.GetComponent<Renderer>(); 
            render.material.SetColor("_Color", color);
        }
    }

    public void NotifyShipDestroyed(int row, int column, int score, bool ufo)
    {
        audioSource.Play(); 
        if (!ufo)
        {
            gridShips[row, column] = null; 
            bottomRow = GetBottomRow(); 
            IncreaseShipSpeed(); 
            shipsLeft--; 
        }
        totalScore += score; 
        textScore.text = "Score: " + totalScore; 
    }

    public void IncreaseShipSpeed()
    {
        shipSpeed += 0.075f; 
        foreach (AlienShip ship in gridShips)
        {
            if (ship is not null)
            {
                ship.speed = shipSpeed; 
            }
        }
    }

    public void NotifyPlayerDestroyed()
    {
        lives--; 
        textLives.text = "Lives: " + lives; 

        // SpawnPlayer(); 

        // player.StartBlinking(); 
    }

    public void SpawnPlayer()
    {
        // spawn new player
        GameObject clone = Instantiate(playerShipPrefab, Vector3.zero, Quaternion.identity);
        PlayerShip clonePrefab = clone.GetComponent<PlayerShip>(); 
        clonePrefab.Manager = this; 
        player = clonePrefab; 
    }

    public List<AlienShip> GetBottomRow()
    {
        List<AlienShip> lastRow = new List<AlienShip>(); 
        for (int col = numberOfColumns - 1; col >= 0; col--)
        {
            AlienShip lastShip = null;  
            for (int row = numberOfRows - 1; row >= 0; row--)
            {
                AlienShip ship = gridShips[row, col];
                if (ship is not null)
                {
                    lastShip = ship; 
                }
            }

            if (lastShip is not null)
            {
                lastRow.Add(lastShip); 
            }
        }

        return lastRow; 
    }

    public void AdvanceShips()
    {
        foreach (AlienShip ship in gridShips)
        {
            if (ship is not null)
            {
                ship.ChangeDirection(); 
            }
        }
    }

    private void SpawnGrid()
    {
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
                GameObject toSpawn = null; 

                if (row <= 1)
                {
                    toSpawn = alienShipLowPrefab; 
                }
                else if (row <= 3)
                {
                    toSpawn = alienShipMidPrefab; 
                }
                else
                {
                    toSpawn = alienShipHighPrefab; 
                }

                GameObject clone = Instantiate(toSpawn, spawnPos, Quaternion.identity);
                // (Optional) parent the spawned object to keep scene hierarchy organized
                clone.transform.SetParent(transform);
                AlienShip clonePrefab = clone.GetComponent<AlienShip>(); 
                clonePrefab.manager = this; 
                clonePrefab.speed = shipSpeed; 
                clonePrefab.targetDistance = approachDistance; 

                clonePrefab.rowIndex = row; 
                clonePrefab.columnIndex = col; 

                clonePrefab.removeCollisionsOnDie = new GameObject[walls.Length]; 

                for (int i = 0; i < walls.Length; i++)
                {
                    clonePrefab.removeCollisionsOnDie[i] = walls[i]; 
                }

                // add alien ship to grid
                gridShips[row, col] = clonePrefab; 
            }
        }

        shipsLeft = numberOfRows * numberOfColumns; 
    }
}
