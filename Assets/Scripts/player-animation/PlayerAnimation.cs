using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using MovementStatesEnumerator = PlatformerCharacterController.MovementStatesManager.MovementStatesEnumerator;

public class PlayerAnimation : MonoBehaviour {
    [SerializeField] private PlatformerCharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorMessageHandler animatorMessageHandler;
    [SerializeField] private MagicaCloth2.MagicaCloth magicaCloth;
    private Dictionary<MovementStatesEnumerator, AbstractMovementState> sharedStates;

    private float moveBlendValue = 0;
    private float crouchBlendValue = 0;

    private void Start() {

        //get shared states dictionary
        sharedStates = characterController.sharedMovementStatesManager.GetStates();

        //initialize state values
        StandartMovementState standartMovementState = sharedStates.GetValueOrDefault(MovementStatesEnumerator.Standart) as StandartMovementState;
        FlyMovementState flyMovementState = sharedStates.GetValueOrDefault(MovementStatesEnumerator.Fly) as FlyMovementState;
        GrabStepMovementState grabStepMovementState = sharedStates.GetValueOrDefault(MovementStatesEnumerator.GrabStep) as GrabStepMovementState;
        DriveUpMovementState driveUpMovementState = sharedStates.GetValueOrDefault(MovementStatesEnumerator.DriveUp) as DriveUpMovementState;
        //adding state actions
        flyMovementState.onStateWasChanged += () => { animator.PlayInFixedTime("jump", 0, 0); };
        //---
        standartMovementState.onStateWasChanged += () => { animator.CrossFadeInFixedTime("move", 0.1f, 0); };
        standartMovementState.OnMoveStarted += () => { moveBlendValue = 1; };
        standartMovementState.OnMoveEnded += () => { moveBlendValue = 0; };
        standartMovementState.OnJumped += () => { animator.CrossFadeInFixedTime("jump", 0.1f, 0); };
        standartMovementState.OnStartFalling += () => { animator.CrossFade("falling", 0.1f); };
        standartMovementState.OnGroundTouched += () => {
            if (standartMovementState.sharedFallDistance > 20) {
                animator.CrossFadeInFixedTime("landed_on_ground", 0.1f, 0);
            } else {
                animator.CrossFadeInFixedTime("move", 0.1f, 0);
            }
        };
        standartMovementState.OnCrouchStarted += () => { crouchBlendValue = 1; };
        standartMovementState.OnCrouchEnded += () => { crouchBlendValue = 0; };
        //---
        grabStepMovementState.onStateWasChanged += () => { animator.CrossFadeInFixedTime("grab_step", 0.08f, 0); };
        grabStepMovementState.onLookedUp += () => { animator.PlayInFixedTime("grab_step_up", 0, 0); };
        //---
        driveUpMovementState.onStateWasChanged += () => { animator.CrossFadeInFixedTime("drive_up", 0.1f, 0); };


        animatorMessageHandler.OnMessageGetted += (message) => {
            switch (message) {
                case "grab_step_up_ended":
                    
                    grabStepMovementState.ResetToStandartMovementStateMode();
                    animator.PlayInFixedTime("move", 0, 0);
                    
                    break;
                default:
                    break;
            }
        };
    }
    private void FixedUpdate() {
        animator.SetFloat("walking", Mathf.Lerp(animator.GetFloat("walking"), moveBlendValue, 0.5f));
        animator.SetFloat("crouch", Mathf.Lerp(animator.GetFloat("crouch"), crouchBlendValue, 0.5f));
    }
}
