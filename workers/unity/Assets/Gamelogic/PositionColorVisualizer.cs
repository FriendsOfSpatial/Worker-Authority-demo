using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Improbable.Unity;
using Improbable.Unity.Visualizer;
using System;
using Improbable.Demo;

[WorkerType(WorkerPlatform.UnityClient)]
public class PositionColorVisualizer : MonoBehaviour
{
    [Require]
    private Improbable.Demo.PositionColor.Reader PositionColorReader;

    [SerializeField]
    private List<MeshRenderer> posRenderers;

    [SerializeField]
    private ParticleSystem particles;

    private void OnEnable()
    {
        foreach (MeshRenderer renderer in posRenderers)
        {
            renderer.material.color = WorkerColor.GetcolorFromId(PositionColorReader.Data.colorId);
        }
        
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
        foreach (Renderer renderer in posRenderers)
        {
            renderer.material.color = WorkerColor.GetcolorFromId(PositionColorReader.Data.colorId);
        }

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
