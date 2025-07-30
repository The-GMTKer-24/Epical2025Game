using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float zoomPercentPerSecond;
    [SerializeField] float clickMoveSpeed;
    [SerializeField] float clickZoomSpeed;
    [SerializeField] private float defaultZoomLevel;

    [SerializeField] private float interpolationEarlyEndPoint;
    [SerializeField] private float interpolationTargetEndPoint;
    
    
    private Camera viewCamera;
    private PlayerControls playerControls;
    private bool isInterpolating;
    private Vector2 targetLocation;
    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        viewCamera = GetComponent<Camera>();
        playerControls.Player.QuickFocus.performed += OnQuickFocus;
    }



    // Update is called once per frame
    void Update()
    {
        Vector2 movementInput = playerControls.Player.Move.ReadValue<Vector2>();
        Vector2 zoomInput = playerControls.Player.Zoom.ReadValue<Vector2>();
        float distFromTarget = Vector2.Distance(transform.position, targetLocation);
        const float threshold = 0.0001f;
        // If we are currently moving towards a target, and we have either arrived or we have traveled far enough that the user can cancel the move with their input stop moving
        if (isInterpolating && ((movementInput.sqrMagnitude <= threshold || zoomInput.sqrMagnitude <= threshold) &&
                                distFromTarget <= interpolationEarlyEndPoint) ||
            distFromTarget <= interpolationTargetEndPoint)
        {
            isInterpolating = false;
        }
        
        if (isInterpolating)
        {
            Vector2 newLocation = ExpDecay(transform.position, targetLocation, clickMoveSpeed, Time.deltaTime);
            transform.position = new Vector3(newLocation.x, newLocation.y, transform.position.z);
            viewCamera.orthographicSize = ExpDecay(viewCamera.orthographicSize, defaultZoomLevel, clickMoveSpeed,
                Time.deltaTime);
        }
        else
        {
            Vector2 scaledInput = movementInput * (moveSpeed * Time.deltaTime);
            transform.transform.position += new Vector3(scaledInput.x, scaledInput.y, 0);

            float scaledZoom = zoomInput.y * (zoomPercentPerSecond * Time.deltaTime) * -1;
            float newScale = scaledZoom * viewCamera.orthographicSize;
            float oldScale = viewCamera.orthographicSize;
            
            Vector2 screenPosition = playerControls.Player.MousePosition.ReadValue<Vector2>();
            Vector2 oldPosition = viewCamera.ScreenToWorldPoint(screenPosition);
            viewCamera.orthographicSize += newScale;
            if (movementInput.sqrMagnitude <= threshold && zoomInput.sqrMagnitude >= threshold)
            {
                Vector2 cameraDelta = oldPosition - (Vector2)viewCamera.ScreenToWorldPoint(screenPosition);
                transform.position += new Vector3(cameraDelta.x, cameraDelta.y, 0);
            }

        }

    }
    private void OnQuickFocus(InputAction.CallbackContext obj)
    {
        isInterpolating = true;
        targetLocation = viewCamera.ScreenToWorldPoint(playerControls.Player.MousePosition.ReadValue<Vector2>());
    }

    private Vector2 ExpDecay(Vector2 a, Vector2 b, float decay, float dt)
    {
        return b + (a - b) * Mathf.Exp(-decay * dt);
    }
    private float ExpDecay(float a, float b, float decay, float dt)
    {
        return b + (a - b) * Mathf.Exp(-decay * dt);
    }
}
