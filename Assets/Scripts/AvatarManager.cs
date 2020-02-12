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

    private Camera mainCam;

    public bool debugShowAvatarName = false;
    public bool debugShowAvatarPosition = false;
    public bool debugShowAvatar = true;
    public bool debugShowJoint = false;
    public bool debugShowJointName = false;

    public Font debugFontLarge, debugFontSmall;


    private void Awake()
    {


        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // init avatar pool
        avatarPool = new Dictionary<int, GameObject>();

    }

    void Start() 
    {


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
                                    -1.0f * message.GetFloat(indexJoint * 8 + 1),
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

            int currTrackerID = (int)Mathf.Floor(id / 1000.0f);


            currAvatar.trackerID = currTrackerID;

            currAvatar.skeletonID = id - currTrackerID * 1000;

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

                Debug.Log("[INFO]: AvatarManager::OnReceiveSkeleton(): Removed avatar with id = " + currAvatar.Key);

                return;

            }
            else
            {

                currAvatarClass.addTimeLastUpdated();

            }
        }
    }

    private void OnGUI()
    {

        Vector3 screenSpace;

        Dictionary<int, GameObject> avatarPoolCopy = avatarPool;

        foreach (KeyValuePair<int, GameObject> currAvatar in avatarPoolCopy)
        {

            if (currAvatar.Value != null)
            {

                if (debugShowAvatarName)
                {

                    GUI.skin.font = debugFontLarge;
                    GUI.color = Color.black;

                    screenSpace = mainCam.WorldToScreenPoint(currAvatar.Value.GetComponent<Avatar>().getJoint(Avatar.Joint.JointName.HEAD).getTransform().position);

                    if (screenSpace.x > 0 && screenSpace.x < Screen.width && screenSpace.y > 0 && screenSpace.y < Screen.height && screenSpace.z > 0)
                        GUI.Label(new Rect(screenSpace.x - 32, Screen.height - screenSpace.y - 96, 256, 64), currAvatar.Value.name);

                }

                if (debugShowAvatarPosition)
                {

                    GUI.skin.font = debugFontSmall;
                    GUI.color = Color.black;

                    screenSpace = mainCam.WorldToScreenPoint(currAvatar.Value.GetComponent<Avatar>().getJoint(Avatar.Joint.JointName.HEAD).getTransform().position);

                    if (screenSpace.x > 0 && screenSpace.x < Screen.width && screenSpace.y > 0 && screenSpace.y < Screen.height && screenSpace.z > 0)
                        GUI.Label(new Rect(screenSpace.x - 32, Screen.height - screenSpace.y - 64, 128, 64), currAvatar.Value.GetComponent<Avatar>().getJoint(Avatar.Joint.JointName.HIPS).getTransform().position.ToString());

                }

                currAvatar.Value.GetComponent<Avatar>().toggleShowAvatar(debugShowAvatar);

                currAvatar.Value.GetComponent<Avatar>().toggleDebug(debugShowJoint);

                if (debugShowJointName)
                {

                    GUI.skin.font = debugFontSmall;
                    GUI.color = Color.gray;

                    foreach (KeyValuePair<Avatar.Joint.JointName, Avatar.Joint> currJoint in currAvatar.Value.GetComponent<Avatar>().getJointPool())
                    {

                        screenSpace = mainCam.WorldToScreenPoint(currJoint.Value.getTransform().position);

                        if (screenSpace.x > 0 && screenSpace.x < Screen.width && screenSpace.y > 0 && screenSpace.y < Screen.height && screenSpace.z > 0)
                            GUI.Label(new Rect(screenSpace.x, Screen.height - screenSpace.y, 128, 64), currJoint.Key.ToString());

                    }
                }
            }
        }
    }

    public void toggleShowAvatarName(bool value)
    {

        debugShowAvatarName = value;

    }

    public void toggleShowAvatarPosition(bool value)
    {

        debugShowAvatarPosition = value;

    }

    public void toggleShowAvatar(bool value)
    {

        debugShowAvatar = value;

    }

    public void toggleShowJoint(bool value)
    {

        debugShowJoint = value;

    }

    public void toggleShowJointName(bool value)
    {

        debugShowJointName = value;

    }
}
