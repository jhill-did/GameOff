using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<MeshCollider>().convex = true;
            //child is your child transform
            child.gameObject.AddComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    { 
    
    }
}
