using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class Objects : MonoSingleton<Objects> {

	public delegate int PoolGrowthDelegate(IObjectPool objectPool);

	public enum PoolGrowthType {
		None,       //Growth is not allowed, returns null object
		Append,     //Grow by one
		Double,     //Double current pool size
		Half,       //Grow by half the current pool size
	}

	public interface IObjectPool {
		int ObjectCount {
			get;
		}
		int FreeCount {
			get;
		}
		int SpawnedCount {
			get;
		}

		IList<CastType> GetPooledObjects<CastType>() where CastType : IPooledObject;
		IList<CastType> GetPooledObjectsFree<CastType>() where CastType : IPooledObject;
		IList<CastType> GetPooledObjectsSpawned<CastType>() where CastType : IPooledObject;

		IPooledObject Spawn();
		void Recycle(IPooledObject instance);

		void DestroyInstances();
		void RecycleInstances();
		void IncreasePoolSize(int count);
	}

	private class ObjectPool<T> : IObjectPool where T : class, IPooledObject {

		private IPooledObject prefab = null;
		private PoolGrowthType growthType = PoolGrowthType.None;
		private PoolGrowthDelegate growthDelegate = null;

		private List<T> objects = new List<T>();
		private List<T> free = new List<T>();
		private List<T> spawned = new List<T>();

		////////////////////////////////////////////////////////////////////////////////

		public int ObjectCount {
			get {
				if (objects == null) {
					return 0;
				}
				return objects.Count;
			}
		}

		public int FreeCount {
			get {
				if (free == null) {
					return 0;
				}
				return free.Count;
			}
		}

		public int SpawnedCount {
			get {
				if (spawned == null) {
					return 0;
				}
				return spawned.Count;
			}
		}

		////////////////////////////////////////////////////////////////////////////////

		public ObjectPool(IPooledObject _prefab, PoolGrowthType _growthType, PoolGrowthDelegate _growthDelegate) {
			prefab = _prefab;
			growthType = _growthType;
			growthDelegate = _growthDelegate;
		}

		public IPooledObject Spawn() {
			if (free.Count <= 0) {
				int growthAmount = CalculateGrowthAmount();
				if (growthAmount > 0) {
					IncreasePoolSize(growthAmount);
				}
			}

			if (free.Count <= 0) {
				Debug.LogWarning("Failed to spawn instance, pool is empty.");
				return null;
			}

			T frontInstance = free[0];
			if (frontInstance == null) {
				Debug.LogError("Error occured, pool contained null instance");
				return null;
			}

			OnInstanceSpawned(frontInstance);
			return frontInstance;
		}

		public void Recycle(IPooledObject instance) {
			if (instance == null) {
				return;
			}

			T typedInstance = instance as T;
			if (typedInstance == null) {
				return;
			}

			if (objects.Contains(typedInstance) == false) {
				Debug.LogError("Failed to find instance in pool");
				return;
			}

			if (spawned.Contains(typedInstance) == false) {
				return;
			}

			OnInstanceRecycled(instance);
		}

		public void DestroyInstances() {
			if (objects == null || objects.Count <= 0) {
				return;
			}

			foreach (T instance in objects) {
				if (instance == null) {
					continue;
				}

				GameObject instanceObject = instance.gameObject;
				if (instanceObject == null) {
					continue;
				}

				UnityEngine.Object.Destroy(instanceObject);
			}

			objects.Clear();
			free.Clear();
			spawned.Clear();
		}

		public void RecycleInstances() {
			if (objects == null || objects.Count <= 0) {
				return;
			}

			foreach (T instance in objects) {
				if (instance == null) {
					continue;
				}

				if (spawned.Contains(instance) == false) {
					continue;
				}

				GameObject instanceObject = instance.gameObject;
				if (instanceObject != null) {
					instanceObject.transform.SetParent(Objects.Instance.transform, false);
					instanceObject.SetActive(false);
				}

				OnInstanceRecycled(instance);
			}
		}

		public void IncreasePoolSize(int count) {
			if (prefab == null) {
				Debug.LogError("Cannot extend pool size, prefab is null");
				return;
			}

			GameObject prefabObject = prefab.gameObject;
			if (prefabObject == null) {
				Debug.LogError("Cannot extend pool size, prefab gameObject is null");
				return;
			}

			for (uint i = 0; i < count; ++i) {
				GameObject instance = UnityEngine.Object.Instantiate(prefabObject);
				if (instance == null) {
					return;
				}

				instance.transform.SetParent(Objects.Instance.transform, false);
				instance.SetActive(false);

				T typedComponent = instance.GetComponent<T>();
				if (typedComponent == null) {
					Debug.LogErrorFormat("{0} cannot be cast to {1}", prefab.name, typeof(T).ToString());
					return;
				}

				typedComponent.prefab = prefab;
				OnInstanceAdded(typedComponent);
			}
		}

		public IList<CastType> GetPooledObjects<CastType>() where CastType : IPooledObject {
			List<CastType> typedInstances = objects.Cast<CastType>().ToList();
			if (typedInstances == null) {
				return null;
			}
			return typedInstances.AsReadOnly();
		}

		public IList<CastType> GetPooledObjectsFree<CastType>() where CastType : IPooledObject {
			List<CastType> typedInstances = free.Cast<CastType>().ToList();
			if (typedInstances == null) {
				return null;
			}
			return typedInstances.AsReadOnly();
		}

		public IList<CastType> GetPooledObjectsSpawned<CastType>() where CastType : IPooledObject {
			List<CastType> typedInstances = spawned.Cast<CastType>().ToList();
			if (typedInstances == null) {
				return null;
			}
			return typedInstances.AsReadOnly();
		}

		////////////////////////////////////////////////////////////////////////////////

		private int CalculateGrowthAmount() {
			if (growthDelegate != null) {
				return growthDelegate.Invoke(this);
			}

			switch (growthType) {
			case PoolGrowthType.Append: {
					return 1;
				}
			case PoolGrowthType.Double: {
					return ObjectCount * 2;
				}
			case PoolGrowthType.Half: {
					return ObjectCount + (ObjectCount / 2);
				}
			}

			return 0;
		}

		private void OnInstanceAdded(IPooledObject instance) {
			if (instance == null) {
				return;
			}

			T typedInstance = instance as T;
			if (typedInstance == null) {
				return;
			}

			if (objects.Contains(typedInstance) == true) {
				return;
			}

			instance.OnEnterPool();

			objects.Add(typedInstance);
			free.Add(typedInstance);
		}

		private void OnInstanceSpawned(IPooledObject instance) {
			if (instance == null) {
				return;
			}

			T typedInstance = instance as T;
			if (typedInstance == null) {
				return;
			}

			free.Remove(typedInstance);
			spawned.Add(typedInstance);
		}

		private void OnInstanceRecycled(IPooledObject instance) {
			if (instance == null) {
				return;
			}

			T typedInstance = instance as T;
			if (typedInstance == null) {
				return;
			}

			instance.OnEnterPool();

			spawned.Remove(typedInstance);
			free.Add(typedInstance);
		}
	}

	////////////////////////////////////////////////////////////////////////////////

	private Dictionary<IPooledObject, IObjectPool> objectPools = new Dictionary<IPooledObject, IObjectPool>();

#if UNITY_EDITOR
	private List<UnityEngine.Object> destructionList = new List<Object>();
	private List<UnityEngine.Object> standaloneInstances = new List<UnityEngine.Object>();
#endif //UNITY_EDITOR

	////////////////////////////////////////////////////////////////////////////////

	private void Update() {
		CleanDestructionList();
	}

	private void CleanDestructionList() {
#if UNITY_EDITOR
		if (destructionList == null || destructionList.Count <= 0) {
			return;
		}

		int randIdx = UnityEngine.Random.Range(0, destructionList.Count);
		if (destructionList[randIdx] == null) {
			destructionList.RemoveAt(randIdx);
		}
#endif
	}

	private IObjectPool GetObjectPool(IPooledObject prefab) {
		if (prefab == null) {
			return null;
		}

		if (objectPools.ContainsKey(prefab) == false) {
			return null;
		}

		return objectPools[prefab];
	}

	////////////////////////////////////////////////////////////////////////////////
	// Spawn Standalone

	public static GameObject CreateSingle(GameObject prefab) {
		return CreateSingle(prefab, prefab.transform.position, prefab.transform.rotation, null);
	}

	public static GameObject CreateSingle(GameObject prefab, Transform parent) {
		return CreateSingle(prefab, prefab.transform.position, prefab.transform.rotation, parent);
	}

	public static GameObject CreateSingle(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) {
		return Instance.CreateSingleInternal(prefab, position, rotation, parent);
	}

	private GameObject CreateSingleInternal(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) {
		if (prefab == null) {
			Debug.LogError("Trying to create null prefab with CreateSingle");
			return null;
		}

		GameObject instanceObject = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
		if (instanceObject == null) {
			Debug.LogError("Instantiate failed in CreateSingle");
			return null;
		}

#if UNITY_EDITOR
		int hashCode = instanceObject.GetHashCode();
		standaloneInstances.Add(instanceObject);
#endif //UNITY_EDITOR

		return instanceObject;
	}

	public static T CreateSingle<T>(T prefab) where T : Component {
		if (prefab == null) {
			return null;
		}

		T typedInstance = UnityEngine.Object.Instantiate(prefab);
		if (typedInstance == null) {
			return null;
		}

		Instance.TrackSingle(typedInstance);

		return typedInstance;
	}

	public static T CreateSingle<T>(T prefab, Transform parent) where T : Component {
		if (prefab == null) {
			return null;
		}

		T typedInstance = UnityEngine.Object.Instantiate(prefab, parent);
		if (typedInstance == null) {
			return null;
		}

		Instance.TrackSingle(typedInstance);

		return typedInstance;
	}

	public static T CreateSingle<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component {
		if (prefab == null) {
			return null;
		}

		T typedInstance = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
		if (typedInstance == null) {
			return null;
		}

		Instance.TrackSingle(typedInstance);

		return typedInstance;
	}

	private void TrackSingle<T>(T typedInstance) where T : Component {
#if UNITY_EDITOR
		GameObject instanceObject = typedInstance.gameObject;
		if (instanceObject != null) {
			standaloneInstances.Add(instanceObject);
		} else {
			Debug.LogErrorFormat("{0} is missing a game object", typedInstance.name);
		}
#endif //UNITY_EDITOR
	}

	////////////////////////////////////////////////////////////////////////////////
	// Destroy Standalone

	public static void DestroySingle(GameObject instance) {
		DestroySingle(instance, 0.0f);
	}

	public static void DestroySingle(GameObject instance, float delay) {
		if (InstanceExists == false) {
			return;
		}

		Instance.DestroySingleInternal(instance, delay);
	}

	private void DestroySingleInternal(GameObject instance, float delay) {
		if (instance == null) {
			return;
		}


		IPooledObject pooledObject = IsPooledObject(instance);
		if (pooledObject != null) {
			Debug.LogError("Cannot destroy a single pooled object. Use recycle instead.");
			return;
		}

#if UNITY_EDITOR
		if (destructionList.Contains(instance) == true) {
			Debug.LogWarningFormat("{0} is begin destoryed more than once, check code for multiple destroy calls", instance.name);
			return;
		} else {
			if (standaloneInstances.Contains(instance) == false) {
				Debug.LogWarningFormat("Destorying {0} without calling SpawnStandalonePrefab", instance.name);
			} else {
				standaloneInstances.Remove(instance);
			}

			destructionList.Add(instance);
		}

		if (Application.isPlaying == false) {
			UnityEngine.Object.DestroyImmediate(instance);
			return;
		}

#endif //UNITY_EDITOR

		if (delay <= 0.0f) {
			UnityEngine.Object.Destroy(instance);
		} else {
			UnityEngine.Object.Destroy(instance, delay);
		}
	}

	public static void DestroySingle<T>(T instance, bool force = false) where T : Component {
		DestroySingle(instance, 0.0f);
	}

	public static void DestroySingle<T>(T instance, float delay, bool force = false) where T : Component {
		if (InstanceExists == false) {
			return;
		}

		Instance.DestroySingleInternal(instance, delay);
	}

	private void DestroySingleInternal<T>(T instance, float delay, bool force = false) where T : Component {
		if (instance == null) {
			return;
		}

		if (force == false) {
			Debug.LogError("Failed to destory component, if you are sure you want to delete a component set the force argument to true");
			return;
		}

		if (delay <= 0.0f) {
			UnityEngine.Object.Destroy(instance);
		} else {
			UnityEngine.Object.Destroy(instance, delay);
		}
	}

	////////////////////////////////////////////////////////////////////////////////
	// Create Pool

	public static void CreatePool(GameObject prefab, int initialPoolSize) {
		IPooledObject pooledObject = IsPooledObject(prefab);
		if (pooledObject == null) {
			Debug.LogError("Failed to create pool. Prefab provided is not a pooled object");
			return;
		}

		CreatePool(pooledObject, initialPoolSize);
	}

	public static void CreatePool(GameObject prefab, int initialPoolSize, PoolGrowthType growthType) {
		IPooledObject pooledObject = IsPooledObject(prefab);
		if (pooledObject == null) {
			Debug.LogError("Failed to create pool. Prefab provided is not a pooled object");
			return;
		}

		CreatePool(pooledObject, initialPoolSize, growthType);
	}

	public static void CreatePool(GameObject prefab, int initialPoolSize, PoolGrowthDelegate growthDelegate) {
		IPooledObject pooledObject = IsPooledObject(prefab);
		if (pooledObject == null) {
			Debug.LogError("Failed to create pool. Prefab provided is not a pooled object");
			return;
		}

		CreatePool(pooledObject, initialPoolSize, growthDelegate);
	}

	public static void CreatePool<T>(T prefab, int initialPoolSize) where T : class, IPooledObject {
		Instance.CreatePoolInternal(prefab, initialPoolSize, PoolGrowthType.Append, null);
	}

	public static void CreatePool<T>(T prefab, int initialPoolSize, PoolGrowthType growthType) where T : class, IPooledObject {
		Instance.CreatePoolInternal(prefab, initialPoolSize, growthType, null);
	}

	public static void CreatePool<T>(T prefab, int initialPoolSize, PoolGrowthDelegate growthDelegate) where T : class, IPooledObject {
		Instance.CreatePoolInternal(prefab, initialPoolSize, PoolGrowthType.Append, growthDelegate);
	}

	private void CreatePoolInternal<T>(T prefab, int initialPoolSize, PoolGrowthType growthType, PoolGrowthDelegate growthDelegate) where T : class, IPooledObject {
		if (prefab == null) {
			return;
		}

		if (objectPools.ContainsKey(prefab) == true) {
			Debug.LogErrorFormat("Failed to create pool. Pool already exists for {0}", prefab.name);
			return;
		}

		IObjectPool objectPool = new ObjectPool<T>(prefab, growthType, growthDelegate);
		if (objectPool == null) {
			Debug.LogError("Failed to create pool. Unknown reason");
			return;
		}

		objectPool.IncreasePoolSize(initialPoolSize);
		objectPools.Add(prefab, objectPool);
	}

	////////////////////////////////////////////////////////////////////////////////
	// Destroy Pool

	public static void DestroyPool(GameObject prefab) {
		if (InstanceExists == false) {
			return;
		}

		IPooledObject pooledObject = IsPooledObject(prefab);
		if (pooledObject == null) {
			Debug.LogError("Failed to destory pool. Prefab provided is not a pooled object");
			return;
		}

		Instance.DestoryPoolInternal(pooledObject);
	}

	public static void DestroyPool(IPooledObject prefab) {
		if (InstanceExists == false) {
			return;
		}

		Instance.DestoryPoolInternal(prefab);
	}

	private void DestoryPoolInternal(IPooledObject prefab) {
		if (prefab == null) {
			return;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			Debug.LogErrorFormat("Failed to destory pool. Either a pool does not exist for {0} or the object provided was not the prefab", prefab.name);
			return;
		}

		objectPool.DestroyInstances();
		objectPools.Remove(prefab);
	}

	////////////////////////////////////////////////////////////////////////////////
	// Recycle Pool

	public static void RecyclePool(GameObject prefab) {
		IPooledObject pooledObject = IsPooledObject(prefab);
		if (pooledObject == null) {
			Debug.LogError("Failed to destory pool. Prefab provided is not a pooled object");
			return;
		}

		Instance.RecyclePoolInternal(pooledObject);
	}

	public static void RecyclePool(IPooledObject prefab) {
		Instance.RecyclePoolInternal(prefab);
	}

	private void RecyclePoolInternal(IPooledObject prefab) {
		if (prefab == null) {
			return;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			Debug.LogErrorFormat("Failed to recycle pool. Either a pool does not exist for {0} or the object provided was not the prefab", prefab.name);
			return;
		}

		objectPool.RecycleInstances();
	}

	////////////////////////////////////////////////////////////////////////////////
	// Spawn 

	public static GameObject Spawn(GameObject prefab) {
		return Spawn(prefab, prefab.transform.position, prefab.transform.rotation, null);
	}

	public static GameObject Spawn(GameObject prefab, Transform parent) {
		return Spawn(prefab, prefab.transform.position, prefab.transform.rotation, parent);
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) {
		IPooledObject pooledObject = IsPooledObject(prefab);
		if (pooledObject == null) {
			return null;
		}

		GameObject instance = Instance.SpawnInternal(pooledObject, position, rotation, parent).gameObject;
		if (instance == null) {
			return null;
		}

		return instance;
	}

	public static T Spawn<T>(T prefab) where T : class, IPooledObject {
		return Spawn(prefab, null, prefab.transform.position, prefab.transform.rotation);
	}

	public static T Spawn<T>(T prefab, Transform parent) where T : class, IPooledObject {
		return Spawn(prefab, parent, prefab.transform.position, prefab.transform.rotation);
	}

	public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : class, IPooledObject {
		return Instance.SpawnInternal<T>(prefab, position, rotation, parent);
	}

	public T SpawnInternal<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : class, IPooledObject {
		if (prefab == null) {
			Debug.LogError("Cannot spawn null prefab");
			return null;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			Debug.LogErrorFormat("Need to create a pool for {0} before you spawn an instance.", prefab.name);
			return null;
		}

		IPooledObject instance = objectPool.Spawn();
		if (instance == null) {
			return null;
		}

		T typedInstance = instance as T;
		if (typedInstance == null) {
			Debug.LogError("Cannot spawn instance, incompatible type");
			objectPool.Recycle(instance);
			return null;
		}

		GameObject instanceObject = typedInstance.gameObject;
		if (instanceObject != null) {
			instanceObject.SetActive(true);
			instanceObject.transform.SetParent(parent, false);
			instanceObject.transform.localPosition = position;
			instanceObject.transform.localRotation = rotation;
		}

		return typedInstance;
	}

	////////////////////////////////////////////////////////////////////////////////
	// Recycle

	public static void Recycle(IPooledObject instance) {
		Instance.RecycleInternal(instance);
	}

	public static void Recycle(GameObject instance) {
		IPooledObject pooledObject = IsPooledObject(instance);
		if (pooledObject == null) {
			return;
		}

		Instance.RecycleInternal(pooledObject);
	}

	private void RecycleInternal(IPooledObject instance) {
		if (instance == null) {
			return;
		}

		IObjectPool objectPool = GetObjectPool(instance.prefab);
		if (objectPool == null) {
			return;
		}

		GameObject instanceObject = instance.gameObject;
		if (instanceObject != null) {
			instanceObject.transform.SetParent(transform, false);
			instanceObject.SetActive(false);
		}

		objectPool.Recycle(instance);
	}

	////////////////////////////////////////////////////////////////////////////////
	// Get Pooled Objects

	public static IList<T> GetPooledObjects<T>(T prefab) where T : IPooledObject {
		return Instance.GetPooledObjectsInternal(prefab);
	}

	private IList<T> GetPooledObjectsInternal<T>(T prefab) where T : IPooledObject {
		if (prefab == null) {
			return null;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			return null;
		}

		return objectPool.GetPooledObjects<T>();
	}

	public static int GetPooledObjectsCount(IPooledObject prefab) {
		return Instance.GetPooledObjectsCountInternal(prefab);
	}

	private int GetPooledObjectsCountInternal(IPooledObject prefab) {
		if (prefab == null) {
			return 0;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			return 0;
		}

		return objectPool.ObjectCount;
	}

	////////////////////////////////////////////////////////////////////////////////
	//  Get Pooled Objects Free

	public static IList<T> GetPooledObjectsFree<T>(T prefab) where T : IPooledObject {
		return Instance.GetPooledObjectsFreeInternal(prefab);
	}

	private IList<T> GetPooledObjectsFreeInternal<T>(T prefab) where T : IPooledObject {
		if (prefab == null) {
			return null;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			return null;
		}

		return objectPool.GetPooledObjectsFree<T>();
	}

	public static int GetPooledObjectsFreeCount(IPooledObject prefab) {
		return Instance.GetPooledObjectsFreeCountInternal(prefab);
	}

	private int GetPooledObjectsFreeCountInternal(IPooledObject prefab) {
		if (prefab == null) {
			return 0;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			return 0;
		}

		return objectPool.FreeCount;
	}

	////////////////////////////////////////////////////////////////////////////////
	// Get Pooled Objects Spawned

	public static IList<T> GetPooledObjectsSpawned<T>(T prefab) where T : IPooledObject {
		return Instance.GetPooledObjectsSpawnedInternal(prefab);
	}

	private IList<T> GetPooledObjectsSpawnedInternal<T>(T prefab) where T : IPooledObject {
		if (prefab == null) {
			return null;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			return null;
		}

		return objectPool.GetPooledObjectsSpawned<T>();
	}

	public static int GetPooledObjectsSpawnedCount(IPooledObject prefab) {
		return Instance.GetPooledObjectsSpawnedCountInternal(prefab);
	}

	private int GetPooledObjectsSpawnedCountInternal(IPooledObject prefab) {
		if (prefab == null) {
			return 0;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			return 0;
		}

		return objectPool.SpawnedCount;
	}

	////////////////////////////////////////////////////////////////////////////////
	// Utils

	public static IPooledObject IsPooledObject(Component instance) {
		return IsPooledObject(instance.gameObject);
	}

	public static IPooledObject IsPooledObject(GameObject instance) {
		return Instance.IsPooledObjectInternal(instance);
	}

	private IPooledObject IsPooledObjectInternal(GameObject instance) {
		if (instance == null) {
			return null;
		}
		return instance.GetComponent<IPooledObject>();
	}

	public static bool IsPooledPrefab(IPooledObject instance) {
		return Instance.IsPooledPrefabInternal(instance);
	}

	private bool IsPooledPrefabInternal(IPooledObject instance) {
		if (instance == null) {
			return false;
		}

		if (instance.prefab == null) {
			return true;
		}
		return false;
	}

	public static bool IsPooledInstance(IPooledObject instance) {
		return Instance.IsPooledInstanceInternal(instance);
	}

	private bool IsPooledInstanceInternal(IPooledObject instance) {
		if (instance == null) {
			return false;
		}

		if (instance.prefab == null) {
			return false;
		}
		return true;
	}

	public static bool PoolExists<T>(T prefab) where T : IPooledObject {
		return Instance.PoolExistsInternal(prefab);
	}

	private bool PoolExistsInternal<T>(T prefab) where T : IPooledObject {
		if (prefab == null) {
			return false;
		}

		IObjectPool objectPool = GetObjectPool(prefab);
		if (objectPool == null) {
			return false;
		}

		return true;
	}

	////////////////////////////////////////////////////////////////////////////////
	// Logging


	public int LogStandaloneAssets() {
#if UNITY_EDITOR
		if (standaloneInstances == null || standaloneInstances.Count <= 0) {
			return 0;
		}

		foreach (UnityEngine.Object standaloneInstance in standaloneInstances) {
			if (standaloneInstance == null) {
				continue;
			}

			//Debug.LogFormat("FWObjectPool: {0}", standaloneInstance.name);
		}

		return standaloneInstances.Count;
#else
	return 0;
#endif //UNITY_EDITOR
	}
}

