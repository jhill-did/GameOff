using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingRock : MonoBehaviour
{
    // Start is called before the first frame update
    public float rate;
    public float magnitude;
    public bool isHorizontal;
    public bool isCircular;

    float timeAccumulator = 0.0f;
    Vector3 originalPosition;
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timeAccumulator += Time.deltaTime;

        if( isCircular)
        {
            float xDistance = Mathf.Cos(timeAccumulator) * magnitude;
            float yDistance = Mathf.Sin(timeAccumulator) * magnitude;
            transform.position = new Vector3(originalPosition.x + xDistance, originalPosition.y + yDistance, originalPosition.z);
        } else
        {
            float distance = Mathf.Sin(timeAccumulator) * magnitude;
            transform.position = isHorizontal ? new Vector3(originalPosition.x + distance, originalPosition.y, originalPosition.z)
                                : new Vector3(originalPosition.x, originalPosition.y + distance, originalPosition.z);
        }
    }
}
