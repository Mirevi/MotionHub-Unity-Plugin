# MotionHubUnityPlugin

This is the receiver Plugin for the [MIREVI MotionHub](https://github.com/Mirevi/MotionHub). You can integrate this in your Project to receive Body Tracking Data for Character animation from the MotionHub.

# Project Setup

- download or clone this unity project
- open the AvatarExample scene and press play

# Package Setup
- you can add the Plugin to an existing project as well by using the .unityPackage file from our [Releases site](https://github.com/Mirevi/MotionHub-Unity-Plugin/releases).
- import the package and add the #manager-Prefab to your scene
- check the In Port on the $oscmanager, it should be 7000
- as soon as the motionHub ist streaming data to the correct address and the unity scene is on playMode, you should see an animated character
