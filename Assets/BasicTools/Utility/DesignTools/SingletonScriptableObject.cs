using UnityEngine;
namespace BasicTools.Utility
{
    /// <summary>
    /// Singleton pattern.
    /// </summary>
    public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
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
                    _instance = CreateInstance<T>();
                }
                return _instance;
            }
        }        
    }
}