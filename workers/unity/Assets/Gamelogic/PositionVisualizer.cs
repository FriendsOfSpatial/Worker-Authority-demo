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

    [SerializeField]
    bool isTank;

    private void OnEnable()
    {

        transform.position = PositionReader.Data.coords.ToUnityVector();
        if (isTank)
        {
            transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(transform.position.z, transform.position.x) * Mathf.Rad2Deg, Vector3.up);
        }

        PositionReader.CoordsUpdated.Add(OnCoordsUpdated);
    }

    private void OnDisable()
    {
        PositionReader.CoordsUpdated.Remove(OnCoordsUpdated);
    }

    private void OnCoordsUpdated(Improbable.Coordinates newCoords)
    {
        transform.position = newCoords.ToUnityVector();

        if (isTank)
        {
            transform.rotation = Quaternion.AngleAxis(-Mathf.Atan2(transform.position.z, transform.position.x) * Mathf.Rad2Deg + 180, Vector3.up);
        }
    }
}
