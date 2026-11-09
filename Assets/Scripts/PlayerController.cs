using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputReader))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform Camera;
    [SerializeField] private float Sensitivity = 1.0f;
    [SerializeField] private float MaxVerticalAngle = 85f;

    private float xRotation = 0f;

    private CharacterController _cc;
    private InputReader _inputReader;

    #region Unity Lifecycle Hooks
    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _inputReader = GetComponent<InputReader>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Look();
    }
    #endregion

    private void Look()
    {
        Vector2 lookInput = _inputReader.LookInput * Sensitivity;
        
        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -MaxVerticalAngle, MaxVerticalAngle);

        Camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x);
    }
}
