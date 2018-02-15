using System.Collections;
using System.Collections.Generic;
using Improbable.Core;
using Improbable.Demo;
using Improbable.Unity.Core;
using Improbable.Unity.Export;
using Improbable.Unity.Visualizer;
using UnityEngine;

public class CheckOutColorVisualizer : MonoBehaviour
{
    [Require]
    private Improbable.Demo.CheckOutColor.Reader CheckOutColorReader;

    [Require]
    private Improbable.Demo.PositionColor.Reader PositionColorReader;

    [SerializeField]
    private GameObject poleParent;

    [SerializeField]
    private List<GameObject> positionPoles;

    [SerializeField]
    private List<MeshRenderer> positionFlagsRendereres;

    [SerializeField]
    private List<GameObject> headPoles;

    [SerializeField]
    private List<MeshRenderer> headFlagsRendereres;

    private void OnEnable()
    {
        UpdateFlags(CheckOutColorReader.Data.colorsIds);

        CheckOutColorReader.PingTriggered.Add(OnPing);
        CheckOutColorReader.ColorsIdsUpdated.Add(OnColorsIdsUpdated);
    }

    private void OnDisable()
    {
        CheckOutColorReader.PingTriggered.Remove(OnPing);
        CheckOutColorReader.ColorsIdsUpdated.Remove(OnColorsIdsUpdated);
    }

    private void OnPing(PingInfo info)
    {
        if (WorkerColor.Instace != null)
        {
            SpatialOS.WorkerCommands.SendCommand(CheckOutColor.Commands.SendColorId.Descriptor,
                new ColorId(WorkerColor.Instace.ThisWorkerColorId), gameObject.EntityId());
            //.OnSuccess(response => )
            //.OnFailure(response => );
        }
    }

    private void OnColorsIdsUpdated(List<uint> newColorsIds)
    {
        UpdateFlags(newColorsIds);
    }

    private void UpdateFlags(List<uint> colorIDs)
    {
        // Set bottom flag always the color of position
        colorIDs.Remove(PositionColorReader.Data.colorId);

        positionFlagsRendereres[0].material.color = WorkerColor.GetcolorFromId(PositionColorReader.Data.colorId);
        positionFlagsRendereres[0].gameObject.SetActive(true);
        positionPoles[0].SetActive(true);

        // Split list in colors from position workers and head workers
        List<uint> positionColors = new List<uint>();
        List<uint> headColors = new List<uint>();

        foreach (uint colorId in colorIDs)
        {
            if (colorId <= 4)
            {
                positionColors.Add(colorId);
            }
            else
            {
                headColors.Add(colorId);
            }
        }


        // Color rest of position flags
        for (int i = 0; i < positionColors.Count; ++i)
        {
            positionFlagsRendereres[i + 1].material.color = WorkerColor.GetcolorFromId(positionColors[i]);
            positionFlagsRendereres[i + 1].gameObject.SetActive(true);
            positionPoles[i + 1].SetActive(true);
        }

        if (positionColors.Count < positionFlagsRendereres.Count)
        {
            for (int i = positionColors.Count + 1; i < positionFlagsRendereres.Count; ++i)
            {
                positionFlagsRendereres[i].material.color = Color.gray;
                positionFlagsRendereres[i].gameObject.SetActive(false);
                positionPoles[i].SetActive(false);
            }
        }

        // Color head flags
        for (int i = 0; i < headColors.Count; ++i)
        {
            headFlagsRendereres[i].material.color = WorkerColor.GetcolorFromId(headColors[i]);
            headFlagsRendereres[i].gameObject.SetActive(true);
            headPoles[i].SetActive(true);
        }

        if (headColors.Count < headFlagsRendereres.Count)
        {
            for (int i = headColors.Count; i < headFlagsRendereres.Count; ++i)
            {
                headFlagsRendereres[i].material.color = Color.gray;
                headFlagsRendereres[i].gameObject.SetActive(false);
                headPoles[i].SetActive(false);
            }
        }


        // Turn off flagpole if no other workers have the entity checked out
        if (colorIDs.Count < 1)
        {
            poleParent.SetActive(false);
        }
        else
        {
            poleParent.SetActive(true);
        }
    }
}
