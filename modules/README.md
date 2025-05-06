# Modules

## Description

This is the directory for **Foveal Modules**.
A module in this context refers to self-contained *projects* for various neuropsychological XR experiments.
These should ideally encapsulate assets, stages, entities, etc, which are unique to the module, while relying on **Foveal Core** for common shared functonality between modules. 
The project architecture was designed this way in order to more easily accommodate future experiments and developers, by allowing pseudo-independent *projects* that should not conflict or depend on each other.

## Contents

### Eye Tracking

The original Foveal project developed in **Unity Game Engine** was primarily centered around creating and running eye tracking tests for hemispatial neglect assessment.
This module is the reimplimentation of this functionality as part of the new **Foveal Godot**, and was for the time being named "Eye Tracking".
The name may be subject to change in the future.

The module includes the following:
- An editor for creating and editing **Gaze Tests**
- A stage for running **Gaze Tests** and saving the result

### Example Module

An example module for future developers to use as an example and/or template for new **Foveal Modules**.

The module includes the following:
- A simple example stage with a player, environment, and 3D-viewport
- An example of a generic menu (which is interactable by the player)
- The UI for the module in main menu (only has a button for switching to the example stage)
