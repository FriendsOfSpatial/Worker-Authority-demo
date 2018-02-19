using System.Collections;
using System.Collections.Generic;
using Improbable.Core;
using Improbable.Demo;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Export;
using Improbable.Unity.Visualizer;
using UnityEngine;

[WorkerType(WorkerPlatform.UnityClient)]
public class CheckOutColorVisualizer : MonoBehaviour
{
    [Require]
    private Improbable.Demo.CheckOutColor.Reader CheckOutColorReader;

    [Require]
    private Improbable.Demo.PositionColor.Reader PositionColorReader;

    [SerializeField]
    private List<MeshRenderer> positionRenderers;

    [SerializeField]
    private List<MeshRenderer> turretRenderers;

    [SerializeField]
    bool isTank;

    private void OnEnable()
    {
        UpdateColors(CheckOutColorReader.Data.colorsIds);

        CheckOutColorReader.ColorsIdsUpdated.Add(OnColorsIdsUpdated);
    }

    private void OnDisable()
    {
        CheckOutColorReader.ColorsIdsUpdated.Remove(OnColorsIdsUpdated);
    }

    protected virtual void OnColorsIdsUpdated(List<uint> newColorsIds)
    {
        UpdateColors(newColorsIds);
    }

    protected void UpdateColors(List<uint> colorIDs)
    {
        // Remove authoritative color only in tiles, for aesthetics reasons
        if (!isTank)
        {
            colorIDs.Remove(PositionColorReader.Data.colorId);
        }

        // Split list in colors from position workers and turret workers
        List<uint> positionColors = new List<uint>();
        List<uint> turretColors = new List<uint>();

        foreach (uint colorId in colorIDs)
        {
            if (colorId <= 4)
            {
                positionColors.Add(colorId);
            }
            else
            {
                turretColors.Add(colorId);
            }
        }


        // Set position workers colors depending on amount of different color
        if (positionColors.Count != 0)
        {
            int renderersPerColor = positionRenderers.Count / positionColors.Count;
            for (int i = 0; i < positionColors.Count; ++i)
            {
                for (int j = 0; j < renderersPerColor; ++j)
                {
                    positionRenderers[i * renderersPerColor + j].material.color =
                        WorkerColor.GetcolorFromId(positionColors[i]);
                }
            }
        }
        // If there are no other checking out, fill with base color
        else
        {
            for (int i = 0; i < positionRenderers.Count; ++i)
            {
                positionRenderers[i].material.color = WorkerColor.GetcolorFromId(PositionColorReader.Data.colorId);
            }
        }

        // Set turret workers colors
        if (turretColors.Count != 0)
        {
            int renderersPerColor = turretRenderers.Count / turretColors.Count;
            for (int i = 0; i < turretColors.Count; ++i)
            {
                for (int j = 0; j < renderersPerColor; ++j)
                {
                    turretRenderers[i * renderersPerColor + j].material.color =
                        WorkerColor.GetcolorFromId(turretColors[i]);
                }
            }
        }
        // If there are no other checking out, fill with base color
        else
        {
            for (int i = turretColors.Count; i < turretRenderers.Count; ++i)
            {
                turretRenderers[i].material.color = Color.white;
            }

            if (isTank)
            {
                Debug.LogWarning(string.Format("Tank Entity {0} is not checked out by any turret workers", gameObject.EntityId()));
            }
        }
    }
}
