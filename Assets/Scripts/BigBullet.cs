using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements.Experimental;

public class BigBullet : MonoBehaviour
{
    [SerializeField] private GameObject alienBit; 
    [SerializeField] private float lifetime = 5f; 
    private float timer = 0f; 

    private bool _launched = false; 
    public bool Launched { 
        get => _launched; 
        set {
            gameObject.GetComponent<Rigidbody>().mass = 10f;
            _launched = value; 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_launched)
        {
            timer += Time.deltaTime; 

            if (timer >= lifetime)
            {
                Explode(); 
            }
        }
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
        if (!col.CompareTag("Alien") && !col.CompareTag("Player") && !col.CompareTag("Alien Bit") && !col.CompareTag("Bullet"))
        {
            Explode(); 
        }
    }
}
