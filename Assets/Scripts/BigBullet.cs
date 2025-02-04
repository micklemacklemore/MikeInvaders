using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class BigBullet : MonoBehaviour
{
    [SerializeField] private GameObject alienBit; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Explode()
    {
        foreach (Transform child in gameObject.transform.GetChild(0))
        {
            if (!child.gameObject.activeSelf)
            {
                continue; 
            }
            GameObject replacement = Instantiate(alienBit); 

            replacement.transform.position = child.position;
            replacement.transform.rotation = child.rotation;  // Preserve world rotation
            replacement.transform.localScale = child.lossyScale;  // Preserve world scale

            replacement.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.blue); 

            child.gameObject.SetActive(false); 
        }

        Destroy(gameObject); 
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider col = collision.collider; 
        if (!col.CompareTag("Player") && !col.CompareTag("Alien Bit") && !col.CompareTag("Bullet"))
        {
            AlienShip ship = col.gameObject.GetComponent<AlienShip>(); 
            if (ship is not null && ship.isDead) return; 

            Explode(); 
        }
    }
}
