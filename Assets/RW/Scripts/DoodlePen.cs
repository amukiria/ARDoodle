using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



public class DoodlePen : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public bool arMode = false;

    public const float UIHeight = 60;        // the screen height of the UI 

    new public Camera camera;
    

    public DoodleLine linePrefab = null;
    public Gradient[] colorTheme = null;

    public float drawingBound = 0;      // The lower bound where touch is valid

    private int mySelectedColorIndex = 0;
    private float myLineWidth = 0.005f;
    private int mySortingOrder = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            // skip the touches fall on the UI
            if (Input.mousePosition.y < drawingBound)
            {
                return;
            }

            SpawnNewLine();
        }
    }

    Gradient getLineGradient()
    {
        return colorTheme[mySelectedColorIndex];
    }

    public void ChangeColorIndex(int index)
    {
        mySelectedColorIndex = index;
    }

    public void ChangeLineWidth(float lineWidth)
    {
        myLineWidth = lineWidth;
    }

    public void SpawnNewLine()
    {
        if(linePrefab == null)
        {
            return;
        }

        var newLine = Instantiate(linePrefab);
        SetupRaycastLogic(newLine);

        newLine.lineGradient = getLineGradient();
        newLine.SetLineOrder(mySortingOrder);
        newLine.ChangeLineWidth(myLineWidth);

        Transform t = newLine.transform;
        t.parent = transform;

        mySortingOrder++;
    }

    public void ClearLines()
    {
        DoodleLine[] lines = GetComponentsInChildren<DoodleLine>();
        // Debug.Log("ClearLines: lines.count=" + lines.Length);
        foreach(DoodleLine line in lines)
        {
           //  Debug.Log("line: " + line);
            Destroy(line.gameObject);
        }
    }

    void SetupRaycastLogic(DoodleLine doodleLine)
    {
        if (arMode)
        {
            doodleLine.raycastDelegate = GetArRaycastLogic;
        }
        else
        {
            doodleLine.raycastDelegate = GetNonArRaycastLogic;
        }
        doodleLine.gameObject.SetActive(true);

    }

    bool GetNonArRaycastLogic(out Vector3 hitPosition)
    {
        var point = Input.mousePosition;


        Ray ray = camera.ScreenPointToRay(point);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            hitPosition = hit.point;
            return true;
        }
        else
        {
            hitPosition = Vector3.zero;
            return false;
        }
    }

    bool GetArRaycastLogic(out Vector3 hitPosition)
    {

        // 1
        var hits = new List<ARRaycastHit>();

        // 2
        bool hasHit = raycastManager.Raycast(Input.mousePosition, hits, TrackableType.PlaneWithinInfinity);

        // 3
        if (hasHit == false || hits.Count == 0)
        {
            hitPosition = Vector3.zero;
            return false;
        }
        else
        {
            hitPosition = hits[0].pose.position;
            return true;
        }
    }


}
