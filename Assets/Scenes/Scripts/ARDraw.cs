using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 枚举Shader4种状态类型
/// </summary>
public enum RenderingMode
{
    Opaque,
    Cutout,
    Fade,
    Transparent,
}

public class ARDraw : MonoBehaviour
{
    public GameObject LineObject;
    private Camera TargetCamera;
    private LineRenderer line;
    public float DrawOffset;
    private int LineDividNumber;
    private bool IsDrawing;
    public float LineWidth;
    private GameObject clone;

    public GameObject DrawSpace;

    private Material CurrentMaterial;

    public GameObject ColorPicker;
    public Material ColorPickerMaterial;

    Color lerpedColor = Color.white;

    public GameObject ColorPopup;
    public GameObject MaterialPopup;

    // Start is called before the first frame  m   
    void Start()
    {
        TargetCamera = Camera.main;
        //依据触摸的状态，确定绘画的状态
        IsDrawing = false;
        //DrawSpace = GameObject.Find("DrawSpace");
        //CurrentMaterial = new Material(Shader.Find("White"));
        CurrentMaterial = Resources.Load("Materials/White", typeof(Material)) as Material;
        //Debug.Log(Application.dataPath + "/Scenes/ARDraw/Material/White");
        Debug.Log(CurrentMaterial);

        InitTouchControl();
       


    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.touchCount > 0)
        //{
        //    if(!IsPointerOverUIObject())
        //    {
        //        Touch touch = Input.GetTouch(0);
        //        switch (touch.phase)
        //        {
        //            // 记录初始的点击位置
        //            case TouchPhase.Began:
        //                clone = (GameObject)Instantiate(LineObject);
        //                clone.tag = "ARLines ";
        //                clone.GetComponent<LineRenderer>().material = CurrentMaterial;
        //                clone.transform.parent = DrawSpace.transform;
        //                line = clone.GetComponent<LineRenderer>();
        //                line.startWidth = LineWidth;
        //                line.endWidth = LineWidth;
        //                i = 0;
        //                IsDrawing = true;
        //                break;
        //            case TouchPhase.Moved:
        //                break;
        //            case TouchPhase.Ended:
        //                i = 0;
        //                IsDrawing = false;
        //                break;
        //        }

        //        if (IsDrawing)
        //        {
        //            i++;
        //            line.positionCount = i;
        //            //line.SetPosition(i - 1, TargetCamera.transform.position + DrawOffset * TargetCamera.transform.forward);
        //            //依据屏幕触摸位置确定线段位置
        //            line.SetPosition(i - 1, TargetCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, DrawOffset)));
        //        }
        //    }
        //}

        //lerpedColor = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 2));

        //GameObject.Find("Cube").GetComponent<MeshRenderer>().material.color = lerpedColor;
    }

    public void InitTouchControl()
    {
        var recognizer = new TKPanRecognizer();

        //recognizer.boundaryFrame = new TKRect(50f, 0, 100f, 100f);

        // when using in conjunction with a pinch or rotation recognizer setting the min touches to 2 smoothes movement greatly
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            recognizer.minimumNumberOfTouches = 2;

        recognizer.gestureRecognizedEvent += (r) =>
        {
            //Camera.main.transform.position -= new Vector3(recognizer.deltaTranslation.x, recognizer.deltaTranslation.y) / 100;
            //Debug.Log("pan recognizer fired: " + r);
            //Debug.Log(recognizer.touchLocation());
            //Debug.Log(recognizer.state);
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (IsDrawing == false)
                {
                    clone = (GameObject)Instantiate(LineObject);
                    clone.tag = "ARLines";
                    clone.GetComponent<LineRenderer>().material = CurrentMaterial;
                    clone.transform.parent = DrawSpace.transform;

                    line = clone.GetComponent<LineRenderer>();
                    //line.material.color = Color.white;
                    line.startWidth = LineWidth;
                    line.endWidth = LineWidth;
                    LineDividNumber = 0;
                    IsDrawing = true;
                }
                else
                {
                    LineDividNumber++;
                    line.positionCount = LineDividNumber;
                    Vector2 touchLocation = recognizer.touchLocation();
                    line.SetPosition(LineDividNumber - 1, TargetCamera.ScreenToWorldPoint(new Vector3(touchLocation.x, touchLocation.y, DrawOffset)));
                }
            }

        };

        // continuous gestures have a complete event so that we know when they are done recognizing
        recognizer.gestureCompleteEvent += r =>
        {
            Debug.Log("pan gesture complete");
            IsDrawing = false;
        };
        TouchKit.addGestureRecognizer(recognizer);

    }


    private bool IsPointerOverUIObject()
    {
        //判断是否点击的是UI，有效应对安卓没有反应的情况，true为UI
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void Clean()
    {
        int childCount = DrawSpace.transform.childCount;
        for (int j = 0; j < childCount; j++)
        {
            Destroy(DrawSpace.transform.GetChild(j).gameObject);
        }

    }

    public void ChangeTexure(string materialName)
    {
        CurrentMaterial = (Material)Instantiate(Resources.Load("Materials/" + materialName, typeof(Material)) as Material);
    }

    public void ChangeTexure(Material material)
    {
        Debug.Log("ChangeTexure"+material.color);
        
        SetMaterialRenderingMode(material, RenderingMode.Transparent);
        CurrentMaterial = (Material)Instantiate(material);
    }

    public void UseColorPickerValue()
    {
        Color32 color = ColorPicker.GetComponent<ColorPickerControl>().GetColorPcikerValue();
        ColorPickerMaterial.color = color;
        SetMaterialRenderingMode(ColorPickerMaterial,RenderingMode.Opaque);
        CurrentMaterial = (Material)Instantiate(ColorPickerMaterial);
    }

    public void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }

    public void ToggleColorPopup()
    {
        MaterialPopup.SetActive(false);
        ColorPopup.SetActive(!ColorPopup.activeSelf);
    }

    public void ToggleMaterialPopup()
    {
        ColorPopup.SetActive(false);
        MaterialPopup.SetActive(!MaterialPopup.activeSelf);
    }
}
