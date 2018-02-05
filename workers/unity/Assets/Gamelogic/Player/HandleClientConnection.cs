﻿using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class HandleClientConnection : MonoBehaviour
    {
        [Require]
        private ClientConnection.Writer ClientConnectionWriter;

        private Coroutine heartbeatCoroutine;

        private void OnEnable()
        {
            ClientConnectionWriter.CommandReceiver.OnDisconnectClient.RegisterResponse(OnDisconnectClient);
            ClientConnectionWriter.CommandReceiver.OnHeartbeat.RegisterResponse(OnHeartbeat);
            heartbeatCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.HeartbeatCheckIntervalSecs, CheckHeartbeat));
        }

        private void OnDisable()
        {
            ClientConnectionWriter.CommandReceiver.OnDisconnectClient.DeregisterResponse();
            ClientConnectionWriter.CommandReceiver.OnHeartbeat.DeregisterResponse();
            StopCoroutine(heartbeatCoroutine);
        }

        private ClientDisconnectResponse OnDisconnectClient(ClientDisconnectRequest request, ICommandCallerInfo callerinfo)
        {
            DeletePlayerEntity();
            return new ClientDisconnectResponse();
        }

        private HeartbeatResponse OnHeartbeat(HeartbeatRequest request, ICommandCallerInfo callerinfo)
        {
            SetHeartbeat(SimulationSettings.TotalHeartbeatsBeforeTimeout);
            return new HeartbeatResponse();
        }

        private void SetHeartbeat(uint beats)
        {
            var update = new ClientConnection.Update();
            update.SetTimeoutBeatsRemaining(beats);
            ClientConnectionWriter.Send(update);
        }

        private void CheckHeartbeat()
        {
            var heartbeatsRemainingBeforeTimeout = ClientConnectionWriter.Data.timeoutBeatsRemaining;
            if (heartbeatsRemainingBeforeTimeout == 0)
            {
                StopCoroutine(heartbeatCoroutine);
                DeletePlayerEntity();
                return;
            }
            SetHeartbeat(heartbeatsRemainingBeforeTimeout - 1);
        }

        private void DeletePlayerEntity()
        {
            SpatialOS.Commands.SendCommand(ClientConnectionWriter,
                                           PlayerCreation.Commands.DeletePlayer.Descriptor,
                                           new DeletePlayerRequest(ClientConnectionWriter.Data.clientId),
                                           ClientConnectionWriter.Data.playerCreatorId);
        }
    }
}
