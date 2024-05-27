using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [Header("Cube Colors")]
    public Color defaultColor   = Color.white;
    public Color highlightColor = Color.yellow;
    public Color clickedColor   = Color.black;

    [Header("Cube Configuration")]
    public bool isStartingCube = false;

    public bool isClicked { get; private set; }

    private bool _isHighlighted = false;

    void Start()
    {
        if (isStartingCube) this.gameObject.GetComponent<Renderer>().material.color = highlightColor;
        else this.gameObject.GetComponent<Renderer>().material.color = defaultColor;
    }

    public void OnClick()
    {
        if (isClicked) return;

        if (!GameController.isGameStarted)
        {
            if (!isStartingCube) return;

            GameController.StartGame();
            isClicked = true;
            _isHighlighted = false;
            this.gameObject.GetComponent<Renderer>().material.color = clickedColor;

        }
        else
        {
            isClicked = true;
            _isHighlighted = false;
            this.gameObject.GetComponent<Renderer>().material.color = clickedColor;
        }
    }

    public void Highlight()
    {
        if(isClicked || _isHighlighted) return;
        this.gameObject.GetComponent<Renderer>().material.color = highlightColor;
        _isHighlighted = true;
    }

    public void UnHighlight()
    {
        if (!_isHighlighted) return;
        this.gameObject.GetComponent<Renderer>().material.color = defaultColor;
        _isHighlighted = false;
    }
}
