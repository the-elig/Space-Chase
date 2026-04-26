using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderCode : MonoBehaviour
{
    Image image;
    Material m;
    CardVisual visual;

    void Start()
    {
        image = GetComponent<Image>();
        m = new Material(image.material);
        image.material = m;
        visual = GetComponentInParent<CardVisual>();

        // Disable all shader keywords to remove the holo effect
        for (int i = 0; i < image.material.enabledKeywords.Length; i++)
        {
            image.material.DisableKeyword(image.material.enabledKeywords[i]);
        }

        // Set to REGULAR with no extras
        image.material.EnableKeyword("_EDITION_REGULAR");
    }

    void Update()
    {
        // Rotation shader update removed to prevent holo shimmer
    }
}