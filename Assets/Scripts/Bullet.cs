using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 _thrust = Vector3.zero; 
    public Vector3 Thrust { get => _thrust; set => _thrust = value; }
    public Quaternion heading; 

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().drag = 0; 
        GetComponent<Rigidbody>().AddRelativeForce(Thrust); 
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
            Destroy(gameObject); 
        }
    }
}
