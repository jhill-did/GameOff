using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public float spinSpeed;
    public float launchMagnitude;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        this.transform.Rotate(Vector3.forward, Time.deltaTime * spinSpeed);
    }

    void OnTriggerEnter(Collider collision)
    {
        var player = collision.gameObject.GetComponent<Character>();
        if (player != null)
        {
            var launchForce = this.transform.forward * launchMagnitude;
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(launchForce, ForceMode.Impulse);
        }
    }
}
