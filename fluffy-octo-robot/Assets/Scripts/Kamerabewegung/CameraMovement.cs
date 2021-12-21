using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public HexGrid hexGrid;
    public float cameraRotationSpeed = 1f;
    public float cameraZoomSpeed = 1f;

    readonly float cameraDistanceMultiplier = 2.1f;
    float minCameraDistance;
    float cameraDistance;

    


    protected void Start()
    {
        minCameraDistance = 51.96152f * cameraDistanceMultiplier;
        cameraDistance = hexGrid.GetGridRadius() * cameraDistanceMultiplier;

        transform.LookAt(hexGrid.transform, Vector3.up);
        
    }

    protected void Update()
    {
        cameraDistance = hexGrid.GetGridRadius() * cameraDistanceMultiplier;
        cameraDistance = Mathf.Max(cameraDistance, minCameraDistance);


        if (Input.GetKey(KeyCode.A))
        {
            transform.RotateAround(Vector3.zero, Vector3.up, cameraRotationSpeed * Time.deltaTime * 100);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.RotateAround(Vector3.zero, Vector3.up, -cameraRotationSpeed * Time.deltaTime * 100);
        }

        if (Vector3.Distance(Vector3.zero, transform.position) != cameraDistance)
        {
            transform.LookAt(hexGrid.transform, Vector3.up);
            transform.position = Vector3.MoveTowards(transform.position, transform.position.normalized * cameraDistance, cameraZoomSpeed * Time.deltaTime * 100);
        }
        
    }
}
