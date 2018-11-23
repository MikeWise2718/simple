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

    const int nmsgline = 8;
    string [] msg = new string[8] { "", "", "", "", "", "", "", "" };
    int[] nmsghit = new int[8];
    private void OnGUI()
    {
        if (updatecount > 0)
        {
            for (int i = 0; i < nmsgline; i++)
            {
                float x = 0;
                float y = 0;
                if (msg[i] != "")
                {
                    GUI.Label(new Rect(x, y + i * 30, 200, 100), msg[i]);// needs to be wide enough or it will break lines
                }
            }
        }
    }

    void Update()
    {
        var camera = GetComponent<Camera>();
        Vector2 screen = new Vector2(camera.pixelWidth, camera.pixelHeight);
        var cfak = camera.orthographicSize / camera.pixelHeight;

        var tcnt = Input.touchCount;
        string pfix = Input.touchCount + "-" + updatecount.ToString()+"-"+nmsghit[tcnt]+":";
        nmsghit[tcnt]++;

        if (updatecount == 0)
        {
            // initialize original position
            oriPos = transform.position;
            oriRot = transform.localRotation;
        }
        switch (tcnt)
        {

            case 0:
                {
                    // initialize old pos - note it was declared to be nullable
                    oldPos[0] = null;
                    oldPos[1] = null;
                    msg[0] = pfix + " reset touch";
                    break;
                }
            case 1:
                {
                    if (oldPos[0] == null || oldPos[1] != null)
                    {
                        oldPos[0] = Input.GetTouch(0).position;
                        oldPos[1] = null;
                    }
                    else
                    {
                        Vector2 newPos = Input.GetTouch(0).position;


                        var delt = (Vector3)((oldPos[0] - newPos) * cfak * 2f);
                        transform.position += transform.TransformDirection(delt);

                        oldPos[0] = newPos;
                    }
                    msg[1] = pfix + " finger 1";
                    break;
                }
            case 2:
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
                        Vector2[] newPos = {
                    Input.GetTouch(0).position,
                    Input.GetTouch(1).position
                };
                        Vector2 newVec = newPos[0] - newPos[1];
                        float newDist = newVec.magnitude;

                        Vector3 deltold = (Vector3)((oldPos[0] + oldPos[1] - screen) * cfak);
                        transform.position += transform.TransformDirection(deltold);

                        float clampToOne = Mathf.Clamp((oldVec.y * newVec.x - oldVec.x * newVec.y) / oldDist / newDist, -1f, 1f);
                        float zrot = Mathf.Asin(clampToOne) / 0.0174532924f;
                        transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, zrot));

                        camera.orthographicSize *= oldDist / newDist;
                        Vector2 deltnew = (newPos[0] + newPos[1] - screen) * cfak;
                        transform.position -= transform.TransformDirection(deltnew);

                        oldPos[0] = newPos[0];
                        oldPos[1] = newPos[1];
                        oldVec = newVec;
                        oldDist = newDist;
                    }
                    msg[2] = pfix + " finger 2";
                    break;
                }
            case 4:
                {
                    // reset camera to original posotioin
                    transform.position = oriPos;
                    transform.localRotation = oriRot;
                    msg[4] = pfix + " reset camera";
                    break;
                }
            default: break;
        }
        updatecount++;

       // msg = "tc:"+Input.touchCount.ToString()+ " cos:"+ camera.orthographicSize + " pixy:"+ camera.pixelHeight+ "  cfak:"+cfak;
    }
}
