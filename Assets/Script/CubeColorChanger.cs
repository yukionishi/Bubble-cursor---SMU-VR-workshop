using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CubeColorChanger : MonoBehaviour
{
    public InputActionReference colorRef = null; // Reference to the input action
    public Color cubeColor; // Color to change the cube to
    private MeshRenderer meshRenderer = null; // Reference to the cube's MeshRenderer
    private bool _isColor = false;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>(); // Get the MeshRenderer component of the cube
        colorRef.action.started += ChangeColor; // Subscribe to the input action's "started" event with the ChangeColor method
    }

    private void OnDestroy()
    {
        colorRef.action.started -= ChangeColor; // Unsubscribe from the input action's "started" event when the script is destroyed
    }

    public void ChangeColor(InputAction.CallbackContext context)
    {
        if (_isColor) meshRenderer.material.color = Color.white;
        else meshRenderer.material.color = cubeColor; // Change the cube's material color to the specified cubeColor

        _isColor = !_isColor;
    }
}