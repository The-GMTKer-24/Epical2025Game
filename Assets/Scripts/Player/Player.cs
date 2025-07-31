using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerControls playerControls;
    
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

    // Update is called once per frame
    void Update()
    {
        Vector2 scaledInput = playerControls.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
        transform.transform.position += new Vector3(scaledInput.x, scaledInput.y, 0);
    }
}
