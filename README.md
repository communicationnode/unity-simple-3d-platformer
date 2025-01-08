# Simple 3D platformer
```Created in Unity version: 6000.0.24f1;```<br>

![prewiew](_git_readme/gif_title.gif) 

-----------------------

# Controls:
```
A,D - left\right moving;
SPACEBAR - jump;
CTRL, C - crouch;
LEFT SHIFT - dash;
```
-----------------------
# External assets:
```
BOXOPHOBIC                   -> simple fog; 
AstarPathfindingProject      -> not used in the demo project
DebugDrawingExtension        -> for best debugging;
MagicaCloth2                 -> for cloth simulations;
RealToon                     -> anime shaders;
Ropofoo - Foot Controller IK -> for foot inverse kinematics;
```
-----------------------
# Player movement main scripts:
```
|main core|
  1) PlatformerCharacterController.cs  -> The main core of player movement. This is the movement state machine. It contains the basic movement states, and methods for controlling and switching between them.
  2) AbstractMovementState.cs          -> Abstract class of the movement state. Describes the main methods of controlling the player

|additional scripts|
  1) DriveUpMovementState.cs     -> Dash movement state;
  2) StandartMovementState.cs    -> Moving, Jumping, Crouching movement state;
  3) FlyMovementState.cs         -> Debug movement state that fly;
  4) GrabStepMovementState.cs    -> Grab the edges of the surface. The player can jump or climb;
```
