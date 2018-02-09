using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Improbable.Unity;
using Improbable.Unity.Visualizer;

//[WorkerType(WorkerPlatform.UnityWorker)]
public class Mover : MonoBehaviour
{
    [Require]
    private Improbable.Position.Writer PositionWriter;

    private void FixedUpdate()
    {
        Vector3 newPosition = Quaternion.Euler(0, 5 * Time.fixedDeltaTime, 0) * PositionWriter.Data.coords.ToUnityVector();

        var update = new Improbable.Position.Update();
        update.SetCoords(new Improbable.Coordinates(newPosition.x, newPosition.y, newPosition.z));
        PositionWriter.Send(update);

        /*
        Vector3 movementDirection;
        if (PositionWriter.Data.coords.X <= 32.5f)
        {
            if (PositionWriter.Data.coords.Z <= 32.5f)
            {
                movementDirection = new Vector3(0, 0, 1);
            }
            else
            {
                movementDirection = new Vector3(1, 0, 0);
            }
        }
        else
        {
            if (PositionWriter.Data.coords.Z > -32.5f)
            {
                movementDirection = new Vector3(0, 0, -1);
            }
            else
            {
                movementDirection = new Vector3(0, 0, -1);
            }
        }

        var update = new Improbable.Position.Update();
        update.SetCoords(PositionWriter.Data.coords + movementDirection * Time.fixedDeltaTime);

        PositionWriter.Send(update);
        */
    }
}
