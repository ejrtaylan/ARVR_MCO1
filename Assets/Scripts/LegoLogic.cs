using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LegoLogic
{
    public static readonly Vector3 Grid = new Vector3(0.04f, 0.09f, 0.05f);
    public static int LayerMaskLego = LayerMask.GetMask("Lego");

    public static Vector3 SnapToGrid(Vector3 input) {
        return new Vector3(Mathf.Round(input.x / Grid.x) * Grid.x,
                            Mathf.Round(input.y / Grid.y) * Grid.y,
                            Mathf.Round(input.z / Grid.z) * Grid.z);
    }
}
