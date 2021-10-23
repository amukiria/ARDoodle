using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodleLine : MonoBehaviour
{
	// threshold to determine whether accept the point or not
	const float NewPointThreshold = 0.00001f;


	public delegate bool RaycastDelegate(out Vector3 hitPosition);


	public bool isActive = true;
	new public Camera camera = null;

	public Gradient lineGradient;
	public float lineWidth = 0.001f;

	public RaycastDelegate raycastDelegate = null;


    // internal data and reference 
	protected LineRenderer myRenderer;
	protected List<Vector3> myPoints = new List<Vector3>();



	private void Awake()
    {
        myRenderer = GetComponent<LineRenderer>();
    }


    // Start is called before the first frame update
    void Start()
    {
        if(raycastDelegate == null)
        {
			raycastDelegate = DefaultRaycastLogic;
		}

        if(lineGradient != null)
        {
			myRenderer.colorGradient = lineGradient;
        }
	}

    public void SetLineOrder(int order)
    {
		myRenderer.sortingOrder = order;
    }


	public void ChangeLineWidth(float newValue)
    {
		lineWidth = newValue;

		if (myRenderer == null)
        {
			return;
        }
		myRenderer.startWidth = lineWidth;
		myRenderer.endWidth = lineWidth;
    }

	bool DefaultRaycastLogic(out Vector3 hitPosition) {
		var point = Input.mousePosition;


		Ray ray = camera.ScreenPointToRay(point);
		// Debug.DrawRay(point, ray.direction, Color.red, 30); // for debug

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			hitPosition = hit.point;
			return true;
		} else
        {
			hitPosition = Vector3.zero;
			return false;
        }
	}

	private void Update()
	{
        if(isActive == false)
        {
			return;
        }

		if (Input.GetMouseButton(0))
		{
			AddPoint();
		}

		// preventing drawing when the mouse is up
		if (Input.GetMouseButtonUp(0))
		{
			if (myRenderer.positionCount == 1)
			{
				AddPoint(true);             // Add an extra point if the line is not complete
			}
			isActive = false;
        }

	}

	void AddPoint(bool forceAdd = false)        // ken: forceAdd for end occasion when create line
	{

        Vector3 hitPosition;
		bool hasHit = raycastDelegate(out hitPosition);
        if(hasHit)
        {
			AddPointToRender(hitPosition.x, hitPosition.y, hitPosition.z, forceAdd);
		}

	}

    float GetNewPointDelta(Vector3 newPoint)
    {
        if(myPoints.Count == 0)
        {
			return float.MaxValue;
        }

		return (myPoints[myPoints.Count - 1] - newPoint).sqrMagnitude;   
    }

	void AddPointToRender(float x, float y, float z, bool forceAdd)
	{
		Vector3 vec = new Vector3(x, y, z);

        if(forceAdd == false && GetNewPointDelta(vec) < NewPointThreshold)
		{
			return; // skip this new point
        }


		myPoints.Add(vec);


        // Refresh the renderer data 
		myRenderer.positionCount = myPoints.Count;
		for (int i = 0; i < myPoints.Count; i++)
		{
			myRenderer.SetPosition(i, myPoints[i]);
		}
	}
}
