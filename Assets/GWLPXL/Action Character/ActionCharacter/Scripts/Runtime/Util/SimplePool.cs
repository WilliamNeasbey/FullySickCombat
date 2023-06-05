using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    ///
    /// Simple pooling for Unity.
    ///   Author: Martin "quill18" Glaude (quill18@quill18.com)
    ///   Latest Version: https://gist.github.com/quill18/5a7cfffae68892621267
    ///   License: CC0 (http://creativecommons.org/publicdomain/zero/1.0/)
    ///   UPDATES:
    /// 	2015-04-16: Changed Pool to use a Stack generic.
    /// 
    /// Usage:
    /// 
    ///   There's no need to do any special setup of any kind.
    /// 
    ///   Instead of calling Instantiate(), use this:
    ///       SimplePool.Spawn(somePrefab, somePosition, someRotation);
    /// 
    ///   Instead of destroying an object, use this:
    ///       SimplePool.Despawn(myGameObject);
    /// 
    ///   If desired, you can preload the pool with a number of instances:
    ///       SimplePool.Preload(somePrefab, 20);
    /// 
    /// Remember that Awake and Start will only ever be called on the first instantiation
    /// and that member variables won't be reset automatically.  You should reset your
    /// object yourself after calling Spawn().  (i.e. You'll have to do things like set
    /// the object's HPs to max, reset animation states, etc...)
    /// 
    /// 
    /// 




    /// <summary>
    /// modifications, created scene runner and ticker for auto return to the pool
	/// added lifetime and auto return - for particles, audio, stuff like that
    /// </summary>
    public static class SimplePool
	{
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR // Introduced in 2019.3. Also can cause problems in builds so only for editor.
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void Init()
		{
			sceneRunner = null;
			sceneParent = null;
			pools = new Dictionary<GameObject, Pool>();
		}
#endif
		// You can avoid resizing of the Stack's internal data by
		// setting this to a number equal to or greater to what you
		// expect most of your pool sizes to be.
		// Note, you can also use Preload() to set the initial size
		// of a pool -- this can be handy if only some of your pools
		// are going to be exceptionally large (for example, your bullets.)
		const int DEFAULT_POOL_SIZE = 3;
		const float DEFAULT_RETURN_TIME = 10;
		static SceneRunnerPool sceneRunner;
		static GameObject sceneParent;
		/// <summary>
		/// The Pool class represents the pool for a particular prefab.
		/// </summary>
		class Pool
		{
			// We append an id to the name of anything we instantiate.
			// This is purely cosmetic.
			int nextId = 1;

			// The structure containing our inactive objects.
			// Why a Stack and not a List? Because we'll never need to
			// pluck an object from the start or middle of the array.
			// We'll always just grab the last one, which eliminates
			// any need to shuffle the objects around in memory.
			Stack<GameObject> inactive;

			// The prefab that we are pooling
			GameObject prefab;
			GameObject parent;
			// Constructor
			public Pool(GameObject prefab, int initialQty)
			{
				this.prefab = prefab;
				parent = new GameObject(prefab.name + "_pool");
                if (sceneRunner == null)
                {
                    GameObject newrunner = new GameObject("Pool Scene Runner");
                    sceneRunner = newrunner.AddComponent<SceneRunnerPool>();
                }
                if (sceneParent == null)
                {
                    sceneParent = new GameObject("ALL POOLS");
                }
				parent.transform.SetParent(sceneParent.transform);
				// If Stack uses a linked list internally, then this
				// whole initialQty thing is a placebo that we could
				// strip out for more minimal code. But it can't *hurt*.
				inactive = new Stack<GameObject>(initialQty);
			}

			// Spawn an object from our pool
			public GameObject Spawn(Vector3 pos, Quaternion rot, bool autoreturn = true, float lifetime = -1)
			{
				GameObject obj;
				ReturnToPool returnp;
				if (inactive.Count == 0)
				{
					// We don't have an object in our pool, so we
					// instantiate a whole new object.
					obj = (GameObject)GameObject.Instantiate(prefab, pos, rot);
					obj.name = prefab.name + " (" + (nextId++) + ")";

					// Add a PoolMember component so we know what pool
					// we belong to.
					obj.AddComponent<PoolMember>().myPool = this;
					if (autoreturn)
                    {
						if (obj.GetComponent<ReturnToPool>() == null)
						{
							returnp = obj.AddComponent<ReturnToPool>();
							if (lifetime > 0)
                            {
								returnp.Lifetime = lifetime;
                            }
                            else
                            {
								returnp.Lifetime = SimplePool.DEFAULT_RETURN_TIME;
							}
							
						}
					}
					
				}
				else
				{
					// Grab the last object in the inactive array
					obj = inactive.Pop();

					if (obj == null)
					{
						// The inactive object we expected to find no longer exists.
						// The most likely causes are:
						//   - Someone calling Destroy() on our object
						//   - A scene change (which will destroy all our objects).
						//     NOTE: This could be prevented with a DontDestroyOnLoad
						//	   if you really don't want this.
						// No worries -- we'll just try the next one in our sequence.

						return Spawn(pos, rot);
					}
				}

				if (autoreturn)
                {
					returnp = obj.GetComponent<ReturnToPool>();
					if (lifetime > 0)
					{
						returnp.Lifetime = lifetime;
                    }
                    else
                    {
						returnp.Lifetime = SimplePool.DEFAULT_RETURN_TIME;
                    }
					returnp.StartTimer();
				}
	
				obj.transform.SetPositionAndRotation(pos, rot);
				obj.SetActive(true);
				return obj;

			}

			// Return an object to the inactive pool.
			public void Despawn(GameObject obj)
			{
				ReturnToPool returnp = obj.GetComponent<ReturnToPool>();
				if (returnp != null)
                {
					returnp.StopTimer();
				}
	
				obj.SetActive(false);
				if (parent != null)
				{
					obj.transform.SetParent(parent.transform);
				}
				// Since Stack doesn't have a Capacity member, we can't control
				// the growth factor if it does have to expand an internal array.
				// On the other hand, it might simply be using a linked list 
				// internally.  But then, why does it allow us to specify a size
				// in the constructor? Maybe it's a placebo? Stack is weird.
				inactive.Push(obj);
			}

		}


		/// <summary>
		/// Added to freshly instantiated objects, so we can link back
		/// to the correct pool on despawn.
		/// </summary>
		class PoolMember : MonoBehaviour
		{
			public Pool myPool;
		}

		// All of our pools
		static Dictionary<GameObject, Pool> pools;
		public static void IniScene()
        {
			
			pools.Clear();
			if (sceneParent == null)
            {
				sceneParent = new GameObject("All Pools");
			}
			if (sceneRunner == null)
            {
				GameObject newrunner = new GameObject("Pool Scene Runner");
				sceneRunner = newrunner.AddComponent<SceneRunnerPool>();
			}
	
		}
		/// <summary>
		/// Initialize our dictionary.
		/// </summary>
		static void Init(GameObject prefab = null, int qty = DEFAULT_POOL_SIZE)
		{
			if (pools == null)
			{
				pools = new Dictionary<GameObject, Pool>();
			}
			if (prefab != null && pools.ContainsKey(prefab) == false)
			{
				pools[prefab] = new Pool(prefab, qty);
			}
		}

		/// <summary>
		/// If you want to preload a few copies of an object at the start
		/// of a scene, you can use this. Really not needed unless you're
		/// going to go from zero instances to 100+ very quickly.
		/// Could technically be optimized more, but in practice the
		/// Spawn/Despawn sequence is going to be pretty darn quick and
		/// this avoids code duplication.
		/// </summary>
		static public void Preload(GameObject prefab, int qty = 1)
		{
			if (prefab == null)
            {
				Debug.Log("Trying to preload a null prefab");
				return;
            }
			Init(prefab, qty);

			// Make an array to grab the objects we're about to pre-spawn.
			GameObject[] obs = new GameObject[qty];
			for (int i = 0; i < qty; i++)
			{
				obs[i] = Spawn(prefab, Vector3.zero, Quaternion.identity);
			}

			// Now despawn them all.
			for (int i = 0; i < qty; i++)
			{
				Despawn(obs[i]);
			}
		}

		/// <summary>
		/// Spawns a copy of the specified prefab (instantiating one if required).
		/// NOTE: Remember that Awake() or Start() will only run on the very first
		/// spawn and that member variables won't get reset.  OnEnable will run
		/// after spawning -- but remember that toggling IsActive will also
		/// call that function.
		/// </summary>
		public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, bool autoReturn = true, float alivetime = -1)
		{
			
			Init(prefab);
			
			return pools[prefab].Spawn(pos, rot, autoReturn, alivetime);
		}

		/// <summary>
		/// Despawn the specified gameobject back into its pool.
		/// </summary>
		public static void Despawn(GameObject obj)
		{
			PoolMember pm = obj.GetComponent<PoolMember>();
			if (pm == null)
			{
				Debug.Log("Object '" + obj.name + "' wasn't spawned from a pool. Destroying it instead.");
				GameObject.Destroy(obj);
			}
			else
			{
				pm.myPool.Despawn(obj);
			}
		}

	}
}
