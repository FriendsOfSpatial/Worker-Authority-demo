using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Improbable.Unity;
using Improbable.Unity.Visualizer;

[WorkerType(WorkerPlatform.UnityWorker)]
public class ColorSetter : MonoBehaviour
{
	[Require]
	private Improbable.Demo.Color.Writer ColorWriter;

    [Require]
    private Improbable.Position.Writer PositionWriter;

    private void OnEnable()
	{
        var update = new Improbable.Demo.Color.Update();

        update.SetColorId(WorkerColor.Instace.ThisWorkerColorId);

        ColorWriter.Send(update);
	}
}
