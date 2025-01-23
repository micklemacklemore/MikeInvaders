using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienShip : MonoBehaviour
{
    public float speed = 1.0f;
    public float targetDistance = 8.0f;  

    public AlienShipManager manager = null; 

    private float direction = 1.0f; // right
    private Vector3 directionVector; 
    private Vector3 targetVector; 
    private bool changeDirection = false; 
    private Rigidbody rb; 
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

    void OnCollisionEnter(Collision collision)
    {
        Collider collider = collision.collider; 

        if (collider.CompareTag("Wall"))
        {
            Debug.Log("Hit Wall"); 
            if (manager is not null)
            {
                manager.AdvanceShips(); 
            }
        }
    }
}
