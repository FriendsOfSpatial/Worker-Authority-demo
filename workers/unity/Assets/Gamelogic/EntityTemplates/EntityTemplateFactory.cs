﻿using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine;
using Improbable.Unity.Entity;
using Improbable.Collections;
using Improbable.Demo;

namespace Assets.Gamelogic.EntityTemplates
{
    public class EntityTemplateFactory : MonoBehaviour
    {
        public static Entity CreatePlayerCreatorTemplate()
        {
            var playerCreatorEntityTemplate = EntityBuilder.Begin()
                .AddPositionComponent(Improbable.Coordinates.ZERO.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.PlayerCreatorPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new PlayerCreation.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new ClientEntityStore.Data(new Map<string, EntityId>()), CommonRequirementSets.PhysicsOnly)
                .Build();

            return playerCreatorEntityTemplate;
        }

        public static Entity CreatePlayerTemplate(string clientId, EntityId playerCreatorId)
        {
            var playerTemplate = EntityBuilder.Begin()
                .AddPositionComponent(Improbable.Coordinates.ZERO.ToUnityVector(), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddMetadataComponent(entityType: SimulationSettings.PlayerPrefabName)
                .SetPersistence(false)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new ClientAuthorityCheck.Data(), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new ClientConnection.Data(SimulationSettings.TotalHeartbeatsBeforeTimeout, clientId, playerCreatorId), CommonRequirementSets.PhysicsOnly)
                .Build();

            return playerTemplate;
        }

        public static Entity CreateCubeTemplate()
        {
            var cubeTemplate = EntityBuilder.Begin()
                .AddPositionComponent(Improbable.Coordinates.ZERO.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.CubePrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .Build();

            return cubeTemplate;
        }

        public static Entity CreateTileTemplate(Vector3 position)
        {
            var tileTemplate = EntityBuilder.Begin()
                .AddPositionComponent(position, CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.TilePrefabName)
                .SetPersistence(true)
                .SetReadAcl(Acl.MakeRequirementSet(CommonAttributeSets.Physics, CommonAttributeSets.Visual, Acl.MakeAttributeSet("turret_rotation")))
                .AddComponent(new Improbable.Demo.PositionColor.Data(0), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Improbable.Demo.CheckOutColor.Data(new List<uint>()), CommonRequirementSets.PhysicsOnly)
                .Build();

            return tileTemplate;
        }

        public static Entity CreateTankTemplate(Vector3 position)
        {
            var moverTemplate = EntityBuilder.Begin()
                .AddPositionComponent(position, CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.TankPrefabName)
                .SetPersistence(true)
                .SetReadAcl(Acl.MakeRequirementSet(CommonAttributeSets.Physics, CommonAttributeSets.Visual, Acl.MakeAttributeSet("turret_rotation")))
                .AddComponent(new Improbable.Demo.PositionColor.Data(0), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Improbable.Demo.CheckOutColor.Data(new List<uint>()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Improbable.Demo.TurretInfo.Data(Random.Range(0,360),0), Acl.MakeRequirementSet(Acl.MakeAttributeSet("turret_rotation")))
                .Build();

            return moverTemplate;
        }
    }
}
