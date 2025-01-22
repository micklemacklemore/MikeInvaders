using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] private GameObject bullet; 
    private Vector3 forceVector; 

    // Start is called before the first frame update
    void Start()
    {
        forceVector.x = 2.0f; 
    }

    // Update is called once per frame
    void Update()
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
}
