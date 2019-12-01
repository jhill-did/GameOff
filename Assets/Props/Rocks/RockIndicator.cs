using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RockIndicator : MonoBehaviour
{
    public PhysicMaterial rockPhysicsMaterial;
    public Material rockMaterial;

    public GameObject bottle;
    public float rockPartMass;
    bool isDestroyed = false;

    public AudioClip rockImpactSound;
    public AudioMixerGroup audioMixerRockGroup; 


    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<MeshRenderer>().material = rockMaterial;
        }
    }


    public IEnumerator breakApart()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<MeshCollider>().convex = true;
            child.gameObject.AddComponent<Rigidbody>();
            child.gameObject.GetComponent<Rigidbody>().mass = rockPartMass;
            child.gameObject.GetComponent<MeshRenderer>().material = rockMaterial;
            child.gameObject.GetComponent<MeshCollider>().material = rockPhysicsMaterial;

            //add rockpiece script to the each gameobject
            child.gameObject.AddComponent<RockPiece>();
            //add audio source for each component; 
            child.gameObject.AddComponent<AudioSource>();
            var rockAudioSource = child.gameObject.GetComponent<AudioSource>();
            rockAudioSource.clip = rockImpactSound;
            rockAudioSource.outputAudioMixerGroup = audioMixerRockGroup;
        }
        yield return new WaitForSeconds(0.01f);
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        var floatingScript = GetComponent<FloatingRock>();
        Destroy(floatingScript);
    }
    // Update is called once per frame
    void Update()
    { 
        if(bottle == null && !isDestroyed)
        {
            StartCoroutine(breakApart());
            isDestroyed = true;
        }
    }
}
