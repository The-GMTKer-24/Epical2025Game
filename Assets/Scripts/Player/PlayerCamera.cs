using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float zoomPercentPerSecond;
    [SerializeField] float clickMoveSpeed;
    [SerializeField] float clickZoomSpeed;
    [SerializeField] float playerFollowSpeed;
    [SerializeField] float defaultZoomLevel;
    [SerializeField] float minCamSize;
    [SerializeField] float maxCamSize;
    [SerializeField] float panSpeed;
    
    [SerializeField] float interpolationEarlyEndPoint;
    [SerializeField] float interpolationTargetEndPoint;
    [SerializeField] float doubleClickLockoutTime;
    [SerializeField] bool snapBackAfterPan;

    [SerializeField] Player player;
    
    private Camera viewCamera;
    private PlayerControls playerControls;
    private bool isInterpolating;
    private bool wasPlayerInMotion;
    private bool isPanned;
    private Vector2 targetLocation;
    private Vector2 panOffset;
    private Vector2 unpanedPosition;
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
        unpanedPosition = transform.position;
        viewCamera = GetComponent<Camera>();
        playerControls.Player.QuickFocus.performed += OnQuickFocus;
        playerControls.Player.Pan.performed += OnPan;
        playerControls.Player.Pan.canceled += PanCanceled;
    }




    // Update is called once per frame
    void Update()
    {
        Vector2 movementInput = playerControls.Player.Move.ReadValue<Vector2>();
        Vector2 zoomInput = playerControls.Player.Zoom.ReadValue<Vector2>();
        Vector2 panAmount = playerControls.Player.MouseDelta.ReadValue<Vector2>();
        float distFromTarget = Vector2.Distance(unpanedPosition, targetLocation);
        const float threshold = 0.1f;
        // If we are currently moving towards a target, and we have either arrived or we have traveled far enough that the user can cancel the move with their input stop moving
        if (isInterpolating && ((movementInput.sqrMagnitude <= threshold || zoomInput.sqrMagnitude <= threshold) &&
                                distFromTarget <= interpolationEarlyEndPoint) ||
            distFromTarget <= interpolationTargetEndPoint)
        {
            StartCoroutine(BriefCameraInputLockout());
        }
        
        if (isInterpolating)
        {
            unpanedPosition = ExpDecay(unpanedPosition, targetLocation, clickMoveSpeed, Time.deltaTime);
            viewCamera.orthographicSize = ExpDecay(viewCamera.orthographicSize, defaultZoomLevel, clickMoveSpeed,
                Time.deltaTime);
        }
        else
        {
            float scaledZoom = zoomInput.y * (zoomPercentPerSecond * Time.deltaTime) * -1;
            float newScale = scaledZoom * viewCamera.orthographicSize;
            
            Vector2 screenPosition = playerControls.Player.MousePosition.ReadValue<Vector2>();
            Vector2 oldPosition = viewCamera.ScreenToWorldPoint(screenPosition);
            viewCamera.orthographicSize = Mathf.Clamp(newScale + viewCamera.orthographicSize, minCamSize, maxCamSize);
            
            if (movementInput.sqrMagnitude <= threshold)
            { 
                if (zoomInput.sqrMagnitude >= threshold)
                {
                    Vector2 cameraDelta = oldPosition - (Vector2)viewCamera.ScreenToWorldPoint(screenPosition);
                    unpanedPosition += cameraDelta;
                    wasPlayerInMotion = false;
                }
            }
            if (isPanned)
            {
                Vector2 scaledPan = panAmount * (-1 * viewCamera.orthographicSize * Time.deltaTime * panSpeed);
                panOffset += scaledPan;
            }
            
            if (movementInput.sqrMagnitude >= threshold)
            {
                wasPlayerInMotion = true;
            }
            if (wasPlayerInMotion)
            {
                if ((Vector2.Distance(unpanedPosition, player.transform.position) <= threshold))
                {
                    wasPlayerInMotion = false;
                }
                unpanedPosition = ExpDecay(unpanedPosition, player.transform.position, playerFollowSpeed, Time.deltaTime);
            }
        }

        Vector2 overall = unpanedPosition + panOffset;
        transform.position = new Vector3(overall.x, overall.y, transform.position.z);
    }


    IEnumerator BriefCameraInputLockout()
    {
        yield return new WaitForSeconds(doubleClickLockoutTime);
        isInterpolating = false;
    }
    
    private void OnQuickFocus(InputAction.CallbackContext obj)
    {
        isInterpolating = true;
        wasPlayerInMotion = false;
        playerControls.Player.Pan.Disable();
        playerControls.Player.Pan.Enable();
        targetLocation = viewCamera.ScreenToWorldPoint(playerControls.Player.MousePosition.ReadValue<Vector2>());
    }
    
    private void PanCanceled(InputAction.CallbackContext ctx)
    {
        isPanned = false;
        unpanedPosition += panOffset;
        panOffset = new Vector2(0, 0);
        wasPlayerInMotion = snapBackAfterPan;
    }

    private void OnPan(InputAction.CallbackContext ctx)
    {
        isInterpolating = false;
        isPanned = true;
        wasPlayerInMotion = false;
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
