using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 thrust; 
    public Quaternion heading; 

    // Start is called before the first frame update
    void Start()
    {
        thrust.z = 1200.0f; 
        GetComponent<Rigidbody>().drag = 0; 
        // GetComponent<Rigidbody>().MoveRotation(heading); 
        GetComponent<Rigidbody>().AddRelativeForce(thrust); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision) 
    {
        Collider collider = collision.collider; 
        if (collider.CompareTag("Alien"))
        {
            AlienShip ship = collider.gameObject.GetComponent<AlienShip>(); 
            ship.Die(); 
            Destroy(gameObject); 
        }
        else{
            Debug.Log("Collided with " + collider.tag); 
        }
    }
}
