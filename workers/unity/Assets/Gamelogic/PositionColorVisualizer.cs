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

    [SerializeField]
    private ParticleSystem particles;

    private void OnEnable()
    {
        posRenderer.material.color = WorkerColor.GetcolorFromId(PositionColorReader.Data.colorId);

        if (particles != null)
        {
            particles.startColor = WorkerColor.GetcolorFromId(PositionColorReader.Data.colorId);
        }

        PositionColorReader.ColorIdUpdated.Add(OnColorIdUpdated);
        PositionColorReader.LosingAuthTriggered.Add(OnLosingAuth);
    }

    private void OnDisable()
    {
        PositionColorReader.ColorIdUpdated.Remove(OnColorIdUpdated);
        PositionColorReader.LosingAuthTriggered.Remove(OnLosingAuth);
    }

    private void OnColorIdUpdated(uint newColorId)
    {
        posRenderer.material.color = WorkerColor.GetcolorFromId(newColorId);

        if (particles != null)
        {
            particles.startColor = WorkerColor.GetcolorFromId(newColorId);
        }
    }

    private void OnLosingAuth(LosingAuthInfo info)
    {
        if (particles != null)
        {
            particles.Play();
        }
    }

}
