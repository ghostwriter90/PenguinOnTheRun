using System;
using UnityEngine;

namespace PenguinOnTheRun.Settings
{
	[Serializable]
	[CreateAssetMenu(fileName = "TrainCarSetting", menuName = "PenguinOnTheRun/TrainCarSetting", order = 1)]
	public class TrainCarSetting : ScriptableObject
	{
		public int ConductorCount;
		public int GrandmaCount;
		public int BoneCount;
		public int FishCount;
	}
}