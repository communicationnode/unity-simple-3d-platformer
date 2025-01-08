using System;
using System.Threading.Tasks;
using UnityEngine;
using MovementStatesEnumerator = PlatformerCharacterController.MovementStatesManager.MovementStatesEnumerator;
public class GrabStepMovementState : AbstractMovementState
{
    //values
    public Action onLookedUp = () => { };
    public Vector3 grabbedHitPoint = default; //this value changing by StandartMovementState usually
    private bool isLookedUpLocker = false;

    //constructor
    public GrabStepMovementState() {
        onStateWasChanged += () => { 
            sharedProperties.sharedRigidBody.useGravity = false;
            sharedProperties.sharedRigidBody.linearDamping = 999;
            sharedProperties.sharedRigidBody.linearVelocity = new Vector3(0,0,0);
            isLookedUpLocker = false;
            grabbedHitPoint = default;
        };
    }

    //methods
    public override void Jump() {
        if (!isLookedUpLocker) {
            isLookedUpLocker = true;
            onLookedUp();
        }

        //will used in the future

        /*sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.SetCurrentState(MovementStatesEnumerator.Standart);
        StandartMovementState standartMovementState = sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.GetCurrentState() as StandartMovementState;
        standartMovementState.jumpCount = standartMovementState.jumpCount <= 0 ? (byte)1 : standartMovementState.jumpCount;
        standartMovementState.sharedProperties.sharedRigidBody.linearVelocity = new Vector3(0, 0, 0);
        standartMovementState.Jump();*/
    }
    public override void Crouch(in bool enabled) {
        sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.SetCurrentState(MovementStatesEnumerator.Standart);
        StandartMovementState standartMovementState = sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.GetCurrentState() as StandartMovementState;
        standartMovementState.gravity = 0;
        standartMovementState.sharedProperties.sharedRigidBody.linearVelocity = new Vector3(0, 0, 0);
        standartMovementState.OnStartFalling();
        standartMovementState.grabLocker = true;
        Task.Run(async () => {
            await Awaitable.MainThreadAsync();
            await Awaitable.WaitForSecondsAsync(0.17f);
            standartMovementState.grabLocker = false;
        });
    }
    public override void FixedUpdate() {
        if (sharedProperties.sharedDirection.y > 0 && !isLookedUpLocker) {
            isLookedUpLocker = true;
            onLookedUp();
        }
    }
    public void ResetToStandartMovementStateMode() {
        sharedProperties.sharedRigidBody.gameObject.transform.position = grabbedHitPoint;
        sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.SetCurrentState(MovementStatesEnumerator.Standart);
    }
}
