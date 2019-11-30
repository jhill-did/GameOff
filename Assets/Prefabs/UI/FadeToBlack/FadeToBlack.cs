using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour {
    public GameObject player;
    public Image image;
    public float fadeHeightStart;
    public float fadeHeightEnd;

    void Update() {
        float fadeAmount = Math.Smoothstep(
            fadeHeightStart,
            fadeHeightEnd,
            player.transform.position.y
        );

        Debug.Log(fadeAmount);
        image.color = Color.Lerp(Color.clear, Color.black, fadeAmount);
    }
}
