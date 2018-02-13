using System.Collections;
using System.Collections.Generic;
using Improbable.Demo;
using Improbable.Entity.Component;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

using System.Linq;

[WorkerType(WorkerPlatform.UnityWorker)]
public class CheckOutcolorSetter : MonoBehaviour
{
    // This is needed to ensure that all 3 components are authoritative under the same worker, 
    //so when we delay the authority handover of the other 2, we will need to get this one and handover this one at the same time
    public Improbable.Demo.CheckOutColor.Writer GetCheckoutWriter()
    {
        return CheckOutColorWriter;
    }

    [Require]
    private Improbable.Demo.CheckOutColor.Writer CheckOutColorWriter;

    [Require]
    private Improbable.Position.Writer PositionWriter;

    private static int PING_TIMEOUT_FRAMES = 5;

    private int counter = PING_TIMEOUT_FRAMES;
    private List<uint> respondingColors = new Improbable.Collections.List<uint>();

    private void OnEnable()
    {
        respondingColors.Clear(); 
        CheckOutColorWriter.CommandReceiver.OnSendColorId.RegisterResponse(RespondToSendColorId);

        DoPing();
    }

    private void OnDisable()
    {
        CheckOutColorWriter.CommandReceiver.OnSendColorId.DeregisterResponse();
    }

    private void FixedUpdate()
    {
        --counter;

        if (counter <= 0)
        {
            CheckOutColorWriter.Data.colorsIds.Clear();
            CheckOutColorWriter.Data.colorsIds.AddRange(respondingColors);

            var update = new Improbable.Demo.CheckOutColor.Update();
            update.colorsIds = CheckOutColorWriter.Data.colorsIds;
            CheckOutColorWriter.Send(update);

            DoPing();
        }
    }

    private SendColorIdReturn RespondToSendColorId(ColorId idReceived, ICommandCallerInfo callerInfo)
    {
        respondingColors.Add(idReceived.colorId);
        respondingColors = respondingColors.Distinct().ToList();
        respondingColors.Sort();

        return SendColorIdReturn.Create();
    }

    private void DoPing()
    {
        counter = PING_TIMEOUT_FRAMES;

        respondingColors.Clear();

        var update = new Improbable.Demo.CheckOutColor.Update();
        update.AddPing(PingInfo.Create());
        CheckOutColorWriter.Send(update);
    }
}