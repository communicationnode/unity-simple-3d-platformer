using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Класс, описывающий ручное перемещение объекта, имеющего <see cref="Rigidbody"/>, через <see cref="InputAction"/> систему.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlatformerCharacterController : MonoBehaviour {

    /*---|  values |---*/
    #region alterable values
    [SerializeField] private InputActionReference movementIputActionReference;
    [SerializeField] private InputActionReference jumpInputActionReference;
    [SerializeField] private InputActionReference crouchInputActionReference;
    [SerializeField] private InputActionReference driveUpInputActionReference;

    [SerializeField] private Properties properties;
    private MovementStatesManager movementStatesManager = new MovementStatesManager();

    public InputActionReference sharedMovementInpuAction { get => movementIputActionReference; }
    public InputActionReference sharedJumpInpuAction { get => jumpInputActionReference; }
    public InputActionReference sharedCrouchInpuAction { get => crouchInputActionReference; }
    public InputActionReference sharedDriveUpInpuAction { get => driveUpInputActionReference; }
    public MovementStatesManager sharedMovementStatesManager { get => movementStatesManager; }

    [System.Serializable]
    public class MovementStatesManager {
        /*---| values |---*/
        public enum MovementStatesEnumerator : byte {Fly,Standart, GrabStep, DriveUp}     
        private Dictionary<MovementStatesEnumerator, AbstractMovementState> movementStates = new Dictionary<MovementStatesEnumerator, AbstractMovementState>();
        private AbstractMovementState currentMovementState;

        /*---| methods |---*/
        public void WriteStateOnDictionary(MovementStatesEnumerator stateType, AbstractMovementState state) => movementStates.Add(stateType, state);      
        public void SetCurrentState(in MovementStatesEnumerator typeState) {
            if (GetCurrentState() != null) {
                GetCurrentState().onStateWasUnfocused();
            }
            movementStates.TryGetValue(typeState, out currentMovementState);
            if (GetCurrentState().onStateWasChanged != null) {
                GetCurrentState().onStateWasChanged();
            }
        }
        public AbstractMovementState GetCurrentState() => currentMovementState;
        public Dictionary<MovementStatesEnumerator, AbstractMovementState> GetStates() => movementStates;
    }

    [System.Serializable]
    public class Properties {
        /*---| consts |---*/
        public const float MIN_GRAVITY = -11f;

        /*---| values |---*/
        [SerializeField] private readonly PlatformerCharacterController platformerCharacterController;
        [SerializeField] private Vector2 direction;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private Collider collider;
        public Vector2 sharedDirection { get => direction; }
        public Rigidbody sharedRigidBody { get => rigidBody; }
        public Collider sharedCollider { get => collider; }
        public PlatformerCharacterController sharedPlatformerCharacterController { get => platformerCharacterController; }

        /*---| methods |---*/
        public void SetDirection(in Vector2 direction) => this.direction = direction;
        public void AppendRigidBody(in Rigidbody rigidbody) => this.rigidBody = rigidbody;
        public Properties(in PlatformerCharacterController platformerCharacterController) {
            this.platformerCharacterController = platformerCharacterController;
            rigidBody = platformerCharacterController.GetComponent<Rigidbody>();
            collider = platformerCharacterController.GetComponentInChildren<Collider>();
        }
    }
    #endregion



    /*---| methods |---*/
    #region methods

    // Unity methods
    private void Start() {
        //initialize properties
        properties = new Properties(this);

        //initialize movement states and writing it in states dictionary
        movementStatesManager.WriteStateOnDictionary(MovementStatesManager.MovementStatesEnumerator.Fly, new FlyMovementState() {
            speedStrength = new Vector2(5, 5),
            sharedProperties = properties,
        });
        movementStatesManager.WriteStateOnDictionary(MovementStatesManager.MovementStatesEnumerator.Standart, new StandartMovementState() {
            speedStrength = new Vector2(7, 7),
            sharedProperties = properties,
            jumpStrength = 15f,
            jumpCountMax = 2
        });
        movementStatesManager.WriteStateOnDictionary(MovementStatesManager.MovementStatesEnumerator.GrabStep, new GrabStepMovementState() {
            sharedProperties = properties,
        });
        movementStatesManager.WriteStateOnDictionary(MovementStatesManager.MovementStatesEnumerator.DriveUp, new DriveUpMovementState() {
            sharedProperties = properties,
        });
        movementStatesManager.SetCurrentState(MovementStatesManager.MovementStatesEnumerator.Standart);

        //standart input actions
        movementIputActionReference.action.started += InvokeMovementActions;
        movementIputActionReference.action.performed += GetMovementDirection;
        movementIputActionReference.action.canceled += ResetMovementDirection;
        jumpInputActionReference.action.started += Jump;
        crouchInputActionReference.action.started += CrouchEnable;
        crouchInputActionReference.action.canceled += CrouchDisable;
        driveUpInputActionReference.action.started += DriveUp;
    }
    private void OnDestroy() {
        movementIputActionReference.action.started -= InvokeMovementActions;
        movementIputActionReference.action.performed -= GetMovementDirection;
        movementIputActionReference.action.canceled -= ResetMovementDirection;
        jumpInputActionReference.action.started -= Jump;
        crouchInputActionReference.action.started -= CrouchEnable;
        crouchInputActionReference.action.canceled -= CrouchDisable;
        driveUpInputActionReference.action.started -= DriveUp;
    }
    private void FixedUpdate() {
        if (movementStatesManager.GetCurrentState() != null) {
            movementStatesManager.GetCurrentState().FixedUpdate();
        }
    }
    private void Update() {
        if (movementStatesManager.GetCurrentState() != null) {
            movementStatesManager.GetCurrentState().Update();
        }
    }

    // Jump
    private void Jump(InputAction.CallbackContext callback) {
        movementStatesManager.GetCurrentState().Jump();
    }

    // Move
    private void GetMovementDirection(InputAction.CallbackContext callback) => properties.SetDirection(callback.ReadValue<Vector2>());
    private void ResetMovementDirection(InputAction.CallbackContext callback) => properties.SetDirection(default);
    private void InvokeMovementActions(InputAction.CallbackContext callback) {
        if (callback.ReadValue<Vector2>().x > 0) { movementStatesManager.GetCurrentState().LookRight(); }
        if (callback.ReadValue<Vector2>().x < 0) { movementStatesManager.GetCurrentState().LookLeft(); }
        if (callback.ReadValue<Vector2>().y > 0) { movementStatesManager.GetCurrentState().LookUp(); }
        if (callback.ReadValue<Vector2>().y < 0) { movementStatesManager.GetCurrentState().LookDown(); }
    }

    // Crouch
    private void CrouchEnable(InputAction.CallbackContext callback) => Crouch(true);
    private void CrouchDisable(InputAction.CallbackContext callback) => Crouch(false);
    private void Crouch(in bool enabled) {
        movementStatesManager.GetCurrentState().Crouch(enabled);
    }
    private void DriveUp(InputAction.CallbackContext callback) {
        movementStatesManager.GetCurrentState().Dash();
    }

    // ___UNITY EDITOR SHIT___
    public void SetFlyMode() {
        movementStatesManager.SetCurrentState(MovementStatesManager.MovementStatesEnumerator.Fly);
    }
    public void SetStandartMode() {
        movementStatesManager.SetCurrentState(MovementStatesManager.MovementStatesEnumerator.Standart);
    }
    public void SetGrabStepMode() {
        movementStatesManager.SetCurrentState(MovementStatesManager.MovementStatesEnumerator.GrabStep);
    }
    public void SetDriveUpMode() {
        movementStatesManager.SetCurrentState(MovementStatesManager.MovementStatesEnumerator.DriveUp);
    }
    #endregion
}