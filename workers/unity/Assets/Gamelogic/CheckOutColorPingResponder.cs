using System.Collections;
using System.Collections.Generic;
using Improbable.Demo;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

[WorkerType(WorkerPlatform.UnityWorker)]
public class CheckOutColorPingResponder : MonoBehaviour
{
    [Require]
    private Improbable.Demo.CheckOutColor.Reader CheckOutColorReader;

    private void OnEnable()
    {
        CheckOutColorReader.PingTriggered.Add(OnPing);
    }

    private void OnDisable()
    {
        CheckOutColorReader.PingTriggered.Remove(OnPing);
    }

    private void OnPing(PingInfo info)
    {
        if (WorkerColor.Instace != null)
        {
            SpatialOS.WorkerCommands.SendCommand(CheckOutColor.Commands.SendColorId.Descriptor,
                new ColorId(WorkerColor.Instace.ThisWorkerColorId), gameObject.EntityId())
            //.OnSuccess(response => )
            .OnFailure(response => Debug.LogWarning(string.Format("SendColorId to Entity {0} returned was not success: {1}", gameObject.EntityId(), response.ErrorMessage))) ;
        }
    }
}
