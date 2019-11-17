using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Entered");
        var player = collision.gameObject.GetComponent<Character>();
        if (player != null)
        {
            Debug.Log("player entered island box");
            Vector3 spawnPosition = GetComponent<BoxCollider>().transform.position;
            player.spawnPosition = spawnPosition;
        }
    }
}
