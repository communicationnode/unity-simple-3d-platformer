using System;
using UnityEngine;
using MovementStatesEnumerator = PlatformerCharacterController.MovementStatesManager.MovementStatesEnumerator;

/// <summary>
///  ласс, описывающий ограниченное поведение <see cref="PlatformerCharacterController"/>
/// </summary>
public class StandartMovementState : AbstractMovementState {
    //values
    public Vector2 speedStrength; //you know what is that
    public byte jumpCountMax;//reset jumpCount by this
    public byte jumpCount; //you know what is that
    public float jumpStrength; //jump strenght
    public float gravity; //falling speed
    public bool isOnGround; //a player landing on the ground?
    public bool isOnGroundForJump = true; //can a player jump after landing on the ground
    public bool grabLocker = false; //usually changes by GrabStepMovementState;

    //shared values
    private ushort fallDistance = 0; //a variable that describes the distance an object falls
    public ushort sharedFallDistance { get => fallDistance; } //a variable that describes the distance an object falls

    //events for other classes
    public Action OnJumped = () => { };
    public Action OnStartFalling = () => { };
    public Action OnGroundTouched = () => { };
    public Action OnCrouchStarted = () => { };
    public Action OnCrouchEnded = () => { };
    public Action OnMoveStarted = () => { };
    public Action OnMoveEnded = () => { };

    //states for events ^
    private bool fallingStateElapsedOnce = false;
    private bool groundTouchedElapsedOnce = false;
    private bool isCrouchElapsedOnce = false;
    private bool isMoveElapsedOnce = false;

    private Vector3 lastDirection;
    private bool gravityCorrectorLocker = false;
    //constructor
    public StandartMovementState() {
        onStateWasChanged += () => {
            sharedProperties.sharedRigidBody.useGravity = true;
            sharedProperties.sharedRigidBody.linearDamping = 0.5f;
        };
    }

