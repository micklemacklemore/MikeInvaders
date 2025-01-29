using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AlienShip : MonoBehaviour
{
    public float speed = 1.0f;
    public float targetDistance = 8.0f;  

    public AlienShipManager manager = null;

    public int rowIndex = 0; 
    public int columnIndex = 0; 

    public GameObject bullet; 

    public int score = 10; 

    private float direction = 1.0f; // right
    private Vector3 directionVector; 
    private Vector3 targetVector; 
    private bool changeDirection = false; 
    private Rigidbody rb; 
    public bool ufo = false; 

    // Start is called before the first frame update
    void Start()
    {
        directionVector = new Vector3(direction, 0.0f, 0.0f); 
        rb = GetComponent<Rigidbody>(); 
        rb.drag = 0; 
    }

    void FixedUpdate()
    {
        if (changeDirection && targetVector.z > transform.position.z)
        {
            direction *= -1.0f; 
            directionVector = new Vector3(direction, 0.0f, 0.0f); 
            changeDirection = false; 
        }

        rb.MovePosition(transform.position + directionVector * Time.fixedDeltaTime * speed); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Shoot(float bulletSpeed)
    {
        Vector3 spawnPos = gameObject.transform.position;
        var activeBullet = Instantiate(bullet, spawnPos, Quaternion.identity);
        activeBullet.GetComponent<Bullet>().Thrust = new Vector3(0, 0, -bulletSpeed);
        Physics.IgnoreCollision(activeBullet.GetComponent<Collider>(), GetComponent<Collider>());
    }

    public void ChangeDirection()
    {
        changeDirection = true; 

        // approach the player
        directionVector = new Vector3(0.0f, 0.0f, -1.0f); 
        targetVector = transform.position + new Vector3(0.0f, 0.0f, -targetDistance); 
    }

    public void Stop()
    {
        directionVector = Vector3.zero; 
    }

    public void Die(bool notify = true)
    {
        Destroy(gameObject); 
        if (manager != null && notify)
        {
            manager.NotifyShipDestroyed(rowIndex, columnIndex, score, ufo);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider collider = collision.collider; 

        if (collider.CompareTag("Wall"))
        {
            if (manager is not null)
            {
                manager.AdvanceShips(); 
            }
        } else if (collider.CompareTag("UFOWall"))
        {
            Die(false); 
        }
    }
}
