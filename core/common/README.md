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

### Extentions

### Interfaces

### Shaders

### Utils
