using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class AlienShip : MonoBehaviour
{
    private float _speed = 1.0f; 
    public float speed { 
        get => _speed; 
        set {
            spriteFrequency = (1f / value); 
            _speed = value; 
        } }
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

    public GameObject[] sprites; 
    private bool spriteSwitch = false; 
    private bool animated = false;
    private float spriteTimer = 0.0f;  
    private float spriteFrequency = 1.0f; 

    public GameObject[] removeCollisionsOnDie; 
    private bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        directionVector = new Vector3(direction, 0.0f, 0.0f); 
        rb = GetComponent<Rigidbody>(); 
        if (sprites.Length != 0)
        {
            animated = true; 
        }
        rb.drag = 0; 
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
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
        spriteTimer += Time.deltaTime; 

        if (animated && spriteTimer >= spriteFrequency)
        {
            sprites[spriteSwitch ? 0 : 1].SetActive(true); 
            sprites[spriteSwitch ? 1 : 0].SetActive(false); 

            spriteSwitch = !spriteSwitch; 
            spriteTimer = 0f; 
        }
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
        // Destroy(gameObject); 
        if (!dead)
        {
            if (manager != null && notify)
            {
                manager.NotifyShipDestroyed(rowIndex, columnIndex, score, ufo);
            }

            rb.useGravity = true; 
            rb.mass = 0.1f; 
            animated = false; 
            _speed = 0f; 
            foreach (GameObject obj in removeCollisionsOnDie)
            {
                Physics.IgnoreCollision(obj.GetComponent<Collider>(), GetComponent<Collider>()); 
            }

            dead = true; 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider collider = collision.collider; 
        if (collider.CompareTag("UFOWall"))
        {
            Die(false); 
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Wall") && manager is not null)
        {
            manager.AdvanceShips(); 
        }
    }

}
