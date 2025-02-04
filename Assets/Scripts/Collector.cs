using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private AlienShipManager _manager; 
    public AlienShipManager Manager { get => _manager; set => _manager = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Alien Bit")) Debug.Log(collider.tag); 
    }
}
