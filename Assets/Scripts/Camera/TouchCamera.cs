using UnityEngine;
public class TouchCamera : MonoBehaviour
{
    Vector2?[] oldPos = {
        null,
        null
    };
    Vector2 oldVec;
    float oldDist;

    int updatecount = 0;
    Vector3 oriPos;
    Quaternion oriRot;

    void Update()
    {
        if (updatecount == 0)
        {
            // initialize original position
            oriPos = transform.position;
            oriRot = transform.localRotation;
        }
        else if (Input.touchCount == 4)
        {
            // reset camera to original posotioin
            transform.position = oriPos;
            transform.localRotation = oriRot;
        }
        else if (Input.touchCount == 0)
        {
            // initialize old pos - note it was declared to be nullable
            oldPos[0] = null;
            oldPos[1] = null;
        }
        else if (Input.touchCount == 1)
        {
            if (oldPos[0] == null || oldPos[1] != null)
            {
                oldPos[0] = Input.GetTouch(0).position;
                oldPos[1] = null;
            }
            else
            {
                var camera = GetComponent<Camera>();
                Vector2 newPos = Input.GetTouch(0).position;

                var delt = (Vector3)((oldPos[0] - newPos) * camera.orthographicSize / camera.pixelHeight * 2f);
                transform.position += transform.TransformDirection(delt);

                oldPos[0] = newPos;
            }
        }
        else
        {
            if (oldPos[1] == null)
            {
                oldPos[0] = Input.GetTouch(0).position;
                oldPos[1] = Input.GetTouch(1).position;
                oldVec = (Vector2)(oldPos[0] - oldPos[1]);
                oldDist = oldVec.magnitude;
            }
            else
            {
                var camera = GetComponent<Camera>();
                Vector2 screen = new Vector2(camera.pixelWidth, camera.pixelHeight);

                Vector2[] newPos = {
                    Input.GetTouch(0).position,
                    Input.GetTouch(1).position
                };
                Vector2 newVec = newPos[0] - newPos[1];
                float newDist = newVec.magnitude;

                Vector3 deltold = (Vector3)((oldPos[0] + oldPos[1] - screen) * camera.orthographicSize / screen.y);
                transform.position += transform.TransformDirection(deltold);

                float clampToOne = Mathf.Clamp((oldVec.y * newVec.x - oldVec.x * newVec.y) / oldDist / newDist, -1f, 1f);
                float zrot = Mathf.Asin(clampToOne) / 0.0174532924f;
                transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, zrot));

                camera.orthographicSize *= oldDist / newDist;
                Vector2 deltnew = (newPos[0] + newPos[1] - screen) * camera.orthographicSize / screen.y;
                transform.position -= transform.TransformDirection(deltnew);

                oldPos[0] = newPos[0];
                oldPos[1] = newPos[1];
                oldVec = newVec;
                oldDist = newDist;
            }
        }
        updatecount++;
    }
}
