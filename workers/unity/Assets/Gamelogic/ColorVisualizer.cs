using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Improbable.Unity;
using Improbable.Unity.Visualizer;
using System;
using Improbable.Demo;

public class ColorVisualizer : MonoBehaviour
{
    [Require]
    private Improbable.Demo.Color.Reader ColorReader;

    [SerializeField]
    private MeshRenderer renderer;



    private void OnEnable()
    {
        renderer.material.color = WorkerColor.GetcolorFromId(ColorReader.Data.colorId);

        ColorReader.ColorIdUpdated.Add(OnColorIdUpdated);
    }

    private void OnDisable()
    {
        ColorReader.ColorIdUpdated.Remove(OnColorIdUpdated);
    }

    private void OnColorIdUpdated(uint newColorId)
    {
        renderer.material.color = WorkerColor.GetcolorFromId(newColorId);
    }

}