////////////////////////////////////////////////////////////////////////////////
// Extensions

public static class FWObjectPoolExtensions {

	//Create Pool
	public static void CreatePool(this GameObject prefab, int initialPoolSize = 0) {
		Objects.CreatePool(prefab, initialPoolSize);
	}

	public static void CreatePool(this GameObject prefab, int initialPoolSize, Objects.PoolGrowthType growthType) {
		Objects.CreatePool(prefab, initialPoolSize, growthType);
	}

	public static void CreatePool(this GameObject prefab, int initialPoolSize, Objects.PoolGrowthDelegate growthDelegate) {
		Objects.CreatePool(prefab, initialPoolSize, growthDelegate);
	}

	public static void CreatePool(this IPooledObject prefab, int initialPoolSize = 0) {
		Objects.CreatePool(prefab, initialPoolSize);
	}

	public static void CreatePool(this IPooledObject prefab, int initialPoolSize, Objects.PoolGrowthType growthType) {
		Objects.CreatePool(prefab, initialPoolSize, growthType);
	}

	public static void CreatePool(this IPooledObject prefab, int initialPoolSize, Objects.PoolGrowthDelegate growthDelegate) {
		Objects.CreatePool(prefab, initialPoolSize, growthDelegate);
	}

	//Destroy
	public static void DestroyPool(this GameObject prefab) {
		Objects.DestroyPool(prefab);
	}

