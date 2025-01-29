using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] private GameObject bullet; 
    [SerializeField] private float speed = 3.0f; 
    [SerializeField] private float bulletSpeed = 1200.0f; 
    private Vector3 forceVector; 
    private GameObject activeBullet;

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
        // Check if bullet has been destroyed before allowing another fire
        if (Input.GetButtonDown("Fire1") && activeBullet == null)
        {
            Vector3 spawnPos = gameObject.transform.position;
            activeBullet = Instantiate(bullet, spawnPos, Quaternion.identity);
            activeBullet.GetComponent<Bullet>().Thrust = new Vector3(0, 0, bulletSpeed);
            Physics.IgnoreCollision(activeBullet.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
