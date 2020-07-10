using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Transform m_targetMarker;
    // Update is called once per frame
    void Update()
    {
        int button = 0;
        if (Input.GetMouseButtonDown(button))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitinfo;
            if (Physics.Raycast(ray, out hitinfo))
            {
                Vector3 targetPosition = hitinfo.point;
                m_targetMarker.position = new Vector3(targetPosition.x, 0.5f, targetPosition.z);
            }
        }
    }
}
