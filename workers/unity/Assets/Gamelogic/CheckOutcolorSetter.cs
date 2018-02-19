using System.Collections;
using Improbable.Demo;
using Improbable.Entity.Component;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

using System.Linq;
using Improbable.Collections;
using Improbable.Worker;

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

    private static int PING_TIMEOUT_FRAMES = 10;

    private int counter = PING_TIMEOUT_FRAMES;
    private System.Collections.Generic.List<uint> respondingColors = new Improbable.Collections.List<uint>();

    private void OnEnable()
    {
        respondingColors.Clear(); 
        CheckOutColorWriter.CommandReceiver.OnSendColorId.RegisterResponse(RespondToSendColorId);
        CheckOutColorWriter.CommandReceiver.OnSendAndUpdateColorId.RegisterResponse(RespondToSendAndUpdateColorId);


        DoPing();
    }

    private void OnDisable()
    {
        CheckOutColorWriter.CommandReceiver.OnSendColorId.DeregisterResponse();
        CheckOutColorWriter.CommandReceiver.OnSendAndUpdateColorId.DeregisterResponse();
    }

    private void FixedUpdate()
    {
        --counter;

        if (counter <= 0)
        {
            respondingColors = respondingColors.Distinct().ToList();
            respondingColors.Sort();

            //CheckOutColorWriter.Data.colorsIds.Clear();
            //CheckOutColorWriter.Data.colorsIds.AddRange(respondingColors);

            var update = new Improbable.Demo.CheckOutColor.Update();
            update.colorsIds = CheckOutColorWriter.Data.colorsIds;
            update.colorsIds.Value.Clear();
            update.colorsIds.Value.AddRange(respondingColors);
            CheckOutColorWriter.Send(update);

            // Do not start pings when about to lose authority to prevent commands being lost
            if (CheckOutColorWriter.Authority == Authority.Authoritative)
            {
                DoPing();
            }
        }
    }

    private SendColorIdReturn RespondToSendColorId(ColorId idReceived, ICommandCallerInfo callerInfo)
    {
        respondingColors.Add(idReceived.colorId);
       

        return SendColorIdReturn.Create();
    }

    private SendAndUpdateColorIdReturn RespondToSendAndUpdateColorId(ColorId idReceived, ICommandCallerInfo callerInfo)
    {
        respondingColors.Add(idReceived.colorId);

        if (!CheckOutColorWriter.Data.colorsIds.Contains(idReceived.colorId))
        {
            CheckOutColorWriter.Data.colorsIds.Add(idReceived.colorId);
            CheckOutColorWriter.Data.colorsIds.Sort();

            var update = new Improbable.Demo.CheckOutColor.Update();
            update.colorsIds = CheckOutColorWriter.Data.colorsIds;
            CheckOutColorWriter.Send(update);
        }

        return SendAndUpdateColorIdReturn.Create();
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