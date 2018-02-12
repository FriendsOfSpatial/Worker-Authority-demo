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
    private List<GameObject> poles;

    [SerializeField]
    private List<MeshRenderer> flagsRendereres;

    private void OnEnable()
    {

        for (int i=0; i<CheckOutColorReader.Data.colorsIds.Count; ++i)
        {
            flagsRendereres[i].material.color = WorkerColor.GetcolorFromId(CheckOutColorReader.Data.colorsIds[i]);
            poles[i].SetActive(true);
        }


        CheckOutColorReader.PingTriggered.Add(OnPing);
        CheckOutColorReader.ColorsIdsUpdated.Add(OnColorsIdsUpdated);
    }

    private void OnDisable()
    {
        CheckOutColorReader.PingTriggered.Remove(OnPing);
        CheckOutColorReader.ColorsIdsUpdated.Remove(OnColorsIdsUpdated);
    }

    private void OnPing(Nothing nothing)
    {
        if (WorkerColor.Instace != null)
        {
            SpatialOS.WorkerCommands.SendCommand(CheckOutColor.Commands.SendColorId.Descriptor,
                new ColorId(WorkerColor.Instace.ThisWorkerColorId), gameObject.EntityId());
            //.OnSuccess(response => OnCreatePlayerCommandSuccess(response, playerCreatorEntityId))
            //.OnFailure(response => OnCreatePlayerCommandFailure(response, playerCreatorEntityId));
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

        flagsRendereres[0].material.color = WorkerColor.GetcolorFromId(PositionColorReader.Data.colorId);
        flagsRendereres[0].gameObject.SetActive(true);
        poles[0].SetActive(true);


        // Color rest of flags
        for (int i = 0; i < colorIDs.Count; ++i)
        {
            flagsRendereres[i + 1].material.color = WorkerColor.GetcolorFromId(colorIDs[i]);
            flagsRendereres[i + 1].gameObject.SetActive(true);
            poles[i + 1].SetActive(true);
        }

        if (colorIDs.Count < flagsRendereres.Count)
        {
            for (int i = colorIDs.Count + 1; i < flagsRendereres.Count; ++i)
            {
                flagsRendereres[i].material.color = Color.gray;
                flagsRendereres[i].gameObject.SetActive(false);
                poles[i].SetActive(false);
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
