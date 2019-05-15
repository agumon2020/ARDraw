using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDraw : MonoBehaviour
{
    private GameObject clone;
    private LineRenderer line;
    private int i;
    public GameObject LineObject;
    public Camera TargetCamera;
    public float LineWidth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clone = (GameObject)Instantiate(LineObject, LineObject.transform.position, transform.rotation);
            line = clone.GetComponent<LineRenderer>();
            line.startWidth = LineWidth;
            line.endWidth = LineWidth;
            i = 0;
        }
        if (Input.GetMouseButton(0))
        {
            i++;
            line.positionCount = i;
            line.SetPosition(i - 1, TargetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15)));
        }
    }
}
