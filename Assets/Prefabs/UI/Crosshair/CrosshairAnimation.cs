using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairAnimation : MonoBehaviour {
    public Character player;
    public Image crosshair;
    public float fadeRate;
    private float opacity;

    void Update() {
        opacity = player.chargingLaunch
            ? Mathf.Clamp01(opacity + fadeRate * Time.deltaTime)
            : Mathf.Clamp01(opacity - fadeRate * Time.deltaTime);

        var charge = player.launchHoldTimer;

        crosshair.material.SetFloat("Charge", charge);
        crosshair.material.SetFloat("Opacity", opacity);
    }
}
