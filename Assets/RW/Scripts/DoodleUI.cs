using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



public class DoodleUI : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public bool arMode;

    public DoodlePen pen;

    const float UIHeight = 60;      // this is the height of the DoodlePanel


    public GameObject scanSurfacePanel;
    public GameObject doodlePanel;

    [SerializeField] Text unitText;
    
    // Start is called before the first frame update
    void Start()
    {
        // Define the boundry of the doodle pen prevent conflict UI touches
        SetPenDrawingBound();

        if (arMode)
        {
            // 1
            SetDoodleUIVisible(false);
            SetCoachingUIVisible(true);

            // 2
            planeManager.planesChanged += PlanesChanged;
        }
        else
        {
            // 3
            SetDoodleUIVisible(true);
            SetCoachingUIVisible(false);
        }

    }


    void SetPenDrawingBound()
    {
        if(pen == null)
        {
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return;
        }

        pen.drawingBound = canvas.scaleFactor * UIHeight;
    }


    public void OnClearClicked()
    {
        if (pen != null)
        {
            pen.ClearLines();
        }
    }

    public void OnColorClicked(int index)
    {
        if (pen != null)
        {
            pen.ChangeColorIndex(index);
        }
    }

    public void SetCoachingUIVisible(bool flag)
    {
        scanSurfacePanel.SetActive(flag);
    }

    public void SetDoodleUIVisible(bool flag)
    {
        doodlePanel.SetActive(flag);
    }

    public void OnLineWidthChange(float value)
    {
        
        if (pen != null)
        {
            pen.ChangeLineWidth(value * 0.001f);
        }

        if(unitText != null)
        {
            unitText.text = string.Format("{0:0.0} cm", (value * 0.1f));
        }
    }

    private void PlanesChanged(ARPlanesChangedEventArgs planeEvent)
    {
        if (planeEvent.added.Count > 0 || planeEvent.updated.Count > 0)
        {
            SetDoodleUIVisible(true);
            SetCoachingUIVisible(false);

            planeManager.planesChanged -= PlanesChanged;
        }
    }

}
