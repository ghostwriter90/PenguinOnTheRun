
using UnityEngine;

namespace BasicTools.Utility
{
	/// <summary>
	/// Singleton pattern.
	/// </summary>
	public class SingletonMonoBehaviour<T> : MonoBehaviour	where T : Component
	{
		protected static T _instance = null;

		/// <summary>
		/// Singleton design pattern
		/// </summary>
		/// <value>The instance.</value>
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T> ();
					if (_instance == null)
					{
						GameObject obj = new GameObject ();
						_instance = obj.AddComponent<T> ();
					}
				}
				return _instance;
			}
		}

	    /// <summary>
	    /// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
	    /// </summary>
	    protected virtual void Awake ()
		{
			if (!Application.isPlaying)
			{
				return;
			}
            if(_instance != null)
            {
                Debug.LogErrorFormat("Assigning a singleton twice: {0} on {1} GameObject ", typeof(T).Name, gameObject.name);
                return;
            }
			_instance = this as T;			
		}
	}
}
