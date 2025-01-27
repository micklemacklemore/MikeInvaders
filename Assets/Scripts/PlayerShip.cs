using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] private GameObject bullet; 
    [SerializeField] private float speed = 3.0f; 
    private Vector3 forceVector; 

    // Start is called before the first frame update
    void Start()
    {
        forceVector.x = speed; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            GetComponent<Rigidbody>().AddRelativeForce(forceVector); 
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            GetComponent<Rigidbody>().AddRelativeForce(-forceVector); 
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire!"); 
            Vector3 spawnPos = gameObject.transform.position; 
            GameObject obj = Instantiate(bullet, spawnPos, Quaternion.identity); 
            Physics.IgnoreCollision(obj.GetComponent<Collider>(), GetComponent<Collider>());
        } 
    }
}
