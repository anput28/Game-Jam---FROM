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
    [SerializeField] private float Gravity = -15f; // The real gravity is 9.81 m/s², but we can exaggerate it for gameplay purposes

    private float _lookRotation = 0f;
    private Vector3 _velocity = Vector3.zero;

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
        _lookRotation -= lookInput.y;
        _lookRotation = Mathf.Clamp(_lookRotation, -MaxVerticalAngle, MaxVerticalAngle);
        Camera.localRotation = Quaternion.Euler(_lookRotation, 0f, 0f);

        // Update the horizontal rotation (y-axis)
        transform.Rotate(Vector3.up * lookInput.x);
    }

    private void Move()
    {
        Vector2 moveInput = _inputReader.MoveInput;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Handle sprinting
        bool isSprinting = _inputReader.SprintInput;
        float speed = isSprinting ? SprintSpeed : Speed;

        // Apply gravity
        if (_cc.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Small negative value to keep the player grounded
        }
        _velocity.y += Gravity * Time.deltaTime;

        move += new Vector3(0, _velocity.y, 0);
        _cc.Move(speed * Time.deltaTime * move);
    }
}
