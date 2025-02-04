using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] private GameObject bullet; 
    [SerializeField] private GameObject bigBullet; 
    [SerializeField] private AlienShipManager manager; 
    [SerializeField] private float speed = 3.0f; 
    [SerializeField] private float bulletSpeed = 1200.0f; 

    public AlienShipManager Manager {set => manager = value; get => manager; }

    private Vector3 forceVector; 
    private Bullet activeBullet;

    private MeshRenderer meshRenderer; 
    private Color originalColor;

    private GameObject currentlyAttracted = null; 

    // Radius and force for attracting objects
    [SerializeField] private float attractionRadius = 5f;
    [SerializeField] private float attractionForce = 10f;
    [SerializeField] private Vector3 attractionOffset = Vector3.zero; 
    [SerializeField] private float launchForce = 1f; 

    // Start is called before the first frame update
    void Start()
    {
        forceVector.x = speed;  
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | 
                                                RigidbodyConstraints.FreezePositionZ | 
                                                RigidbodyConstraints.FreezePositionY;
        
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.GetColor("_Color"); 
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

        // Check if we're holding the Down arrow in FixedUpdate (for physics)
        if (Input.GetKey(KeyCode.DownArrow) && currentlyAttracted != null)
        {
            PullObjectTowardPlayer(currentlyAttracted.GetComponent<Rigidbody>());
        }
    }

    public void Die()
    {
        // manager.NotifyPlayerDestroyed(); 
        // Destroy(gameObject); 
    }

    #region TODO: might use this later

    public void StartBlinking()
    {
        // StartCoroutine(BlinkCoroutine());
    }

    public float blinkDuration = 2f; // How long the object will blink
    public float blinkInterval = 0.1f; // Speed of blinking

    private IEnumerator BlinkCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            meshRenderer.enabled = !meshRenderer.enabled; // Toggle visibility
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        meshRenderer.enabled = true; // Ensure object is visible after blinking
    }

    #endregion

    void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (currentlyAttracted == null)
            {
                currentlyAttracted = FindNearestObject();
            }
            meshRenderer.material.SetColor("_Color", Color.blue);
        }

        if (currentlyAttracted != null && Vector3.Distance(transform.position + attractionOffset, currentlyAttracted.transform.position) < 1.5f)
        {
            if (!currentlyAttracted.CompareTag("BigBullet"))
            {
                GameObject replacement = Instantiate(bigBullet);

                replacement.transform.position = currentlyAttracted.transform.position;  // Preserve world position
                replacement.transform.rotation = currentlyAttracted.transform.rotation;  // Preserve world rotation
                replacement.transform.localScale = currentlyAttracted.transform.lossyScale;  // Preserve world scale

                currentlyAttracted.GetComponent<AlienShip>().CurrentSprite.transform.SetParent(replacement.transform); 

                currentlyAttracted.SetActive(false);
                currentlyAttracted = replacement;
            }
            
        }

        // 2. On key-up, launch it in the +Z direction
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            meshRenderer.material.SetColor("_Color", originalColor); 
            if (currentlyAttracted != null)
            {
                BigBullet b = currentlyAttracted.GetComponent<BigBullet>(); 
                if (b is not null)
                {
                    b.Launched = true; 
                    LaunchObjectInZ(currentlyAttracted.GetComponent<Rigidbody>());
                    currentlyAttracted = null; // We’re done with it
                }
            }
        }

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

    // <summary>
    /// Finds the nearest rigidbody within `attractionRadius`.
    /// </summary>
    private GameObject FindNearestObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attractionRadius);

        float minDist = float.MaxValue;
        GameObject nearestBody = null;

        foreach (var collider in hitColliders)
        {
            GameObject rb = collider.gameObject;
            if (rb.GetComponent<AlienShip>() != null && rb.gameObject != gameObject)
            {
                float dist = Vector3.Distance(transform.position, rb.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestBody = rb;
                }
            }
        }

        return nearestBody;
    }

    /// <summary>
    /// Pull the object toward the player with a "magnet" force.
    /// </summary>
    private void PullObjectTowardPlayer(Rigidbody targetBody)
    {
        Vector3 direction = (transform.position + attractionOffset - targetBody.transform.position).normalized;
        targetBody.AddForce(direction * attractionForce, ForceMode.Force);
    }

    /// <summary>
    /// Launch the object in the +Z direction (relative to our world space).
    /// </summary>
    private void LaunchObjectInZ(Rigidbody targetBody)
    {
        // Clear any leftover velocity so it doesn't affect the launch
        targetBody.velocity = Vector3.zero;

        // Add an impulse or velocity in world +Z direction
        Vector3 launchDir = new Vector3(0, 0, 1); // or transform.forward if you want local ship's forward
        targetBody.AddForce(launchDir * launchForce);
    }
}
