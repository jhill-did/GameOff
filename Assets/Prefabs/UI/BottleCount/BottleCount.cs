using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottleCount : MonoBehaviour {
    public BottleInventory inventory;
    public Transform bottleIcon;
    public TextMeshProUGUI countLabel;
    public Animator textAnimation;

    public AnimationCurve lerpCurve;
    public Vector3 activePosition;
    public Vector3 inactivePosition;

    public float holdTimer = 0.0f;
    private float activeTime = 0.0f;
    
    private int previousCount = 0;

    private void Start() {
        this.UpdateLabel();
    }

    void Update() {
        var currentCount = inventory.bottlesCollected;
        if (currentCount != previousCount) {
            previousCount = currentCount;

            textAnimation.SetTrigger("CollectBottle");
            UpdateLabel();

            holdTimer = Mathf.Clamp(this.holdTimer + 2.0f, 0.0f, 2.0f);
        }

        holdTimer = Mathf.Clamp(holdTimer - Time.deltaTime, 0.0f, 2.0f);

        bottleIcon.localRotation = Quaternion.Euler(Vector3.up * Time.time * 240.0f);

        activeTime += holdTimer > 0.0f
            ? 4.0f * Time.deltaTime
            : -4.0f * Time.deltaTime;

        activeTime = Mathf.Clamp01(activeTime);

        (this.transform as RectTransform).anchoredPosition = Vector3
            .Lerp(inactivePosition, activePosition, lerpCurve.Evaluate(activeTime));
    }

    void UpdateLabel() {
        countLabel.text = $"{inventory.bottlesCollected} / {BottleInventory.maxBottles}";
    }
}
