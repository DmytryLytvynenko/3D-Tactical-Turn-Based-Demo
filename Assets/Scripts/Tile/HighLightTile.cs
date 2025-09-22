using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightTile : MonoBehaviour
{
    public void HighLight(Color highlightColor)
    {
        GetComponent<MeshRenderer>().material.color = highlightColor;
    }
    public void ClearHighLight()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
    }
}
