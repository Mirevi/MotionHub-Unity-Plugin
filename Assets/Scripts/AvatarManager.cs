using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager : MonoBehaviour
{

    private OSC oscmanager;

    public GameObject avatarPrefab;

    Dictionary<int, GameObject> avatarPool;

    [Range(0.01f, 10.0f)]
    public float timeout = 0.25f; 

    void Start() 
    {

        // init avatar pool
        avatarPool = new Dictionary<int, GameObject>();

        // find oscmanager refference
        oscmanager = GameObject.Find("$oscmanager").GetComponent<OSC>();

        // set callback function for on receive osc messages
        oscmanager.SetAddressHandler("/mh/skeleton/", OnReceiveSkeleton);

    }

    public void OnReceiveSkeleton(OscMessage message)
    {

        // get skeleton id
        int id = message.GetInt(0);

        // check if avatar is already present
        GameObject currAvatarObject;
        Avatar currAvatar;
        avatarPool.TryGetValue(id, out currAvatarObject);

        if (currAvatarObject != null) 
        {

            // get avatar class refference
            currAvatar = currAvatarObject.GetComponent<Avatar>();

            Vector3 globalPositionJoint;
            Quaternion globalRotationJoint;

            // loop through all joints and update joint pose
            for (int indexJoint = 0; indexJoint < 21; indexJoint++) 
            {

                // get global position
                globalPositionJoint = new Vector3(
                                            message.GetFloat(indexJoint * 8 + 1),
                                            message.GetFloat(indexJoint * 8 + 2),
                                            message.GetFloat(indexJoint * 8 + 3)
                                            );

                // get global rotation
                globalRotationJoint = new Quaternion(
                                                message.GetFloat(indexJoint * 8 + 4),
                                                message.GetFloat(indexJoint * 8 + 5),
                                                message.GetFloat(indexJoint * 8 + 6),
                                                message.GetFloat(indexJoint * 8 + 7)
                                                );

                // set joint pose
                currAvatar.setJointPose((Avatar.Joint.JointName)indexJoint, globalPositionJoint, globalRotationJoint);                

            }

            // update joint pose
            currAvatar.updateAvatar();

        }
        // create a new avatar
        else 
        {

            // instantiate new avatar and add it to avatar pool
            currAvatarObject = Instantiate(avatarPrefab, transform);
            avatarPool.Add(id, currAvatarObject);

            // get avatar class
            currAvatar = currAvatarObject.GetComponent<Avatar>();

            // set avatar object name and refference in class
            currAvatarObject.name   = "avatar_" + id;
            currAvatar.setName("avatar_" + id);

            Debug.Log("[INFO]: AvatarManager::OnReceiveSkeleton(): Created new avatar with id = " + id);

        }
    }

    private void Update()
    {

        Avatar currAvatarClass;

        foreach (KeyValuePair<int, GameObject> currAvatar in avatarPool)
        {

            currAvatarClass = currAvatar.Value.GetComponent<Avatar>();

            if(currAvatarClass.getTimeLastUpdated() >= timeout)
            {
                
                avatarPool.Remove(currAvatar.Key);
                Destroy(currAvatar.Value);

                return;

            }
            else
            {

                currAvatarClass.addTimeLastUpdated();

            }
        }
    }
}
