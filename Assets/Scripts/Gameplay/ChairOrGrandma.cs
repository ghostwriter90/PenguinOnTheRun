﻿using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
	[ExecuteInEditMode, SelectionBase]
	public class ChairOrGrandma : MonoBehaviour
	{
#pragma warning disable 649
		[SerializeField] Transform chairPrefab;
		[SerializeField] Transform grandmaPrefab;
#pragma warning restore 649

		public void Initialize(bool isGrandma)
		{
			Transform prefab = isGrandma ? grandmaPrefab : chairPrefab;
			Transform instance = Instantiate(prefab);
			instance.SetParent(transform.parent);
			instance.position = transform.position;

			Destroy(gameObject);
		}
	}
}
