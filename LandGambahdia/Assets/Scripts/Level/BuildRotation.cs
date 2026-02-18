using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildRotation : MonoBehaviour
{
    public void RotationBuilding(GameObject landTail)
    {
        BuildingControl bc = gameObject.GetComponent<BuildingControl>();
        LandTail landControl = landTail.GetComponent<LandTail>();
        if (bc != null && landControl != null)
        {
            int countRotation = (landControl.TailRot + 1) % 4;
            for (int i = 0; i < countRotation; i++)
            {
                bc.RotateTail();
            }            
        }
    }
}
