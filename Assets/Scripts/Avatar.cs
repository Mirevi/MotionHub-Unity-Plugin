﻿using System;
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

        // return new joint with localTPoseRotation and localKinectRotationInverse
        return new Joint(jointType, transform, localTPoseRotation, rootTransform);

    }

    private static SkeletonBone GetSkeletonBone(Animator animator, string boneName) => animator.avatar.humanDescription.skeleton.First(sb => sb.name == boneName);


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

        private Transform jointTransform, rootTransform;

        // ## CONSTRUCTOR ##

        public Joint(JointName _name, Transform _jointTransform, Quaternion _localTPoseRotation, Transform _rootTransform)
        {

            name = _name;
            jointTransform = _jointTransform;
            localTPoseRotation = _localTPoseRotation;
            rootTransform = _rootTransform;

        }
                  
        // ## METHODS ##

        public void updatePosition()
        {

            jointTransform.position = globalPosition;

        }

        public void updateRotation()
        {

            Quaternion rotation = globalRotation;

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

        public void setGlobalPosition(Vector3 value)
        {

            globalPosition = value;

        }

        public void setGlobalRotation(Quaternion value)
        {

            globalRotation = value;

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
