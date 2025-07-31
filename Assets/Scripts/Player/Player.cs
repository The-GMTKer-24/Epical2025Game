using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed;
        private PlayerControls playerControls;

        private void Awake()
        {
            playerControls = new PlayerControls();
        }

        // Update is called once per frame
        private void Update()
        {
            var scaledInput = playerControls.Player.Move.ReadValue<Vector2>() * (Time.deltaTime * speed);
            // Animation shenanigans HERE
            transform.transform.position += new Vector3(scaledInput.x, scaledInput.y, 0);
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
}