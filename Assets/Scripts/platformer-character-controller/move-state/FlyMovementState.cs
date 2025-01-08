using UnityEngine;

public class FlyMovementState : AbstractMovementState {
    //values
    public Vector2 speedStrength;

    //constructor
    public FlyMovementState() {
        onStateWasChanged += ()=> {
            sharedProperties.sharedRigidBody.useGravity = false;
            sharedProperties.sharedRigidBody.linearDamping = 6;
        };
    }

    //methods
    public override void FixedUpdate() {
        sharedProperties.sharedRigidBody.AddForce(sharedProperties.sharedDirection * speedStrength, ForceMode.Force);     
    }
}
