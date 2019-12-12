using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Avatar : MonoBehaviour
{

    // ## MEMBER ##

    private int id;
    private string name;

    // list of joint pools
    private Dictionary<Joint.JointName, Joint> jointPool;

    private Transform rootTransform;

    private float timeLastUpdated = 0.0f;

    // ## INITIALIZATION ##

    void Start() 
    {
        
        InitJoints();

    }

    private void InitJoints()
    {

        // init joint pool
        jointPool = new Dictionary<Joint.JointName, Joint>();

        // get animator
        Animator animator = GetComponent<Animator>();
        // set root transform
        rootTransform = animator.GetBoneTransform(HumanBodyBones.Hips);

        // set avatar to origin
        rootTransform.localPosition = Vector3.zero;

        // joint loop
        foreach (var item in Enum.GetValues(typeof(Joint.JointName)))
        {

            Joint.JointName currentJointName = (Joint.JointName)item;
            Joint currentJoint = GetJointData(currentJointName, animator);

            if (currentJoint != null)
            {

                // add new joint to pool
                jointPool.Add(currentJointName, currentJoint);

            }
        }
    }

    private Joint GetJointData(Joint.JointName jointType, Animator animator)
    {

        var hbb = MapKinectJoint(jointType);
        if (hbb == HumanBodyBones.LastBone)
            return null;

        var transform = animator.GetBoneTransform(hbb);
        if (transform == null)
            return null;

        var localTPoseRotation = GetSkeletonBone(animator, transform.name).rotation;
        var t = transform;
        while (!ReferenceEquals(t, rootTransform))
        {
            t = t.parent;
            localTPoseRotation = GetSkeletonBone(animator, t.name).rotation * localTPoseRotation;
        }

        Quaternion localKinectRotationInverse = GetLocalKinectRotationInverse(jointType);

        // return new joint with localTPoseRotation and localKinectRotationInverse
        return new Joint(jointType, transform, localTPoseRotation, localKinectRotationInverse, rootTransform);

    }

    private static SkeletonBone GetSkeletonBone(Animator animator, string boneName) => animator.avatar.humanDescription.skeleton.First(sb => sb.name == boneName);

    private Quaternion GetLocalKinectRotationInverse(Joint.JointName jointType)
    {
        // Used this page as reference for T-pose orientations
        // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
        // Assuming T-pose as body facing Z+, with head at Y+. Same for target character
        // Coordinate system seems to be left-handed not right handed as depicted
        // Thus inverse T-pose rotation should align Y and Z axes of depicted local coords for a joint with body coords in T-pose
        switch (jointType)
        {
            case Joint.JointName.HIPS:
            case Joint.JointName.SPINE:
            case Joint.JointName.CHEST:
            case Joint.JointName.NECK:
            case Joint.JointName.HEAD:
            case Joint.JointName.UPLEG_L:
            case Joint.JointName.LEG_L:
            case Joint.JointName.FOOT_L:
                return Quaternion.AngleAxis(90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

            case Joint.JointName.TOE_L:
                return Quaternion.AngleAxis(-90, Vector3.up);

            case Joint.JointName.UPLEG_R:
            case Joint.JointName.LEG_R:
            case Joint.JointName.FOOT_R:
                return Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

            case Joint.JointName.TOE_R:
                return Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

            case Joint.JointName.SHOULDER_L:
            case Joint.JointName.ARM_L:
            case Joint.JointName.FOREARM_L:
                return Quaternion.AngleAxis(90, Vector3.right);

            case Joint.JointName.HAND_L:
                return Quaternion.AngleAxis(180, Vector3.right);

            case Joint.JointName.SHOULDER_R:
            case Joint.JointName.ARM_R:
            case Joint.JointName.FOREARM_R:
                return Quaternion.AngleAxis(-90, Vector3.right);

            case Joint.JointName.HAND_R:
                return Quaternion.identity;

            default:
                return Quaternion.identity;
        }
    }

    // ## METHODS ##

    /// <summary>
    /// 
    /// </summary>
    public void updateAvatar()      
    {

        jointPool[Joint.JointName.HIPS].updatePosition();

        foreach (KeyValuePair<Joint.JointName, Joint> currentJoint in jointPool)
        {

            currentJoint.Value.updateRotation();

        }

        timeLastUpdated = 0.0f;

    }

    // ## GETTER AND SETTER ##

    public void setId(int value)
    {

        id = value;

    }

    public void setName(string value)
    {

        name = value;

    }

    public void setJointPose(Joint.JointName jointName, Vector3 position, Quaternion rotation)
    {

        Joint currentJoint;

        if (jointPool.TryGetValue(jointName, out currentJoint))
        {

            currentJoint.setGlobalPosition(ConvertKinectPosition(position));

            currentJoint.setGlobalRotation(rotation);

        }
    }

    public void addTimeLastUpdated()
    {

        timeLastUpdated += Time.deltaTime;

    }

    public float getTimeLastUpdated()
    {

        return timeLastUpdated;

    }

    // ## UTIL ##

    private Vector3 ConvertKinectPosition(Vector3 value)
    {
        // Kinect Y axis points down, so negate Y coordinate
        // Scale to convert millimeters to meters
        // https://docs.microsoft.com/en-us/azure/Kinect-dk/coordinate-systems
        // Other transforms (positioning of the skeleton in the scene, mirroring)
        // are handled by properties of ascendant GameObject's
        return new Vector3(-value.x, value.y, -value.z);
    }

    private HumanBodyBones MapKinectJoint(Joint.JointName joint)
    {
        // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
        switch (joint)
        {
            case Joint.JointName.HIPS: return HumanBodyBones.Hips;
            case Joint.JointName.SPINE: return HumanBodyBones.Spine;
            case Joint.JointName.CHEST: return HumanBodyBones.Chest;
            case Joint.JointName.NECK: return HumanBodyBones.Neck;
            case Joint.JointName.HEAD: return HumanBodyBones.Head;
            case Joint.JointName.UPLEG_L: return HumanBodyBones.LeftUpperLeg;
            case Joint.JointName.LEG_L: return HumanBodyBones.LeftLowerLeg;
            case Joint.JointName.FOOT_L: return HumanBodyBones.LeftFoot;
            case Joint.JointName.TOE_L: return HumanBodyBones.LeftToes;
            case Joint.JointName.UPLEG_R: return HumanBodyBones.RightUpperLeg;
            case Joint.JointName.LEG_R: return HumanBodyBones.RightLowerLeg;
            case Joint.JointName.FOOT_R: return HumanBodyBones.RightFoot;
            case Joint.JointName.TOE_R: return HumanBodyBones.RightToes;
            case Joint.JointName.SHOULDER_L: return HumanBodyBones.LeftShoulder;
            case Joint.JointName.ARM_L: return HumanBodyBones.LeftUpperArm;
            case Joint.JointName.FOREARM_L: return HumanBodyBones.LeftLowerArm;
            case Joint.JointName.HAND_L: return HumanBodyBones.LeftHand;
            case Joint.JointName.SHOULDER_R: return HumanBodyBones.RightShoulder;
            case Joint.JointName.ARM_R: return HumanBodyBones.RightUpperArm;
            case Joint.JointName.FOREARM_R: return HumanBodyBones.RightLowerArm;
            case Joint.JointName.HAND_R: return HumanBodyBones.RightHand;
            default: return HumanBodyBones.LastBone;
        }
    }

    // ## DATA TYPES ##

    public class Joint
    {

        // ## MEMBER ##

        private JointName name;
        private Vector3 globalPosition;
        private Quaternion globalRotation;
        private JointConfidence confidence;

        private Quaternion localTPoseRotation;
        private Quaternion localKinectRotationInverse;

        private Transform jointTransform, rootTransform;

        // ## CONSTRUCTOR ##

        public Joint(JointName _name, Transform _jointTransform, Quaternion _localTPoseRotation, Quaternion _localKinectRotationInverse, Transform _rootTransform)
        {

            name = _name;
            jointTransform = _jointTransform;
            localTPoseRotation = _localTPoseRotation;
            localKinectRotationInverse = _localKinectRotationInverse;
            rootTransform = _rootTransform;

        }
                  
        // ## METHODS ##

        public void updatePosition()
        {

            jointTransform.position = globalPosition;

        }

        public void updateRotation()
        {

            Quaternion rotation = ConvertKinectRotation(globalRotation);
            rotation *= localKinectRotationInverse;
            rotation *= localTPoseRotation;

            Quaternion invParentRotationInCharacterSpace = Quaternion.identity;
            Transform t = jointTransform;
            while (!ReferenceEquals(t, rootTransform))
            {

                t = t.parent;
                invParentRotationInCharacterSpace *= Quaternion.Inverse(t.localRotation);

            }

            //Must be in correct order!!
            jointTransform.localRotation = invParentRotationInCharacterSpace * rotation;

        }

        // ## GETTER AND SETTER ##

        public void setRootTransform(Transform value)
        {

            rootTransform = value;

        }

        public void setName(JointName value)
        {

            name = value;

        }

        public JointName getName()
        {

            return name;

        }

        public void setGlobalPosition(Vector3 value)
        {

            globalPosition = value;

        }

        public Vector3 getGlobalPosition()
        {

            return globalPosition;

        }

        public void setGlobalRotation(Quaternion value)
        {

            globalRotation = value;

        }

        public Quaternion getGlobalRotation()
        {

            return globalRotation;

        }

        public void setConfidence(JointConfidence value)
        {

            confidence = value;

        }

        public JointConfidence getConfidence()
        {

            return confidence;

        }

        public void setLocalTPoseRotation(Quaternion value)
        {

            localTPoseRotation = value;

        }

        public Quaternion getLocalTPoseRotation()
        {

            return localTPoseRotation;

        }

        public Transform getTransform()
        {

            return jointTransform;

        }

        // ## UTIL ##

        private Quaternion ConvertKinectRotation(Quaternion value)
        {
            // Kinect coordinate system for rotations seems to be
            // left-handed Y+ up, Z+ towards camera
            // So apply 180 rotation around Y to align with Unity coords (Z away from camera)
            return Quaternion.AngleAxis(180, Vector3.up) * new Quaternion(value.x, value.y, value.z, value.w);
        }

        // ## DATA TYPES ##

        public enum JointConfidence
        {
            NONE,
            LOW,
            MEDIUM,
            HIGH
        }

        public enum JointName
        {
            HIPS,
            SPINE,
            CHEST,
            NECK,
            SHOULDER_L,
            ARM_L,
            FOREARM_L,
            HAND_L,
            SHOULDER_R,
            ARM_R,
            FOREARM_R,
            HAND_R,
            UPLEG_L,
            LEG_L,
            FOOT_L,
            TOE_L,
            UPLEG_R,
            LEG_R,
            FOOT_R,
            TOE_R,
            HEAD
        };

    }
}
