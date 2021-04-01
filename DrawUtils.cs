using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUtils : MonoBehaviour
{

    
    /*  ######!-SET A TEXTMESH IN UNITY-!######
        Set some text mesh in unity with all the properties set
    */
    /// <summary>
    /// Take in the parent transform, local position vector (X, Y, Z), text to be used, fontsize for such text, color, and the sorting order.
    /// Once this is done we just set all the parameters to the components of the created textmesh.
    /// </summary>
    public static TextMesh DrawTextMesh(Transform parent, Vector3 localPosition, string text, int fontSize, Color color, int sortingOrder)
    {
        GameObject gameObject = new GameObject("Grid_Texdt", typeof(TextMesh));

        Transform transform = gameObject.transform;
        transform.localPosition = localPosition;

        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;

        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

        return textMesh;
    }

}
