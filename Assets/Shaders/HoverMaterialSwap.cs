using System;
using Factory_Elements;
using UnityEngine;

public class HoverMaterialSwap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material hoverMaterial;
    private PlayerControls playerControls;
    private GameObject previousHit;

    private void Awake()
    {
        playerControls = new PlayerControls();
        previousHit = null;
    }

    private void OnEnable()
    {
        
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }


    // Update is called once per frame
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(playerControls.Player.MousePosition.ReadValue<Vector2>());
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
        if (hit && !(hit.collider is null))
        {
            IFactoryElement element = hit.collider.gameObject.GetComponent<IFactoryElement>();
            if (element is null)
            {
                if (previousHit is not null)
                {
                    previousHit.GetComponent<SpriteRenderer>().material = normalMaterial;
                    previousHit = null;
                }
                return;
            }

            if (previousHit == hit.collider.gameObject)
            {
                return;
            }
            
            // New hit
            if (previousHit)
            {
                previousHit.GetComponent<SpriteRenderer>().material = normalMaterial;
            }
            hit.collider.gameObject.GetComponent<SpriteRenderer>().material = hoverMaterial;
            previousHit = hit.collider.gameObject;
        }
        else
        {
            if (previousHit)
            {
                previousHit.GetComponent<SpriteRenderer>().material = normalMaterial;
                previousHit = null;
            }
        }
    }
}
