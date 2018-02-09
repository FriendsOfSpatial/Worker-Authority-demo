using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable;
using Improbable.Unity.Core;

public class WorkerColor : MonoBehaviour
{
	public static WorkerColor Instace;

	public uint ThisWorkerColorId;

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
			case 1: return Color.red;
			case 2: return Color.blue;
			case 3: return Color.green;
			case 4: return Color.yellow;
			default: return Color.gray;

		}


	}
}
