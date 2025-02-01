using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 _thrust = Vector3.zero; 
    public Vector3 Thrust { get => _thrust; set => _thrust = value; }
    public Quaternion heading; 

    private bool live = true; 
    public bool isAlive { get => live; }
    public bool destroyOnCollision = false; 

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().drag = 0; 
        GetComponent<Rigidbody>().AddRelativeForce(Thrust); 
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (live)
        {
            Collider collider = collision.collider; 
            if (collider.CompareTag("Alien"))
            {
                AlienShip ship = collider.gameObject.GetComponent<AlienShip>(); 
                ship.Die(gameObject.transform.position); 
            }
            else if (collider.CompareTag("BunkerPiece"))
            {
                collider.gameObject.GetComponent<MeshRenderer>().enabled = false; 
                collider.gameObject.GetComponent<BoxCollider>().enabled = false; 
            }
            else if (collider.CompareTag("Player"))
            {
                PlayerShip ship = collider.gameObject.GetComponent<PlayerShip>();
                ship.Die();  
            }

            if (destroyOnCollision)
            {
                Destroy(gameObject); 
                return; 
            }

            live = false; 
            GetComponent<Rigidbody>().useGravity = true; 
            GetComponent<Rigidbody>().mass = 0f; 
            GetComponent<Renderer>().material.SetColor("_Color", Color.grey); 
        }
        // Destroy(gameObject); 
    }
}
