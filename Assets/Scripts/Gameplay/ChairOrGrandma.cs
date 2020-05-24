using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
	[ExecuteInEditMode, SelectionBase]
	public class ChairOrGrandma : MonoBehaviour
	{
		[SerializeField] Transform chairPrefab;
		[SerializeField] Transform grandmaPrefab;

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
