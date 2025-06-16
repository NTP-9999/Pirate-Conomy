using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity = 100f;
    
    [Header("Camera Constraints")]
    public float maxLookAngle = 80f;
    public float minLookAngle = -80f;
    
    [Header("Optional Player Body Reference")]
    public Transform playerBody;
    
    private float xRotation = 0f;
    private float yRotation = 0f;
    
    void Start()
    {
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        
        // If no player body is assigned, try to find parent transform
        if (playerBody == null && transform.parent != null)
        {
            playerBody = transform.parent;
        }
    }
    
    void Update()
    {
        HandleMouseLook();
        HandleInput();
    }
    
    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        // Rotate around Y axis (horizontal look)
        yRotation += mouseX;
        
        // Rotate around X axis (vertical look) with constraints
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);
        
        // Apply rotation to camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Apply horizontal rotation to player body if available
        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else
        {
            // If no player body, rotate the camera itself
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
    
    void HandleInput()
    {
        // Toggle cursor lock with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    
    /// <summary>
    /// Set the camera's rotation programmatically
    /// </summary>
    public void SetCameraRotation(float x, float y)
    {
        xRotation = Mathf.Clamp(x, minLookAngle, maxLookAngle);
        yRotation = y;
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
    
    /// <summary>
    /// Get current camera rotation
    /// </summary>
    public Vector2 GetCameraRotation()
    {
        return new Vector2(xRotation, yRotation);
    }
}