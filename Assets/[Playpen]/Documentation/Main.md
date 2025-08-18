
# Changes
- The project started from the "VR" template.
- `AutoHand` package was imported.
- `Polygon sci-fi city` package was imported.
- Created the `Playpen` scene as a copy of the `XRHands` from the AutoHand OpenXR package.
- The `Auto Hand Player Container/TrackerOffsets/Highlight Projection (R)/OuterMesh` uses the `Playpen\Assets\AutoHand\Examples\Materials\Highlight\Hands_transparent.mat` material, which uses the `Playpen\Assets\[Playpen]\Shaders\Hands_transparfent.shader` shader. This shader was not compiling, and has been replaced with the `Playpen\Assets\[Playpen]\Shaders\Hands_transparent.shader` shader.
- Imported the `DOTS_RTS_Course_VisualAssets_Part1.unitypackage` package.
- Created `Assets/[Playpen]/DOTS/DOTS URP Config` from `Assets/Settings/Project Configuration/Quality URP Config`.
- As suggested on the Unity "Entities project setup" page, "Enter Play Mode Settings" in "Project Settings/Editor" was set to "Do not reload Domain or Scene".

# Notes
- A "DOTS" quality level has been created and is the default quality level for PC.
- The "DOTS" quality level uses the "DOTS URP Config" render pipeline asset.
- The "DOTS URP Config" render pipeline asset uses the "DOTS URP Preset" Universal Render Data.
- 

# Issues
- Shadow not showing
  - Move the camera close. If you can see the shadow, then the shadow distance is too low. This is set in the active render pipeline asset.
  - 