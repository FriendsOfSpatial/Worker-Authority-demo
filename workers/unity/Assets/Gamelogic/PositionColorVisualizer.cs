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

    [SerializeField]
    private bool isTank;

    private void OnEnable()
    {
        SetColor();

        PositionColorReader.ColorIdUpdated.Add(OnColorIdUpdated);
        PositionColorReader.LosingAuthTriggered.Add(OnLosingAuth);

        if (isTank)
        {
            VisualizerSettings.Instance.TankPositionColorVisualizers.Add(this);
        }
        else
        {
            VisualizerSettings.Instance.TilePositionColorVisualizers.Add(this);
        }
    }

    private void OnDisable()
    {
        PositionColorReader.ColorIdUpdated.Remove(OnColorIdUpdated);
        PositionColorReader.LosingAuthTriggered.Remove(OnLosingAuth);

        if (isTank)
        {
            VisualizerSettings.Instance.TankPositionColorVisualizers.Remove(this);
        }
        else
        {
            VisualizerSettings.Instance.TilePositionColorVisualizers.Remove(this);
        }
    }

    private void OnColorIdUpdated(uint newColorId)
    {
        SetColor();
    }

    private void OnLosingAuth(LosingAuthInfo info)
    {
        if (particles != null && VisualizerSettings.Instance.UseTankPositionTransitionParticles)
        {
            particles.Play();
        }
    }

    public void SetColor()
    {
        foreach (MeshRenderer renderer in posRenderers)
        {
            if (isTank)
            {
                renderer.material.color = WorkerColor.GetcolorFromId(VisualizerSettings.Instance.UseTankPositionAuthorityColor? PositionColorReader.Data.colorId : 0);
            }
            else
            {
                renderer.material.color = WorkerColor.GetcolorFromId(VisualizerSettings.Instance.UseTilePositionAuthorityColor ? PositionColorReader.Data.colorId : 0);
            }
        }

        if (isTank && particles != null)
        {
            particles.startColor = WorkerColor.GetcolorFromId(PositionColorReader.Data.colorId);
        }
    }
}
