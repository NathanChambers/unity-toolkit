using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager {

	public enum eInitState {
		READY,
		LOADING,
		ERROR,
	}

	private static ServiceManager instance = null;

	public static ServiceManager Instance {
		get {
			if (instance == null) {
				instance = new ServiceManager();
			}
			return instance;
		}
	}

	////////////////////////////////////////////////////////////////////////////////
	public event Action<IUser> OnUserAuthenticated;

	////////////////////////////////////////////////////////////////////////////////

	protected bool hasInitialised = false;
	protected Dictionary<int, IServiceProvider> serviceProviders = new Dictionary<int, IServiceProvider>();
	protected Dictionary<int, IServiceProvider> defaultProviders = new Dictionary<int, IServiceProvider>();
	protected Dictionary<uint,IUser> cacheUsers = new Dictionary<uint,IUser>();

	////////////////////////////////////////////////////////////////////////////////

	public IUser PrimaryUser {
		get {
			return GetCacheUser();
		}
	}

	public IDictionary<uint, IUser> CacheUsers {
		get {
			return cacheUsers;
		}
	}

	public bool IsReady {
		get {
			foreach(var provider in serviceProviders.Values) {
				if(provider.InitState != eInitState.READY) {
					return false;
				}
			}
			return true;
		}
	}

	////////////////////////////////////////////////////////////////////////////////

	public void Initialise() {
		serviceProviders.Clear();
		defaultProviders.Clear();
		cacheUsers.Clear();
		
		hasInitialised = true;

		UpdateCacheUser(primaryUser);
	}

	public void Cleanup() {
		if (hasInitialised == false) {
			return;
		}
		
		hasInitialised = false;
		serviceProviders.Clear();
		defaultProviders.Clear();
		cacheUsers.Clear();
	}

	////////////////////////////////////////////////////////////////////////////////

	public IUser GetCacheUser(uint uid) {
		return cacheUsers[uid];
	}

	public IUser GetCacheUser(System.Predicate<IUser> match) {
		if (cacheUsers == null || cacheUsers.Count <= 0) {
			return null;
		}

		foreach (IUser user in cacheUsers.Values) {
			if (user == null) {
				continue;
			}

			if (match(user) == false) {
				continue;
			}

			return user;
		}

		return IUser.Generate();
	}

	public List<IUser> GetCacheUsers(System.Predicate<IUser> match) {
		List<IUser> matchUsers = new List<IUser>();
		if (cacheUsers == null || cacheUsers.Count <= 0) {
			return matchUsers;
		}

		foreach (IUser cacheUser in cacheUsers.Values) {
			if (cacheUser == null) {
				continue;
			}

			if (match.Invoke(cacheUser) == false) {
				continue;
			}

			matchUsers.Add(cacheUser);
		}

		return matchUsers;
	}

	public void RemoveCacheUser(IUser user) {
		if (user == null) {
			return;
		}

		if (cacheUsers.ContainsKey(user.UID) == false) {
			return;
		}

		cacheUsers.Remove(user.UID);
	}

	public void UpdateCacheUser(IUser user) {
		if (user == null) {
			return;
		}

		if (cacheUsers.ContainsKey(user.UID) == false) {
			cacheUsers.Add(user.UID, user);
		} else {
			cacheUsers[user.UID] = user;
		}
	}

	////////////////////////////////////////////////////////////////////////////////

	protected void RegisterProvider<T>() where T : IServiceProvider, new() {
		T serviceProvider = new T();
		if (serviceProvider == null) {
			Debug.LogError("Failed to register provider, instance is null");
			return;
		}

		int providerTypeHash = typeof(T).GetHashCode();
		if (serviceProviders.ContainsKey(providerTypeHash) == true) {
			Debug.LogErrorFormat("Provider {0} has already been registered");
			return;
		}

		serviceProviders.Add(providerTypeHash, serviceProvider);
	}

	

	public void SetDefaultProvider<ServciceType, ProviderType>() 
		where ServciceType: IService
		where ProviderType: IServiceProvider {
			int serviceTypeHash = typeof(ServciceType).GetHashCode();
			if(defaultProviders.ContainsKey(serviceTypeHash) == true) {
				Debug.LogErrorFormat("{0} has already been assigned a default provider", typeof(ServciceType).Name);
				return;
			}

			IServiceProvider provider = GetProvider<ProviderType>();
			if(provider == null) {
				Debug.LogErrorFormat("{0} isn't a registered service provider", typeof(ProviderType).Name);
				return;
			}

			defaultProviders.Add(serviceTypeHash, provider);
	}

	////////////////////////////////////////////////////////////////////////////////

	public IServiceProvider GetProvider(int providerTypeHash) {
		if(serviceProviders.ContainsKey(providerTypeHash) == false) {
			return null;
		}
		return serviceProviders[providerTypeHash];
	}

	public ProviderType GetProvider<ProviderType>() where ProviderType : IServiceProvider{
		int typeHash = typeof(ProviderType).GetHashCode();
		if(serviceProviders.ContainsKey(typeHash) == false) {
			return null;
		}

		IServiceProvider provider =  serviceProviders[typeHash];
		if(provider == null) {
			return null;
		}

		ProviderType typedProvider = provider as ProviderType;
		if(typedProvider == null) {
			return null;
		}

		return typedProvider;
	}

	public ServiceType GetService<ServiceType>() where ServiceType : IService {
		if (defaultProviders == null || defaultProviders.Count <= 0) {
			Debug.LogError("Default service provider has not been set");
			return null;
		}

		int typeHash = typeof(ServiceType).GetHashCode();
		if(defaultProviders.ContainsKey(typeHash) == false) {
			Debug.LogError("Default service provider has not been set");
			return null;
		}

		IServiceProvider provider = defaultProviders[typeHash];
		if(provider == null) {
			Debug.LogError("Default service provider is null");
			return null;
		}

		return provider.GetService<ServiceType>();
	}

	public ServiceType GetService<ServiceType, ProviderType>() 
		where ServiceType : IService 
		where ProviderType : IServiceProvider {

		IServiceProvider provider = GetProvider<ProviderType>();
		if (provider == null) {
			return null;
		}

		return provider.GetService<ServiceType>();
	}

	public ServiceTypes[] GetServices<ServiceTypes>() where ServiceTypes : IService {
		List<ServiceTypes> services = new List<ServiceTypes>();
		foreach (IServiceProvider serviceProvider in serviceProviders.Values) {
			if (serviceProvider == null) {
				continue;
			}

			ServiceTypes service = serviceProvider.GetService<ServiceTypes>();
			if (service == null) {
				continue;
			}

			services.Add(service);
		}

		if (services == null || services.Count <= 0) {
			return null;
		}

		return services.ToArray();
	}

	public void UserAuthenticated(IUser user) {
		if(OnUserAuthenticated != null) {
			OnUserAuthenticated.Invoke(user);
		}
	}
}