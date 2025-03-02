using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's transform
    public Vector3 offset = new Vector3(0, 0, -10); // Offset of the camera relative to the player
    public float smoothSpeed = 5f; // Speed at which the camera follows the player

    private bool isFollowing = false; // Whether the camera should follow the player

    private void Start()
    {
        playerTransform = FindFirstObjectByType<Player>(FindObjectsInactive.Include).gameObject.transform;
    }

    private void OnEnable()
    {
        GameStateManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void LateUpdate()
    {
        if (isFollowing && playerTransform != null)
        {
            Vector3 desiredPosition = playerTransform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.InGame)
        {
            EnableCameraFollow();
        }
        else
        {
            DisableCameraFollow();
        }
    }

    private void EnableCameraFollow()
    {
        isFollowing = true;
    }

    private void DisableCameraFollow()
    {
        isFollowing = false;
    }
}
