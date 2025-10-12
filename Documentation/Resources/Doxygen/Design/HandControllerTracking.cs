/*!
  \page HandWristTracking Hand/Wrist Tracking

  \section HandWristTrackingAvatar Setting the Position & Rotation of Avatar Hands

  \subsection HandWristTrackingAvatarIntro Introduction
  The app uses AutoHand as a foundation for controller and hand tracking. Among other useful functionality, AutoHand has functionality which makes it possible to make the hands collide with geometry and not clip through it. To accomplish this, a `follow` object is specified. The hand follows this object, but will offset the hand to prevent clipping when the hand is colliding with another object.

  \subsection HandWristTrackingAvatarFollowObject Follow Object
  The `AutoHand Avatar Support` prefab contains "target" GameObjects. The `Start` method of `AutoHandPlayer` sets these objects to follow tracked devices.  These GameObjects have child GameObjects; one for each of the following: OpenXR hands, WebXR hands, and controllers. These objects specify a platform and tracking type specific transform offset and have the names `[openXR|webXR|controller]HandTargetOffsetTransform[Left|Right]`, depending on the platform and hand. The appropriate child GameObject is set as the follow target for the AutoHand in the `Update` method of `AutoHandPlayer`.

  Note: The current implementation will not work with positional offsets.


  \section HandWristTrackingWrist Setting the Position & Rotation of IK Arm Targets

  \subsection HandWristTrackingWristIntro Introduction
  The app uses a `TwoBoneIKConstraint` per arm to drive arm bending with hand positions and rotations. The `TwoBoneIKConstraint` components have `Target` editor variables set, which are used as the target positions & rotations of the wrists. The `IKRiggedActor` component is responsible for setting the positions and rotations of the target objects.

  \subsection HandWristTrackingIKRiggedActor IKRiggedActor
  As mentioned, the `IKRiggedActor` component is used to set the positions and rotations of the IK target objects. When an IK rigged actor is created in `IKRiggedAvatarFactory.CreateAvatar`, a reference to the avatar's `IKRiggedActor` component is obtained and a set of tracked source objects are specified via a call to `SetTrackedSources`. For a remote actor, the coresponding networked objects are used as the source objects. For local actors, the AutoHand position and rotation are used as the source objects. The AutoHand position and rotation are used as the tracked source objects since the AutoHand will prevent the hand from clipping through geometry.

  In the `Update` method of `IKRiggedActor`, the position and rotation of the IK targets are set by combining the tracked source object's position and rotation and the offset values specified in the adjacent `TrackedParts` component.



*/

