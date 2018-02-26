using System.Collections;
using System.Collections.Generic;
using Improbable.Demo;
using UnityEngine;

using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;

[WorkerType(WorkerPlatform.UnityWorker)]
public class PositionColorSetter : MonoBehaviour
{
	[Require]
	private Improbable.Demo.PositionColor.Writer PositionColorWriter;

    [Require]
    private Improbable.Position.Writer PositionWriter;

    private int framesBeforeAcceptAuthLoss = -1;

    private void OnEnable()
	{
        var update = new Improbable.Demo.PositionColor.Update();

        update.SetColorId(WorkerColor.Instace.ThisWorkerColorId);

	    PositionColorWriter.Send(update);

	    PositionWriter.AuthorityChanged.Add(OnAboutToLoseAuthority);
    }

    private void OnDisable()
    {
        PositionWriter.AuthorityChanged.Remove(OnAboutToLoseAuthority);
    }

    private void FixedUpdate()
    {
        if (framesBeforeAcceptAuthLoss >= 0)
        {
            --framesBeforeAcceptAuthLoss;

            if (framesBeforeAcceptAuthLoss < 0)
            {
                PositionWriter.SendAuthorityLossImminentAcknowledgement();
                PositionColorWriter.SendAuthorityLossImminentAcknowledgement();
                gameObject.GetComponent<CheckOutcolorSetter>().GetCheckoutWriter().SendAuthorityLossImminentAcknowledgement();
            }
        }
    }

    private void OnAboutToLoseAuthority(Authority info)
    {
        if (info == Authority.AuthorityLossImminent)
        {
            PositionColor.Update update = new PositionColor.Update();
            update.AddLosingAuth(LosingPositionAuthInfo.Create());
            PositionColorWriter.Send(update);

            framesBeforeAcceptAuthLoss = 5;
        }
    }
}
