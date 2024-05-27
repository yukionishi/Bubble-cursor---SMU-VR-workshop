using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class InteractionController : MonoBehaviour
{
    public GameObject[] bubbleCursorItems;
    public ActionBasedController XRController;
    public int maxLinecastDistance = 1000;

    private XRRayInteractor _XRControllerRayInteractor;

    void Start()
    {
        XRController.selectAction.action.performed += OnTriggerPress;
        _XRControllerRayInteractor = XRController.GetComponentInChildren<XRRayInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.isGameStarted) HighlightNearestCube();
    }

    private void OnDestroy()
    {
        XRController.activateAction.action.performed -= OnTriggerPress;
    }

    private void HighlightNearestCube()
    {
        // Get the raycast origin and direction so that we can perform a raycast
        // in the direction of the controller.
        var (rayOrigin, rayDirection) = getRayDetails();

        var nearestUnclickedCube = GetNearestUnclickedCubeToRayCast(rayOrigin, rayDirection);

        // Unhighlight the previously highlighted cube
        foreach (GameObject cube in bubbleCursorItems)
        {
            cube.GetComponent<CubeController>().UnHighlight();
        }

        // If there is a neatest unclicked cube, then highlight it.
        if (nearestUnclickedCube != null)
        {
            nearestUnclickedCube.GetComponent<CubeController>().Highlight();
        }
    }

    private void OnTriggerPress(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger Pressed!");

        var (rayOrigin, rayDirection) = getRayDetails();

        // Log this information to the console
        Debug.Log("Trigger Ray Origin: " + rayOrigin);
        Debug.Log("Trigger Ray Direction: " + rayDirection);

        var nearestUnclickedCube = GetNearestUnclickedCubeToRayCast(rayOrigin, rayDirection);

        // If there is a nearest unclicked cube, then click it.
        if (nearestUnclickedCube != null)
        {
            // Get the cube controller component
            CubeController cubeController = nearestUnclickedCube.GetComponent<CubeController>();
            // Click the cube
            cubeController.OnClick();
        }
    }

    /** Gets the XRRayInteractor's ray origin and direction. */
    private (Vector3, Vector3) getRayDetails()
    {
        // Get the public `RayOriginTransform` property from the XRRayInteractor
        // component
        Transform rayOriginTransform = _XRControllerRayInteractor.rayOriginTransform;

        // Get the starting position and rotation of the raycast
        Vector3 rayOrigin = rayOriginTransform.position;
        Quaternion rayRotation = rayOriginTransform.rotation;

        return (rayOrigin, rayRotation * Vector3.forward);
    }

    /** Calculated the distance between a a point and a Raycast line */
    private float DistanceBetweenPointAndLine(
      Vector3 point,
      Vector3 lineOrigin,
      Vector3 lineDirection
    )
    {
        Vector3 lineToPoint = point - lineOrigin;
        float t = Vector3.Dot(lineToPoint, lineDirection);

        // Clamp the parameter 't' to ensure the line doesn't exceed the specified length
        t = Mathf.Clamp(t, 0f, maxLinecastDistance);

        Vector3 nearestPointOnLine = lineOrigin + t * lineDirection;
        return Vector3.Distance(point, nearestPointOnLine);
    }

    /**Given a point and line (direction), return the nearest unclicked cube.*/
    private GameObject GetNearestUnclickedCubeToRayCast(Vector3 rayOrigin, Vector3 rayDirection)
    {
        // Stores the nearest unclicked cube if any.
        GameObject nearestUnclickedCube = null;

        // Perform a Raycast from the given origin and direction to see if there is
        // a direct raycast hit. We can assume that if there is a hit that is a 
        // bublecursoritem and it's unclicked, then that's the nearest unclicked cube.
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, maxLinecastDistance);
        // If there is a hit, make sure that it's a bubbleCursorItem and it's unclicked
        if (hits.Length == 1)
        {
            // Get the game object that was hit
            GameObject hitObject = hits[0].collider.gameObject;
            // Check if it's a a bubbleCursorItem
            if (System.Array.IndexOf(bubbleCursorItems, hitObject) != -1)
            {
                // If so, check if it's unclicked
                if (hitObject.GetComponent<CubeController>().isClicked == false)
                {
                    // If so, then store it as the nearest unclicked cube
                    nearestUnclickedCube = hitObject;
                }
            }
        }
        // If there is not Raycast hit, and the isBubbleCursorEnabled is true, then
        // we can attempt to find the nearest unclicked cube (not a direct hit).
        else if (GameController.isBubbleCursorEnabled)
        {
            // Store the minimum distance from the bubbleCursorItems to the Raycast line.
            float minDistance = maxLinecastDistance;

            // Get all the bubbleCursorItems that are unclicked
            List<GameObject> unclickedBubbleCursorItems = new List<GameObject>();
            foreach (GameObject cube in bubbleCursorItems)
            {
                if (cube.GetComponent<CubeController>().isClicked == false)
                {
                    unclickedBubbleCursorItems.Add(cube);
                }
            }

            // For each bubbleCursorItem that is unclicked
            foreach (GameObject cube in unclickedBubbleCursorItems)
            {
                // Store the distance from the cube to the Raycast line.
                float distance =
                  DistanceBetweenPointAndLine(cube.transform.position, rayOrigin, rayDirection);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestUnclickedCube = cube;
                }
            }
        }

        // Return the nearest unclicked cube if any.
        return nearestUnclickedCube;
    }

}
