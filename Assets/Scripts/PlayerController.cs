using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputReader))]
public class PlayerController : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] private Transform Camera;
    [SerializeField] private float Sensitivity = 0.1f;
    [SerializeField] private float MaxVerticalAngle = 85f;

    [Header("Movement")]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float SprintSpeed = 15f;

    private float xRotation = 0f;

    private CharacterController _cc;
    private InputReader _inputReader;

    #region Unity Lifecycle Hooks
    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _inputReader = GetComponent<InputReader>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Look();
        Move();
    }
    #endregion

    private void Look()
    {
        Vector2 lookInput = _inputReader.LookInput * Sensitivity;

        // Update the vertical rotation (x-axis)
        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -MaxVerticalAngle, MaxVerticalAngle);
        Camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Update the horizontal rotation (y-axis)
        transform.Rotate(Vector3.up * lookInput.x);
    }

    private void Move()
    {
        Vector2 moveInput = _inputReader.MoveInput;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        bool isSprinting = _inputReader.SprintInput;
        float speed = isSprinting ? SprintSpeed : Speed;

        _cc.Move(speed * Time.deltaTime * move);
    }
}
