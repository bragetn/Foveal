# Common

## Description

This is the directory for common reusable logic, utilities and components that are shared across **Foveal Core** and other **Foveal Modules**.

## Contents

### Autoloads

This directory contains Godot-autoloads, which are synonymous to singletons.
Currently, this only contains the **CoreRadio** autoload, which hosts globally accessible signals used to communicate to and from certain core components and systems.

These are the signals of **CoreRadio** as of now:

- **LoadScene(path)**: Tells **Staging** (main scene) to swich the current stage with the stage specified by *path*
- **ToggleGazeDot()**: Tells **EyeTracker** to show/hide the gaze dot
- **GrabEntered(grabbedNode)**: Used by the **Grabber** extension to tell other nodes when and what was grabbed
- **GrabExited()**: Used by the **Grabber** extension to tell other nodes when stopped grabbing
- **ToggleAdminMode()**: Disables **Admin Mode** if enabled, enables it if disabled
- **CalibrateEyeTracker()**: Tells **EyeTrackerCalibrator** to start the calibration rutine

**CoreRadio** also contains a public boolean property named "AdminMode", which is used by different stages and components to know if to enable or disable admin features.

### Data Models

This directory contains data-classes used in **Foveal Core**:

#### GazeSampleEntry

Used by the **GazeSampler** extension to store sample data from **EyeTracker**.
The following data is sampled:

- **Time (TimeSpan)**: The elapsed time after **GazeSampler** was started
- **CameraPosition (float, float, float)**: The global position of the main camera
- **CameraRotation (float, float, float)**: The global rotation of the main camera
- **GazeRayRotation (float, float, float)**: The rotation of the gaze ray from **EyeTracker**
- **HitPoint (float, float, float)**: The intersection point between the gaze ray and world geometry from **EyeTracker**, will be NaN if there is no intersection

#### ModuleResource

A godot resource class for dynamic **Foveal Modules** in **MainMenu**.
Contains the following data:

- **ModuleName (string)**: The name of the module in **MainMenu**
- **ModuleMenuScene (PackedScene)**: The UI scene to display for the module in **MainMenu**

### Extensions

This directory contains **Foveal Extensions**, which can be added to a XROrigin3D node to enable certain features like eye-tracking.

#### EyeTracker

This is an extension that uses the eye-tracking functionality of the headset to send raycasts in the eye-gaze direction during physics process. It also visualizes where the player is looking with a **GazeDot**. The raycasts will only affect objects that impliments the **IGazeable** interface.

Export properties:

- **Enabled (bool)**: Controls whether or not to send raycasts in physics process
- **RayLength (float)**: Determines the length of the ray used in the physics process raycast
- **Camera (XRCamera3D)**: The camera node the raycast will be sendt from
- **ShowGazeDot (bool)**: Determines if the **GazeDot** is initialy visible or not
- **GazeDotDistance (float)**: Determines the initial distance away from the camera of the **GazeDot**
- **GazeDotRadius (float)**: Determines the initial radius of the **GazeDot**

#### GazeSampler

This is an extension for collecting samples from an **EyeTracker** extension node, and storing them as a list of **GazeSampleEntry** objects. It has public methods for starting and stopping the sampler, and for retrieving the list of the samples.

Export properties:

- **ETracker (EyeTracker)**: The **EyeTracker** extension node which **GazeSampler** should collect samples from

#### Grabber

This is an extension that uses pointer-events from a **Godot XR Tools** function-pointer, to interact with objects that impliments **IGrabbable**. It also temporarily disables the movement-turn node used by the same controller (if exists), in order to moved grabbed targets with the controller joystick.

Export properties:

- **TargetMoveSpeed (float)**: The speed to move grabbed targets towards or away from the pointer origin
- **Pointer (Node3D)**: The **Godot XR Tools** function-pointer node used to interact with the game-world
- **HandController (XRController3D)**: The xr-contoller of the hand with the function-pointer and movement-turn
- **MovementTurn (Node)**: The **Godot XR Tools** movement-turn node used to rotate the player

### Interfaces

This directory contains **Foveal Core** interfaces which can be implimented by other components to become interactable with **Foveal Extensions**.

#### IGazeable

Used in conjunction with the **EyeTracker** extension.

Contains the following methods:

- **OnGazeEnter()**: Triggered on the first physics update the gaze intersects the object
- **OnGazeStay(double delta)**: Called continuously on physics updates while the gaze ramains on the object
- **OnGazeExit()**: Triggered on the first physics update the gaze leaves the object

#### IGrabbable

Used in conjunction with the **Grabber** extension.

Contains the following methods:

- **OnGrabEnter(PointerUtil.PointerEvent pointerEvent)**: Triggered on the **Godot XR Tools** pointer-event **Pressed**, on the current object that intersects the function-pointer
- **OnGrabStay(double delta, float value)**: Called continuously on physics updates on the current grabbed target of the **Grabber** extension, provides the joystick Y-input of the XRController as *value*
- **OnGrabExit()**: Triggered on the **Godot XR Tools** pointer-event **Released**, on the current object that intersects the function-pointer

### Shaders

This directory contains globally accessible GDShaders to be used in **Foveal Core** and **Foveal Modules**.
Currently hosts the *checkerboard* shader, which can be used with a **ShaderMaterial** to give meshes a simple configurable checkerboard pattern as the name implies.

The *checkerboard* shader has the following configurable shader uniforms:

- **color_a**: Vec3
- **color_b**: Vec3
- **scale**: float
- **offset**: Vec3

### Utils

This directory contains static utility classes for the project.
As of now the only util is the **PointerUtil** class, which is used to parse a **Godot XR Tools** pointer-event from GDScript to a custom **PointerEvent** class to be used in C#.