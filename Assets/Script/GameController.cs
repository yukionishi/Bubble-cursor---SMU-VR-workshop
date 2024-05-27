using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI toggleText;

    public InteractionController interactionController;
    // Get a reference to the button on the controller which will toggle
    // between bubble cursor.
    public InputActionReference toggleRef;

    /***** Private Variables *****/
    private static float startTime = 0;

    public static bool isGameStarted { get; private set; }
    public static bool isBubbleCursorEnabled { get; private set; }

    private void Awake()
    {
        // Subscribe to the input action's "started" event with the ToggleBubbleCursor method
        toggleRef.action.started += ToggleBubbleCursor;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the input action's "started" event with the ToggleBubbleCursor method
        toggleRef.action.started -= ToggleBubbleCursor;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted == false) return;

        // Check if all cubes are clicked
        bool isAllCubesClicked = true;
        foreach (GameObject cube in interactionController.bubbleCursorItems)
        {
            if (cube.GetComponent<CubeController>().isClicked == false)
            {
                isAllCubesClicked = false;
                break;
            }
        }

        // If all cubes are not clicked, then update the time
        if (isAllCubesClicked == false)
        {
            timerText.text = (Time.realtimeSinceStartup - startTime).ToString("F2");
        }
    }

    /***** Private Methods *****/
    private void ToggleBubbleCursor(InputAction.CallbackContext context)
    {
        // Only allow toggling before the game starts
        if (isGameStarted) return;

        isBubbleCursorEnabled = !isBubbleCursorEnabled;
        toggleText.text = isBubbleCursorEnabled ? "Bubble Cursor Enabled" : "Bubble Cursor Disabled";
    }

    public static void StartGame()
    {
        isGameStarted = true;
        startTime = Time.realtimeSinceStartup;
    }
}
