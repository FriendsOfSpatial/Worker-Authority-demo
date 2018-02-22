using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable;
using Improbable.Unity.Core;

public class WorkerColor : MonoBehaviour
{
	public static WorkerColor Instace;

	public uint ThisWorkerColorId;

    private static Color[] colors = {new Color(1, 97/255f, 135/255f), new Color(89/255f, 124/255f, 244/255f),
        new Color(59/255f, 226/255f, 160/255f), new Color(1, 204/255f, 0),
        new Color(215/255f, 65/255f, 5/255f), new Color(65/255f, 0, 120/255f)};
    
    private void Awake()
	{
		Instace = this;
	}


	private void Update()
	{
		if (ThisWorkerColorId == 0 && SpatialOS.IsConnected)
		{
			int colorInt = int.Parse(SpatialOS.WorkerId.Substring(SpatialOS.WorkerId.Length - 1));
			colorInt = (colorInt % 4) + 1;
			ThisWorkerColorId = (uint)colorInt;
		}
	}

	public static Color GetcolorFromId(uint id)
	{

		switch (id)
		{
			case 1: return colors[0];
            case 2: return colors[1];
            case 3: return colors[2];
            case 4: return colors[3];
		    case 5: return colors[4];
		    case 6: return colors[5];
            default: return Color.grey;

		}


	}
}