    //methods
    public override void FixedUpdate() {
        //movement base
        Vector3 newDirection = sharedProperties.sharedDirection * speedStrength;

        //caching direction
        if (sharedProperties.sharedDirection.x != 0) {
            lastDirection = sharedProperties.sharedDirection;
        }

        sharedProperties.sharedRigidBody.AddForce(new Vector3(newDirection.x, 0, newDirection.z), ForceMode.Force);
        sharedProperties.sharedRigidBody.linearVelocity = new Vector3(newDirection.x, gravity, newDirection.z);

        //jumpcount reset
        isOnGround = CheckGround();
        if (isOnGroundForJump == true && isOnGround) {
            jumpCount = jumpCountMax;
        }

        //normalized
        if (sharedProperties.sharedRigidBody.linearVelocity.magnitude > speedStrength.magnitude) {
            Vector3 normalizedVelocity = sharedProperties.sharedRigidBody.linearVelocity.normalized * speedStrength;
            sharedProperties.sharedRigidBody.linearVelocity = new Vector3(normalizedVelocity.x, sharedProperties.sharedRigidBody.linearVelocity.y, normalizedVelocity.z);
        }

        //move action elapsing
        if (MathF.Abs(sharedProperties.sharedDirection.x) > 0 && !isMoveElapsedOnce) {
            isMoveElapsedOnce = true;
            OnMoveStarted();
        }
        if (MathF.Abs(sharedProperties.sharedDirection.x) <= 0 && isMoveElapsedOnce) {
            isMoveElapsedOnce = false;
            OnMoveEnded();
        }

        //gravity
        if (isOnGround) {
            if (!gravityCorrectorLocker) { gravity = gravity / 2f; } } 
        else {
            gravity = gravity >= PlatformerCharacterController.Properties.MIN_GRAVITY ? gravity - 0.95f : PlatformerCharacterController.Properties.MIN_GRAVITY;
        }

        //check when falling is started
        if (gravity <= -7.5f && !isOnGround) {
            fallDistance += 1;

            //elapsing event once when you falling
            if (!fallingStateElapsedOnce) {
                fallingStateElapsedOnce = true;
                groundTouchedElapsedOnce = false;
                OnStartFalling();
            }
        }

        //method for grab movement state
        CheckGroundToGrab();
    }
    public override void Jump() {
        JumpAsync();
    }
    public override void Crouch(in bool enabled) {
        CrouchAsync(enabled);
    }
    private async void CrouchAsync(bool enabled) {
        switch (enabled) {

            case true: {//player crouching
                    BoxCollider collider = sharedProperties.sharedCollider as BoxCollider;
                    collider.center = new Vector3(
                        collider.center.x,
                        -0.25f,
                        collider.center.z);
                    collider.size = new Vector3(
                        collider.size.x,
                        0.5f,
                        collider.size.z);
                    if (!isCrouchElapsedOnce) {
                        isCrouchElapsedOnce = true;
                        OnCrouchStarted();
                    }
                }
                break;

            case false: {//player not crouching
                    await Awaitable.MainThreadAsync();

                    bool repeatCheckRoof = true;
                    while (repeatCheckRoof) {

                        //temp values
                        Vector3 rayStartPoint = sharedProperties.sharedRigidBody.transform.position + sharedProperties.sharedRigidBody.transform.up * 1.2f;
                        short roofCount = 0;
                        await Awaitable.FixedUpdateAsync();

                        //search roof under head
                        Collider[] colliders = Physics.OverlapSphere(rayStartPoint, 0.14f);

                        //handle roof colliders
                        foreach (Collider item in colliders) {
                            if (item == sharedProperties.sharedCollider) {
                                continue;
                            }
                            if (item != sharedProperties.sharedCollider) {
                                roofCount++;
                            }
                        }
                        //if roof is not detected - break <while> loop
                        if (roofCount == 0) {
                            repeatCheckRoof = false;
                            if (isCrouchElapsedOnce) {
                                isCrouchElapsedOnce = false;
                                OnCrouchEnded();
                            }
                        }
                    }

                    //reset collider size
                    BoxCollider collider = sharedProperties.sharedCollider as BoxCollider;
                    collider.center = new Vector3(
                        collider.center.x,
                        0,
                        collider.center.z);
                    collider.size = new Vector3(
                        collider.size.x,
                        1,
                        collider.size.z);
                }
                break;
        }
    }
    private async void JumpAsync() {
        try {
            if (jumpCount > 0) {
                gravityCorrectorLocker = true;
                fallDistance = 0;
                sharedProperties.sharedRigidBody.transform.Translate(0, 0.07f, 0);
                isOnGroundForJump = false;
                fallingStateElapsedOnce = false;
                jumpCount--;
                gravity = jumpStrength;
                OnJumped();
                sharedProperties.sharedRigidBody.linearVelocity = new Vector3(sharedProperties.sharedRigidBody.linearVelocity.x, gravity, sharedProperties.sharedRigidBody.linearVelocity.z);
                await Awaitable.WaitForSecondsAsync(0.3f);
                isOnGroundForJump = true;
                await Awaitable.WaitForSecondsAsync(0.2f);
                gravityCorrectorLocker = false;
            }
        } catch (Exception e) {
            Debug.LogException(e);
        }
    }
    private bool CheckGround() {
        //ground check
        Collider[] overlappedObjects = Physics.OverlapSphere(sharedProperties.sharedRigidBody.transform.position, 0.1f);
        /*DebugExtension.DebugWireSphere(sharedProperties.sharedRigidBody.transform.position,Color.white, radius: 0.1f, duration:0.07f);*/

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

                    //reset falling state as false and elapse OnGroundTouch action once
                    fallingStateElapsedOnce = false;
                    if (!groundTouchedElapsedOnce) {
                        groundTouchedElapsedOnce = true;
                        OnGroundTouched();
                        fallDistance = 0;
                    }
                    break;
                }
            }
        }
        return groundFinded;
    }
    private void CheckGroundToGrab() {

        if (grabLocker) return;
        //check steps to grab when falling
        if (fallingStateElapsedOnce is true && lastDirection.x != 0) {
            Vector3 playerPos = sharedProperties.sharedRigidBody.transform.position;

            //local function for checks colliders (true = finded collider | false = no colliders)
            Func<Vector3, Vector3, bool> checkColliders = (offset, halfExtents) => {
                /*DebugExtension.DebugBounds(new Bounds(playerPos + offset, halfExtents), duration: 0.2f);*/

                //check colliders
                Collider[] colliders = Physics.OverlapBox(playerPos + offset, halfExtents);
                foreach (Collider collider in colliders) {
                    //ignore self collider
                    if (collider == sharedProperties.sharedCollider) {
                        continue;
                    }
                    return true;
                }
                return false;
            };

            //execute check grab and locker colliders
            bool findedHeadRoofLocker = checkColliders(new Vector3(0, 2, 0), new Vector3(0.1f, 1.2f, 0.2f));
            bool findedGrabLocker = checkColliders(new Vector3(lastDirection.x * 0.2f, 2.95f, 0), new Vector3(0.5f, 0.8f, 0.2f));
            bool findedStepToGrab = checkColliders(new Vector3(lastDirection.x * 0.4f, 2.2f, 0), new Vector3(0.02f, 0.3f, 0.2f));

            //check all bools for execute code below
            if (findedStepToGrab && !findedGrabLocker && !findedHeadRoofLocker) {
                //set grab movement state
                sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.SetCurrentState(MovementStatesEnumerator.GrabStep);
                //correcting position
                if (Physics.Raycast(sharedProperties.sharedRigidBody.transform.position + new Vector3(lastDirection.x * 0.5f, 2.2f, 0), new Vector3(0, -1, 0), out RaycastHit hit, 1)) {
                    Vector3 plrPos = sharedProperties.sharedRigidBody.gameObject.transform.position;
                    sharedProperties.sharedRigidBody.gameObject.transform.position = new Vector3(plrPos.x, hit.point.y - 2, plrPos.z);
                }
                //send correcting position to grab movement state
                GrabStepMovementState grabStepMovementState = sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.GetCurrentState() as GrabStepMovementState;
                grabStepMovementState.grabbedHitPoint = hit.point;
            }
        }
    }
    public override void Dash() {
        if (sharedProperties.sharedDirection.x != 0 && isOnGround) {
            Crouch(true);
            sharedProperties.sharedPlatformerCharacterController.sharedMovementStatesManager.SetCurrentState(MovementStatesEnumerator.DriveUp);
        }
    }
}