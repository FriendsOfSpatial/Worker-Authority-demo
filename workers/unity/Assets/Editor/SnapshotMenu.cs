using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Worker;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
	public class SnapshotMenu : MonoBehaviour
	{
		[MenuItem("Improbable/Snapshots/Generate Default Snapshot")]
		private static void GenerateDefaultSnapshot()
		{
			var snapshotEntities = new Dictionary<EntityId, Entity>();
			var currentEntityId = 1;

			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreatePlayerCreatorTemplate());

            float startX = -142.5f, startZ = -142.5f;
            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateTileTemplate(new Vector3(startX + i*15f, 0, startZ + j*15f)));
                }
            }

            // Inner circle
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateTankTemplate(new Vector3(-5, 0, -5)));


		    for (int i = 1; i < 10; ++i)
		    {
		        Vector3 posVector = Vector3.forward * 15 * i;
                snapshotEntities.Add(new EntityId(currentEntityId++),EntityTemplateFactory.CreateTankTemplate(Quaternion.Euler(0, Random.Range(1, 90), 0) * posVector));
		        snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateTankTemplate(Quaternion.Euler(0, Random.Range(91, 180), 0) * posVector));
		        snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateTankTemplate(Quaternion.Euler(0, Random.Range(181, 270), 0) * posVector));
		        snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateTankTemplate(Quaternion.Euler(0, Random.Range(271, 360), 0) * posVector));
            }

            SaveSnapshot(snapshotEntities);
		}

		private static void SaveSnapshot(IDictionary<EntityId, Entity> snapshotEntities)
		{
			File.Delete(SimulationSettings.DefaultSnapshotPath);
			using (SnapshotOutputStream stream = new SnapshotOutputStream(SimulationSettings.DefaultSnapshotPath))
			{
				foreach (var kvp in snapshotEntities)
				{
					var error = stream.WriteEntity(kvp.Key, kvp.Value);
					if (error.HasValue)
					{
						Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", error.Value);
						return;
					}
				}
			}

			Debug.LogFormat("Successfully generated initial world snapshot at {0}", SimulationSettings.DefaultSnapshotPath);
		}
	}
}
