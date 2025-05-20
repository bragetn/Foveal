# Eye Tracking Module

## Description

The original Foveal project developed in **Unity Game Engine** was primarily centered around creating and running eye tracking tests for hemispatial neglect assessment.
This module is the reimplimentation of this functionality as part of the new **Foveal Godot**, and was for the time being named "Eye Tracking".
The name may be subject to change in the future.

The module includes the following:
- An editor for creating and editing **Gaze Tests**
- A stage for running **Gaze Tests** and saving the result

## Contents

### Assets

This directory contains visual and/or audio resources, specific to this module.

- **Sprites**: PNG images used in menus
- **Themes**: Godot themes for UI

### Autoloads

This directory contains Godot-autoloads specific to the **Eye Tracking** module.
As of now, it only contains the **EyeTrackingRadio** autoload, which hosts numerous signals for communicating between nodes like menus and entities in this module.

### Data Models

This directory contains data-classes used in the **Eye Tracking** module:

#### GazeTargetData

Data-class for storing the state of a single **GazeTarget**.
Includes the following data:

- **X (float)**: The local x-position of the **GazeTarget** relative to the **TargetBox**
- **Y (float)**: The local y-position of the **GazeTarget** relative to the **TargetBox**
- **Z (float)**: The local z-position of the **GazeTarget** relative to the **TargetBox**
- **Radius (float)**: The radius of the **GazeTarget**
- **Delay (float)**: The delay in seconds before the **GazeTarget** appears in a test
- **Type (int)**: The type of the **GazeTarget** (0, 1 or 2)

#### GazeTestData

Data-class for storing the state of a whole **GazeTest**.
Includes the following data:

- **Name (string)**: The name of the test
- **GazeTime (float)**: The default **GazeTime** in seconds of each **GazeTarget** in the test
- **ColliderSize (float)**: The scale of the collider of the **GazeTarget** (the scale is multiplied with the radius of the **GazeTarget** to determine the radius of the collider)
- **Targets (List<GazeTargetData>)**: A list of **GazeTargetData**, storing the state of each **GazeTarget** in the test

#### TargetResultData

Data-class for storing result data of a single **GazeTarget** after a test has been performed.
Includes the following data:

- **GazeTargetData (GazeTargetData)**: The state of the **GazeTarget**
- **HasBeenSeen (bool)**: Indicates whether or not the target has been seen (looked at for **GazeTime** duration) during the test
- **TimeBeforeSeen (TimeSpan)**: The time before the target was seen during the test, will be zero if the target was not seen

#### TestResultData

Data-class for storing result data of the whole **GazeTest** after it has been performed.
Includes the following data:

- **TestName (string)**: The name of the test that has been performed
- **TestTime (TimeSpan)**: The total duration of the test
- **GazeTime (float)**: The **GazeTime** in seconds used in the test
- **PlayerDistance (float)**: The distance of the player used in the test
- **TargetResults (List<TargetResultData>)**: A list of **TargetResultData**, storing the result of each **GazeTarget** after the test

### Entities

This directory contains scenes and node hierarchies representing interactive elements, specific to the **Eye Tracking** module.

- **EditorMenuScene**: A scene containing the editor-menu and a virtual keyboard
- **GazeTarget**: The targets used in a **GazeTest**, are both gazeable and grabbable
- **GazeTestItem**: 
- **TargetBox**: A dynamically sized box that manages **GazeTarget** nodes, handles most of the **GazeTest** logic 

### Stages

This directory contains self-contained environments or "levels" within the **Eye Tracking** module.
These scenes are inherited from **SceneBase** in **Foveal Core** which includes a player and core logic.

- **RunTest**: A stage for running **Gaze Tests** and saving the result
- **TestEditor**: An editor for creating and editing **Gaze Tests**

### UI

This directory contains 2D interface elements specific to the **Eye Tracking** module.
These scenes are used for UI components like menus and in-stage overlays.

- **ContdownTimerMenu**: A menu for displaying a countdown timer, used in **RunTest**
- **Editor**: Contains all the menus used in the editor-menu in **TestEditor**
- **ModuleMenu**: 
- **Player**: Contains all the menus used in the player hand-menu in **TestEditor**
- **PreviewTest**: A simple menu for starting and stopping test previews in **TestEditor**
- **RunTestMenu**: The main UI-menu used in **RunTest**