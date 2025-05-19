# Stages

## Description

This is the directory for self-contained environments or "levels" within **Foveal Core**.
These scenes include a XROrigin3D node with a XRCamera3D and two XRController3D nodes for each hand controller.

## Contents

- **staging**: The *main scene* of **Foveal Godot**, includes functionality for scene transitions and loading, from **Godot XR Tools** 
- **scene_base**: The base scene for stages in **Foveal Godot**, should be inherited by new stages in order to work with scene transitions, from **Godot XR Tools**
- **main_hub**: The main hub of **Foveal Godot**, includes the main menu with **Foveal Modules** and the **eye_tracker_calibrator**, inherited scene from **scene_base**