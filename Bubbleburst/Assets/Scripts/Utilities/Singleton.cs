using UnityEngine;

namespace DanMacC.BubbleBurst.Utilities
{
    /// <summary>
    /// Singleton class that is used for the manager systems in the game
    /// This class also uses the DontDestroyOnLoad so it persists across scenes
    /// 
    /// NOTE: Singletons have drawbacks:
    /// - You cannot control when they are initialized which causes problems if you have a ton of them
    /// - It is effectively 'global' which is generally best to avoid
    /// - You have to ensure there is only ever one of them
    /// - If there are instances already existing in scenes when loading into them, it is difficult to control which one will get destroyed
    /// 
    /// However, for management systems like the GameManager and InteractionManager, they can be very helpful
    /// These kinds of systems are inherently designed to only have one instance, especially since they are persistent
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T m_Instance = default;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    // FindObjectOfType is slow so best to avoid doing frequently like in Update()
                    // However, okay to trigger once like this
                    m_Instance = FindObjectOfType<T>();
                    DontDestroyOnLoad(m_Instance.gameObject);

                    // Destroy any others that are NOT the instance so we only have one
                    foreach (var instance in FindObjectsOfType<T>())
                    {
                        if (instance != m_Instance)
                        {
                            Destroy(instance.gameObject);
                        }
                    }
                }

                return (T)m_Instance;
            }
        }

        private void Awake()
        {
            // Covers the case of a new Singleton instance being created past the initial call
            // Ex: loading a new scene that has the same singleton in it
            if (this != Instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
}