	public static void DestroyPool(this IPooledObject prefab) {
		Objects.DestroyPool(prefab);
	}

	//Recycle

	public static void RecyclePool(this GameObject prefab) {
		Objects.RecyclePool(prefab);
	}

	public static void RecyclePool(this IPooledObject prefab) {
		Objects.RecyclePool(prefab);
	}

	//Spawn
	public static GameObject Spawn(this GameObject prefab) {
		return Spawn(prefab, null, Vector2.zero, Quaternion.identity);
	}

	public static GameObject Spawn(this GameObject prefab, Transform parent) {
		return Spawn(prefab, parent, Vector2.zero, Quaternion.identity);
	}

	public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation) {
		IPooledObject pooledObject = prefab.GetComponent<IPooledObject>();
		if (pooledObject == null) {
			return null;
		}

		GameObject instance = Spawn(pooledObject, parent, position, rotation).gameObject;
		if (instance == null) {
			return null;
		}

		return instance;
	}

	public static T Spawn<T>(this T prefab) where T : class, IPooledObject {
		return Objects.Spawn<T>(prefab, null, Vector3.zero, Quaternion.identity);
	}

	public static T Spawn<T>(this T prefab, Transform parent) where T : class, IPooledObject {
		return Objects.Spawn<T>(prefab, parent, Vector3.zero, Quaternion.identity);
	}

	public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : class, IPooledObject {
		return Objects.Spawn<T>(prefab, parent, position, rotation);
	}

	//Recycle
	public static void Recycle(this GameObject prefab) {
		Objects.Recycle(prefab);
	}

	public static void Recycle(this IPooledObject prefab) {
		Objects.Recycle(prefab);
	}

	//Spawned Instances
	public static IList<T> GetPooledObjects<T>(this T instance) where T : IPooledObject {
		return Objects.GetPooledObjects(instance);
	}

	public static int GetPooledObjectsCount<T>(this IPooledObject instance) {
		return Objects.GetPooledObjectsCount(instance);
	}

	public static IList<T> GetPooledObjectsFree<T>(this T instance) where T : IPooledObject {
		return Objects.GetPooledObjectsFree(instance);
	}

	public static int GetPooledObjectsFreeCount<T>(this IPooledObject instance) {
		return Objects.GetPooledObjectsFreeCount(instance);
	}

	public static IList<T> GetPooledObjectsSpawned<T>(this T instance) where T : IPooledObject {
		return Objects.GetPooledObjectsSpawned(instance);
	}

	public static int GetPooledObjectsSpawnedCount<T>(this IPooledObject instance) {
		return Objects.GetPooledObjectsSpawnedCount(instance);
	}

	//Utils
	public static IPooledObject IsPooledObject(this GameObject instance) {
		return Objects.IsPooledObject(instance);
	}

	public static bool IsPooledPrefab(IPooledObject instance) {
		return Objects.IsPooledPrefab(instance);
	}

	public static bool IsPooledInstance(IPooledObject instance) {
		return Objects.IsPooledInstance(instance);
	}

	public static bool PoolExists(IPooledObject instance) {
		return Objects.PoolExists(instance);
	}
}

////////////////////////////////////////////////////////////////////////////////
// Pooled Object Interface

public interface IPooledObject {

	//Monobehaviour Properties
	GameObject gameObject {
		get;
	}

	Transform transform {
		get;
	}

	string name {
		get;
	}

	//Pooling Properties
	IPooledObject prefab {
		get;
		set;
	}

	//Pooling Methods
	void OnEnterPool();
}