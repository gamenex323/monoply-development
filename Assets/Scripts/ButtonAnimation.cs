using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonAnimation : MonoBehaviour
{
    public bool scaleAnimation = true; // Toggle to enable or disable the animation
    public float animationDuration = 0.3f; // Duration of the animation
    public float scaleFactor = 1.2f; // The scale factor for the "popup" effect

    void OnEnable()
    {
        if (scaleAnimation)
        {
            // Start a popup animation when the object is enabled
            PopupAnimation();
            //
        }
    }

    void PopupAnimation()
    {
        // Save the original scale
        Vector3 originalScale = transform.localScale;

        // Animate to the larger scale, then back to the original scale
        transform.DOScale(originalScale * scaleFactor, animationDuration / 2)
            .SetEase(Ease.OutQuad) // Easing for the "scale up" effect
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, animationDuration / 2)
                    .SetEase(Ease.InQuad); // Easing for the "scale down" effect
            });
    }
}
