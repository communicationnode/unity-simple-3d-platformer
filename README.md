<h1 align="center"> Simple 3D platformer </h1>
  
```Created in Unity version: 6000.0.24f1;```<br>

![prewiew](_git_readme/gif_title.gif) 

-----------------------

<h1 align="center"> Controls: </h1>

```
A,D - left\right moving;
SPACEBAR - jump;
CTRL, C - crouch;
LEFT SHIFT - dash;
```
-----------------------
<h1 align="center"> External assets: </h1>

```
BOXOPHOBIC                   -> simple fog; 
AstarPathfindingProject      -> not used in the demo project
DebugDrawingExtension        -> for best debugging;
MagicaCloth2                 -> for cloth simulations;
RealToon                     -> anime shaders;
Ropofoo - Foot Controller IK -> for foot inverse kinematics;
```
-----------------------
<h1 align="center"> Player movement main scripts: </h1>

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
How it looks in inspector: <br>
![prewiew](_git_readme/platformerComponent.png) <br> 
![prewiew](_git_readme/platformerFolder.png) <br>
![prewiew](_git_readme/statesFolder.png) <br>
