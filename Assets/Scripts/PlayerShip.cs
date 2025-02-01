using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] private GameObject bullet; 
    [SerializeField] private AlienShipManager manager; 
    [SerializeField] private float speed = 3.0f; 
    [SerializeField] private float bulletSpeed = 1200.0f; 

    public AlienShipManager Manager {set => manager = value; get => manager; }

    private Vector3 forceVector; 
    private Bullet activeBullet;

    private MeshRenderer meshRenderer; 

    // Start is called before the first frame update
    void Start()
    {
        forceVector.x = speed;  
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
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

    public void Die()
    {
        manager.NotifyPlayerDestroyed(); 
        Destroy(gameObject); 
    }

    public void StartBlinking()
    {
        StartCoroutine(BlinkCoroutine());
    }

    public float blinkDuration = 2f; // How long the object will blink
    public float blinkInterval = 0.1f; // Speed of blinking

    private IEnumerator BlinkCoroutine()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            meshRenderer.enabled = !meshRenderer.enabled; // Toggle visibility
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        meshRenderer.enabled = true; // Ensure object is visible after blinking
    }

    void Update()
    {
        // Check if bullet has been destroyed before allowing another fire
        if (Input.GetButtonDown("Fire1") && (activeBullet == null || !activeBullet.isAlive) )
        {
            Vector3 spawnPos = gameObject.transform.position;
            var obj = Instantiate(bullet, spawnPos, Quaternion.identity);

            activeBullet = obj.GetComponent<Bullet>(); 
            activeBullet.destroyOnCollision = true; 

            obj.GetComponent<Bullet>().Thrust = new Vector3(0, 0, bulletSpeed);
            Physics.IgnoreCollision(obj.GetComponent<Collider>(), GetComponent<Collider>());
        }

        if (Input.GetButtonDown("Fire2"))
        {
            manager.KillAll(); 
        }
    }
}
