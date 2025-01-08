using System;
using UnityEngine;


/// <summary>
///  ласс, определ€ющий поведение <see cref="PlatformerCharacterController"/>
/// </summary>
public abstract class AbstractMovementState
{
    /*---| values |---*/
    public Action onStateWasChanged = () => { }; //invoked when this state became is current
    public Action onStateWasUnfocused = () => { }; //invoked when this state is losing current focus
    public PlatformerCharacterController.Properties sharedProperties;

    /*---| methods |---*/
    public virtual void FixedUpdate() {/*it's in abstract class, what are you expected see here, dumbass????*/}
    public virtual void Update() {/*it's in abstract class, what are you expected see here, dumbass????*/}
    public virtual void Jump() {/*it's in abstract class, what are you expected see here, dumbass????*/}
    public virtual void Crouch(in bool enabled) {/*it's in abstract class, what are you expected see here, dumbass????*/}
    public virtual void Dash() {/*it's in abstract class, what are you expected see here, dumbass????*/}

    public virtual void LookUp() {/*it's in abstract class, what are you expected see here, dumbass????*/ }
    public virtual void LookDown() {/*it's in abstract class, what are you expected see here, dumbass????*/ }
    public virtual void LookLeft() {/*it's in abstract class, what are you expected see here, dumbass????*/ }
    public virtual void LookRight() {/*it's in abstract class, what are you expected see here, dumbass????*/ }


    /*---| faggot ideas |---*/
    //stupid shit idk why this is still here
    ~AbstractMovementState() {
        onStateWasChanged = null;
        onStateWasUnfocused = null;
    }
}
