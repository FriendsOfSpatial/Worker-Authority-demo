﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Improbable;
using Improbable.Collections;
using Improbable.Demo;
using Improbable.Worker;

namespace Managed
{
    class TurretProcessor
    {
        private uint thisWorkerColorId;
        private Connection connection;

        private static float rotationSpeed = 45f;

        private System.Collections.Generic.List<EntityId> authoritativeTurretRotations = new System.Collections.Generic.List<EntityId>();

        private Dictionary<EntityId, float> checkOutTurretRotations = new Dictionary<EntityId, float>();

        private System.Collections.Generic.List<EntityId> entitiesToConfirmAuthorityLoss = new System.Collections.Generic.List<EntityId>();

        public TurretProcessor(uint colorId, Connection connection)
        {
            thisWorkerColorId = colorId;
            this.connection = connection;
        }

        public void OnTurretInfoComponentAuthorityChanged(AuthorityChangeOp op)
        {
            if (op.Authority == Authority.Authoritative)
            {
                authoritativeTurretRotations.Add(op.EntityId);

                var update = new Improbable.Demo.TurretInfo.Update();
                update.colorId = thisWorkerColorId;
                connection.SendComponentUpdate(op.EntityId, update);

                connection.SendCommandRequest(op.EntityId, new CheckOutColor.Commands.SendAndUpdateColorId.Request(thisWorkerColorId), 300);
            }
            else if (op.Authority == Authority.AuthorityLossImminent)
            {
                entitiesToConfirmAuthorityLoss.Add(op.EntityId);
            }
            else if (op.Authority == Authority.NotAuthoritative)
            {
                authoritativeTurretRotations.Remove(op.EntityId);
            }
        }

        public void OnTurretInfoComponentAdded(AddComponentOp<Improbable.Demo.TurretInfo> op)
        {
            checkOutTurretRotations[op.EntityId] = op.Data.Get().Value.rotation;
        }

        public void OnTurretInfoComponentRemoved(RemoveComponentOp op)
        {
            if (checkOutTurretRotations.ContainsKey(op.EntityId))
            {
                checkOutTurretRotations.Remove(op.EntityId);
            }

            authoritativeTurretRotations.Remove(op.EntityId);
        }

        public void OnTurretInfoComponentUpdated(ComponentUpdateOp<Improbable.Demo.TurretInfo> op)
        {
            if (op.Update.Get().rotation.HasValue)
            {
                checkOutTurretRotations[op.EntityId] = op.Update.Get().rotation.Value;
            }
        }

        public void OnCheckOutComponentUpdated(ComponentUpdateOp<Improbable.Demo.CheckOutColor> op)
        {
            if (op.Update.Get().ping.Count > 0)
            {
                connection.SendCommandRequest(op.EntityId, new CheckOutColor.Commands.SendColorId.Request(thisWorkerColorId), 300);
            }
        }

        public void Tick(float timeInMiliseconds)
        {
            for (int i = entitiesToConfirmAuthorityLoss.Count - 1; i >= 0; --i)
            {
                connection.SendAuthorityLossImminentAcknowledgement<Improbable.Demo.TurretInfo>(entitiesToConfirmAuthorityLoss[i]);
                entitiesToConfirmAuthorityLoss.RemoveAt(i);
            }
                
            foreach (EntityId id in authoritativeTurretRotations)
            {
                var update = new Improbable.Demo.TurretInfo.Update();
                
                update.rotation = checkOutTurretRotations[id] + (rotationSpeed * (timeInMiliseconds / 1000f));
                connection.SendComponentUpdate(id, update);
            }
        }

        public void OnCommandSendColorIdResponse(CommandResponseOp<CheckOutColor.Commands.SendColorId> op)
        {
            if (op.StatusCode != StatusCode.Success)
            {
                connection.SendLogMessage(LogLevel.Warn, Startup.LoggerName, string.Format("SendColorId to Entity {0} return was not success: {1}", op.EntityId, op.Message));
            }
        }

        public void OnCommandSendAndUpdateColorIdResponse(CommandResponseOp<CheckOutColor.Commands.SendAndUpdateColorId> op)
        {
            if (op.StatusCode != StatusCode.Success && op.EntityId.Id > 406) // DEBUG
            {
                connection.SendLogMessage(LogLevel.Warn, Startup.LoggerName, string.Format("SendAndUpdateColorId to Entity {0} return was not success: {1}", op.EntityId, op.Message));
            }
        }
    }
}
