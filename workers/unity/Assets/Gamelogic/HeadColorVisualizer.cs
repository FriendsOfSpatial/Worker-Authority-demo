using System.Collections;
using System.Collections.Generic;
using Improbable.Demo;
using Improbable.Unity.Visualizer;
using UnityEngine;

public class HeadColorVisualizer : MonoBehaviour {

    [Require]
    private Improbable.Demo.HeadColor.Reader HeadColorReader;

    [SerializeField]
    private MeshRenderer headRenderer;

    private void OnEnable()
    {
        headRenderer.material.color = WorkerColor.GetcolorFromId(HeadColorReader.Data.colorId);

        HeadColorReader.ColorIdUpdated.Add(OnColorIdUpdated);
    }

    private void OnDisable()
    {
        HeadColorReader.ColorIdUpdated.Remove(OnColorIdUpdated);
    }

    private void OnColorIdUpdated(uint newColorId)
    {
        headRenderer.material.color = WorkerColor.GetcolorFromId(newColorId);
    }
}
