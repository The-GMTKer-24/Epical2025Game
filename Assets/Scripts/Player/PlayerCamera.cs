using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float zoomPercentPerSecond;
        [SerializeField] private float clickMoveSpeed;
        [SerializeField] private float clickZoomSpeed;
        [SerializeField] private float playerFollowSpeed;
        [SerializeField] private float defaultZoomLevel;
        [SerializeField] private float minCamSize;
        [SerializeField] private float maxCamSize;
        [SerializeField] private float panSpeed;

        [SerializeField] private float interpolationEarlyEndPoint;
        [SerializeField] private float interpolationTargetEndPoint;
        [SerializeField] private float doubleClickLockoutTime;
        [SerializeField] private bool snapBackAfterPan;

        [SerializeField] private Player player;
        private bool isInterpolating;
        private bool isPanned;
        private Vector2 panOffset;
        private PlayerControls playerControls;
        private Vector2 targetLocation;
        private Vector2 unpanedPosition;

        private Camera viewCamera;
        private bool wasPlayerInMotion;

        private void Awake()
        {
            playerControls = new PlayerControls();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            unpanedPosition = transform.position;
            viewCamera = GetComponent<Camera>();
            playerControls.Player.QuickFocus.performed += OnQuickFocus;
            playerControls.Player.Pan.performed += OnPan;
            playerControls.Player.Pan.canceled += PanCanceled;
        }


        // Update is called once per frame
        private void Update()
        {
            var movementInput = playerControls.Player.Move.ReadValue<Vector2>();
            var zoomInput = playerControls.Player.Zoom.ReadValue<Vector2>();
            var panAmount = playerControls.Player.MouseDelta.ReadValue<Vector2>();
            var distFromTarget = Vector2.Distance(unpanedPosition, targetLocation);
            const float threshold = 0.1f;
            // If we are currently moving towards a target, and we have either arrived or we have traveled far enough that the user can cancel the move with their input stop moving
            if ((isInterpolating && (movementInput.sqrMagnitude <= threshold || zoomInput.sqrMagnitude <= threshold) &&
                 distFromTarget <= interpolationEarlyEndPoint) ||
                distFromTarget <= interpolationTargetEndPoint)
                StartCoroutine(BriefCameraInputLockout());

            if (isInterpolating)
            {
                unpanedPosition = ExpDecay(unpanedPosition, targetLocation, clickMoveSpeed, Time.deltaTime);
                viewCamera.orthographicSize = ExpDecay(viewCamera.orthographicSize, defaultZoomLevel, clickMoveSpeed,
                    Time.deltaTime);
            }
            else
            {
                var scaledZoom = zoomInput.y * zoomPercentPerSecond * -1;
                var newScale = scaledZoom * viewCamera.orthographicSize;

                var screenPosition = playerControls.Player.MousePosition.ReadValue<Vector2>();
                Vector2 oldPosition = viewCamera.ScreenToWorldPoint(screenPosition);
                viewCamera.orthographicSize =
                    Mathf.Clamp(newScale + viewCamera.orthographicSize, minCamSize, maxCamSize);

                if (movementInput.sqrMagnitude <= threshold)
                    if (zoomInput.sqrMagnitude >= threshold)
                    {
                        var cameraDelta = oldPosition - (Vector2)viewCamera.ScreenToWorldPoint(screenPosition);
                        unpanedPosition += cameraDelta;
                        wasPlayerInMotion = false;
                    }

                if (isPanned)
                {
                    var scaledPan = panAmount * (-1 * viewCamera.orthographicSize * Time.deltaTime * panSpeed);
                    panOffset += scaledPan;
                }

                if (movementInput.sqrMagnitude >= threshold) wasPlayerInMotion = true;
                if (wasPlayerInMotion)
                {
                    if (Vector2.Distance(unpanedPosition, player.transform.position) <= threshold)
                        wasPlayerInMotion = false;
                    unpanedPosition = ExpDecay(unpanedPosition, player.transform.position, playerFollowSpeed,
                        Time.deltaTime);
                }
            }

            var overall = unpanedPosition + panOffset;
            transform.position = new Vector3(overall.x, overall.y, transform.position.z);
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }


        private IEnumerator BriefCameraInputLockout()
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
}