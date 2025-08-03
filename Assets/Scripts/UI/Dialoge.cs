using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialoguer : MonoBehaviour
{
    [SerializeField] private Collider2D collider;

    [SerializeField] private String hint;

    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject bubble;

    void Update()
    {
        if (collider != null && collider.IsTouchingLayers(collisionLayers))
        {
            text.text = hint;
            bubble  .SetActive(true);
        }
        else
        {
            bubble.gameObject.SetActive(false);
        }
    }
}