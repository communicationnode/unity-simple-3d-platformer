using System.Threading.Tasks;
using UnityEngine;

public class DriveUpMovementState : AbstractMovementState {
    //values
    private bool isOnGround; //a player landing on the ground?
    private bool stopDriveUp = false;
    //constructor
    public DriveUpMovementState() {
        onStateWasChanged += () => {
            stopDriveUp = false;
            isOnGround = false;
            sharedProperties.sharedRigidBody.AddForce(new Vector3(sharedProperties.sharedDirection.x/2, 0, 0), ForceMode.Impulse);
            Task.Run(async() => {
                await Awaitable.MainThreadAsync();
                await Awaitable.WaitForSecondsAsync(1f);
                stopDriveUp = true;
            });           
        };
    }

    public override void FixedUpdate() {
        //is checking ground available
        if (stopDriveUp) {
            isOnGround = CheckGround();
        }

        //check ground
        if (isOnGround) {
            sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.SetCurrentState(PlatformerCharacterController.MovementStatesManager.MovementStatesEnumerator.Standart);
            StandartMovementState standartMovementState = sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.GetCurrentState() as StandartMovementState;
            standartMovementState.Crouch(false);
        }

        //correcting velocity
        Vector3 velocity = sharedProperties.sharedRigidBody.linearVelocity;
        if (velocity.y < -11f) {
            velocity = new Vector3(velocity.x,PlatformerCharacterController.Properties.MIN_GRAVITY,velocity.z);
        }
        sharedProperties.sharedRigidBody.linearVelocity = velocity;

    }
    private bool CheckGround() {
        //ground check
        Collider[] overlappedObjects = Physics.OverlapSphere(sharedProperties.sharedRigidBody.transform.position, 0.1f);

        //gravity
        bool groundFinded = false;
        if (overlappedObjects != null && overlappedObjects.Length > 0) {
            foreach (Collider collider in overlappedObjects) {
                //if you checked self collider = loop continue
                if (collider == sharedProperties.sharedCollider) {
                    continue;
                }
                //if another collider = you finded the ground
                else if (collider != sharedProperties.sharedCollider) {
                    groundFinded = true;
                    break;
                }
            }
        }
        return groundFinded;
    }
}
