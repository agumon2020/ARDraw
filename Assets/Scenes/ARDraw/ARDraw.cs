using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARDraw : MonoBehaviour
{
    public GameObject LineObject;
    public Camera TargetCamera;
    private LineRenderer line;
    public float DrawOffset;
    private int i;
    private bool DrawLine;
    public float LineWidth;
    private GameObject clone;

    // Start is called before the first frame update
    void Start()
    {
        DrawLine = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                // Record initial touch position.
                case TouchPhase.Began:
                    clone = (GameObject)Instantiate(LineObject);
                    clone.tag = "ARLines";
                    line = clone.GetComponent<LineRenderer>();
                    line.startWidth = LineWidth;
                    line.endWidth = LineWidth;
                    i = 0;
                    DrawLine = true;
                    break;
                case TouchPhase.Moved:
                    break;
                case TouchPhase.Ended:
                    i = 0;
                    DrawLine = false;
                    break;
            }

            if (DrawLine)
            {
                i++;
                line.positionCount = i;
                line.SetPosition(i - 1, TargetCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y,DrawOffset)));
                //line.SetPosition(i - 1, TargetCamera.transform.position + DrawOffset * TargetCamera.transform.forward);
            }
        }
    }
}
