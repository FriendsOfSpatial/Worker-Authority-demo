using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Improbable.Unity;
using Improbable.Unity.Visualizer;
using System;
using Improbable.Demo;

public class PositionVisualizer : MonoBehaviour
{
    [Require]
    private Improbable.Position.Reader PositionReader;

    private void OnEnable()
    {

        transform.position = PositionReader.Data.coords.ToUnityVector();

        PositionReader.CoordsUpdated.Add(OnCoordsUpdated);
    }

    private void OnDisable()
    {
        PositionReader.CoordsUpdated.Remove(OnCoordsUpdated);
    }

    private void OnCoordsUpdated(Improbable.Coordinates newCoords)
    {
        transform.position = newCoords.ToUnityVector();
    }
}
