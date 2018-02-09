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

            float startX = -105, startZ = -105;
            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateTileTemplate(new Vector3(startX + i*10f, 0, startZ + j*10f)));
                }
            }

		    for (int i = 0; i < 10; ++i)
		    {
		        snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateMoverTemplate(new Vector3(Random.Range(-60f, -2f), 0, Random.Range(-60f, -2f))));
		        snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateMoverTemplate(new Vector3(Random.Range(-60f, -2f), 0, Random.Range(2f, 60f))));
		        snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateMoverTemplate(new Vector3(Random.Range(2f, 60f), 0, Random.Range(-60f, -2f))));
		        snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateMoverTemplate(new Vector3(Random.Range(2f, 60f), 0, Random.Range(2f, 60f))));

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
