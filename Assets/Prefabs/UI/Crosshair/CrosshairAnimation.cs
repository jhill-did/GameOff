using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairAnimation : MonoBehaviour {
    public Character player;
    public Image crosshair;
    public Image glidingCrosshair;
    public float fadeRate;
    private float opacity;

    private float glidingCrosshairOpacity;

    void Update() {
        opacity = player.chargingLaunch
            ? Mathf.Clamp01(opacity + fadeRate * Time.deltaTime)
            : Mathf.Clamp01(opacity - fadeRate * Time.deltaTime);

        glidingCrosshairOpacity = player.movementMode == MovementMode.Glide
            ? Mathf.Clamp01(glidingCrosshairOpacity + fadeRate * Time.deltaTime)
            : Mathf.Clamp01(glidingCrosshairOpacity - fadeRate * Time.deltaTime);

        var charge = player.launchHoldTimer;

        crosshair.material.SetFloat("Charge", charge);
        crosshair.material.SetFloat("Opacity", opacity);
        glidingCrosshair.color = new Color(1.0f, 1.0f, 1.0f, glidingCrosshairOpacity);
    }
}
