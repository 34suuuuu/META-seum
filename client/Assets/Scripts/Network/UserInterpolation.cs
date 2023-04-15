using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterpolationInfo
{
    public Vector3  start, dest;
    public float progress;

    public UserInterpolationInfo(Vector3 startV, Vector3 destV)
    {
        start = startV;
        dest = destV;
        progress = 0f;
    }
}

public class UserInterpolation : MonoBehaviour
{
    float moveSpeed;
    public Dictionary<GameObject, UserInterpolationInfo> usersToInterpolate;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 4f;
        usersToInterpolate = new Dictionary<GameObject, UserInterpolationInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if(usersToInterpolate.Count > 0)
        {
            foreach(KeyValuePair<GameObject, UserInterpolationInfo> user in usersToInterpolate)
            {
                if (user.Value.progress < 1f)
                {
                    user.Value.progress += moveSpeed * Time.deltaTime;
                    user.Key.transform.position = Vector3.Lerp(user.Value.start, user.Value.dest, user.Value.progress);
                }
                else
                {
                    //usersToInterpolate.Remove(user.Key);
                }
            }
        }
    }

    public void Move(GameObject go, Vector3 beforePos, Vector3 currentPos)
    {
        usersToInterpolate[go] = new UserInterpolationInfo(beforePos, currentPos);
    }
}
