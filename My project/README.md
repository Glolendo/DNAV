# DNAV
Overview

DNAV is a Unity-based Virtual Reality (VR) navigation system designed to help users navigate a hospital-style environment. The project demonstrates VR interaction, environment generation, and navigation guidance using Unity and C# scripts. Users can move through the environment, interact with objects, and follow navigation goals to reach different destinations.

**Features**

  VR hand tracking and interaction

  Object grabbing and manipulation

  Navigation system with destination goals

  Automatic hospital layout generation

  Interactive UI buttons

**Technologies Used**
  
  Unity Game Engine

  C#

  VR Interaction Scripts

  GitHub for version control

  Scripts Description
  
**Navigation**

  NavigationManager.cs – Controls the navigation system and manages destination goals.

  DestinationGoal.cs – Defines the target destination for navigation.

  DestinationTrigger.cs – Detects when a user reaches a destination.

**Environment Layout**

  HospitalAutoBuilder.cs – Automatically generates the hospital environment layout.

  LayoutBuilderSimple.cs – Builds a basic layout of the environment.

  OptimizedLayoutBuilder.cs – Creates a more efficient version of the layout.

**VR Interaction**

  HandFollow.cs – Allows the VR hands to smoothly follow the user's real hand movements.

  ObjectGrab.cs – Enables users to grab and interact with objects in the environment.

**UI and Environment Controls**

  UIManager.cs – Handles button interactions and UI events.

  WallColorChanger.cs – Allows the user to change wall colors in the environment.

**How to Run the Project**

  Clone the repository:

  git clone https://github.com/Glolendo/DNAV.git

  Open the project in Unity Hub.

  Open the main scene located in the Scenes folder.

  Press Play in Unity to start the VR navigation experience.
