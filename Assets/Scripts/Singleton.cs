﻿using UnityEngine;
 
/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T m_Instance;
 
	private static object m_Lock = new object();
 
	public static T Instance
	{
		get
		{
			if ( applicationIsQuitting )
			{
				Debug.LogWarning( "[Singleton] Instance '"+ typeof( T ) +
					"' already destroyed on application quit." +
					" Won't create again - returning null." );
				return null;
			}
 
			lock( m_Lock )
			{
				if ( m_Instance == null )
				{
					m_Instance = (T) FindObjectOfType( typeof( T ) );
 
					if ( FindObjectsOfType( typeof( T ) ).Length > 1 )
					{
						Debug.LogError( "[Singleton] Something went really wrong " +
							" - there should never be more than 1 singleton!" +
							" Reopening the scene might fix it." );
						return m_Instance;
					}
 
					if ( m_Instance == null )
					{
						GameObject singleton = new GameObject();
						m_Instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) "+ typeof( T ).ToString();
 
						DontDestroyOnLoad( singleton );
 
						Debug.Log( "[Singleton] An instance of " + typeof(T) + 
							" is needed in the scene, so '" + singleton +
							"' was created with DontDestroyOnLoad." );
					}
					else
					{
						Debug.Log( "[Singleton] Using instance already created: " +
							m_Instance.gameObject.name );
					}
				}
 
				return m_Instance;
			}
		}
	}
 
	private static bool applicationIsQuitting = false;
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public void OnDestroy ()
	{
		applicationIsQuitting = true;
	}
}