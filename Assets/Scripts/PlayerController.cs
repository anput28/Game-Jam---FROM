using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float SprintDuration = 5f;
    [SerializeField] private float RechargeTimeAfterSprint = 5f;
    [SerializeField] private float Gravity = -15f; // The real gravity is 9.81 m/s², but we can exaggerate it for gameplay purposes

    [Header("UI")]
    [SerializeField] private Image SprintImage;

    private float _lookRotation = 0f;
    private Vector3 _velocity = Vector3.zero;

    private float _sprintTimeElapsed = 0f;
    private float _sprintRechargeTimeElapsed = 0f;
    private bool _isExhausted = false;

    private CharacterController _cc;
    private InputReader _inputReader;

    #region Unity Lifecycle Hooks
    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _inputReader = GetComponent<InputReader>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (SprintImage != null)
        {
            SprintImage.fillAmount = 0f;
        }
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

        float speed = CalculateSpeed();

        move = ApplyGravity(move);
        _cc.Move(speed * Time.deltaTime * move);
    }

    private float CalculateSpeed()
    {
        UpdateStamina();
        return CanSprint() ? SprintSpeed : Speed;
    }

    private bool CanSprint()
    {
        return _inputReader.IsSprinting && !_isExhausted;
    }

    private void UpdateStamina()
    {
        float dt = Time.deltaTime;

        if (CanSprint())
        {
            _sprintTimeElapsed += dt;

            // Handle exhaustion
            if (_sprintTimeElapsed >= SprintDuration)
            {
                _sprintTimeElapsed = SprintDuration;
                _isExhausted = true;
            }

            // Decrease the recharge time elapsed while sprinting
            _sprintRechargeTimeElapsed = Mathf.Max(0f, _sprintRechargeTimeElapsed - dt);
        }
        else if (_sprintTimeElapsed > 0)
        {
            _sprintRechargeTimeElapsed += dt;
            if (_sprintRechargeTimeElapsed >= RechargeTimeAfterSprint)
            {
                _sprintRechargeTimeElapsed = RechargeTimeAfterSprint;
                _isExhausted = false;
            }

            // Decrease the sprint time elapsed while recharging
            _sprintTimeElapsed = Mathf.Max(0f, _sprintTimeElapsed - dt);
        }

        UpdateSprintUI();
    }

    private void UpdateSprintUI()
    {
        if (SprintImage != null)
        {
            float fillAmount = _sprintTimeElapsed / SprintDuration;
            SprintImage.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }

    private Vector3 ApplyGravity(Vector3 motion)
    {
        if (_cc.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Small negative value to keep the player grounded
        }
        _velocity.y += Gravity * Time.deltaTime;
        motion += new Vector3(0, _velocity.y, 0);
        return motion;
    }

}
