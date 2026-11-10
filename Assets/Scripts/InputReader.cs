using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, InputSystem.IPlayerActions
{
    public Vector2 LookInput { get; private set; }
    public Vector2 MoveInput { get; private set; }

    private InputSystem _inputSystem;

    #region Lifecycle Hooks
    private void Awake()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        _inputSystem.Player.Enable();
    }

    private void OnDisable()
    {
        _inputSystem.Player.Disable();
    }

    private void OnDestroy()
    {
        _inputSystem.Dispose();
    }
    #endregion

    #region Input Callbacks

    public void OnAttack(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
