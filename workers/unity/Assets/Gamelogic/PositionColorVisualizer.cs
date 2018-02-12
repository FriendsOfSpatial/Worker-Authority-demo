using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Improbable.Unity;
using Improbable.Unity.Visualizer;
using System;
using Improbable.Demo;

public class PositionColorVisualizer : MonoBehaviour
{
    [Require]
    private Improbable.Demo.PositionColor.Reader PositionColorReader;

    [SerializeField]
    private MeshRenderer posRenderer;

    private void OnEnable()
    {
        posRenderer.material.color = WorkerColor.GetcolorFromId(PositionColorReader.Data.colorId);

        PositionColorReader.ColorIdUpdated.Add(OnColorIdUpdated);
    }

    private void OnDisable()
    {
        PositionColorReader.ColorIdUpdated.Remove(OnColorIdUpdated);
    }

    private void OnColorIdUpdated(uint newColorId)
    {
        posRenderer.material.color = WorkerColor.GetcolorFromId(newColorId);
    }

}
