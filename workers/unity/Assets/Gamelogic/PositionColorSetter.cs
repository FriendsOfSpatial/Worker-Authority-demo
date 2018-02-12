using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Improbable.Unity;
using Improbable.Unity.Visualizer;

[WorkerType(WorkerPlatform.UnityWorker)]
public class PositionColorSetter : MonoBehaviour
{
	[Require]
	private Improbable.Demo.PositionColor.Writer PositionColorWriter;

    [Require]
    private Improbable.Position.Writer PositionWriter;

    private void OnEnable()
	{
        var update = new Improbable.Demo.PositionColor.Update();

        update.SetColorId(WorkerColor.Instace.ThisWorkerColorId);

	    PositionColorWriter.Send(update);
	}
}
