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
        var player = collision.gameObject.GetComponent<Character>();
        if (player != null)
        {
            Vector3 spawnPosition = GetComponent<BoxCollider>().transform.position;
            player.spawnPosition = spawnPosition;
        }
    }
}
