using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float zoomSpeed;
    [SerializeField] float clickMoveSpeed;
    [SerializeField] private float defaultZoomLevel;
    
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
        Vector2 playerInput = playerControls.Player.Move.ReadValue<Vector2>();
        Vector2 scaledInput = playerInput * (moveSpeed * Time.deltaTime);
        transform.transform.position += new Vector3(scaledInput.x, scaledInput.y, 0);

        Vector2 zoomInput = playerControls.Player.Zoom.ReadValue<Vector2>();
        float scaledZoom = zoomInput.y * (zoomSpeed * Time.deltaTime) * -1;
        viewCamera.orthographicSize += scaledZoom;
    }
    private void OnQuickFocus(InputAction.CallbackContext obj)
    {
        isInterpolating = true;
        targetLocation = viewCamera.ScreenToWorldPoint(playerControls.Player.MousePosition.ReadValue<Vector2>());
    }
}
