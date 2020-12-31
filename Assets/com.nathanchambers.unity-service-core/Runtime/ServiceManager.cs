using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager {
	private static ServiceManager instance = null;
	public static ServiceManager Instance {
		get {
			if (instance == null) {
				instance = new ServiceManager();
			}
			return instance;
		}
	}

	protected Dictionary<int, IProvider> providers = new Dictionary<int, IProvider>();
	private bool initialised = false;

	////////////////////////////////////////////////////////////////////////////////

	public void Initialise() {
		Requires.True(initialised == false);
		foreach(var provider in providers.Values) {
			provider.Initialise();
		}
		initialised = true;
	}

	public void Cleanup() {
		Requires.True(initialised == true);
		foreach(var provider in providers.Values) {
			provider.Cleanup();
		}
		providers.Clear();
		initialised = false;
	}

	////////////////////////////////////////////////////////////////////////////////

	public void RegisterProvider<ProviderType>(ProviderType provider) where ProviderType : IProvider {
		Requires.True(initialised == false, $"Failed to register {typeof(ProviderType).Name}. Service manager already initialised");
		
		int typeHash = typeof(ProviderType).GetHashCode();
		Requires.True(providers.ContainsKey(typeHash) == false, $"Failed to register {typeof(ProviderType).Name}. Provider already registered");
		providers.Add(typeHash, provider);
	}

	public void RegisterProvider<ProviderType>() where ProviderType : IProvider, new() {
		ProviderType provider = new ProviderType();
		RegisterProvider(provider);
	}

	////////////////////////////////////////////////////////////////////////////////

	public ProviderType GetProvider<ProviderType>() where ProviderType : IProvider {
		int typeHash = typeof(ProviderType).GetHashCode();
		Requires.True(providers.ContainsKey(typeHash) == true, $"Failed to get {typeof(ProviderType).Name}. Provider not registered");
		return (ProviderType)providers[typeHash];
	}

	public ServiceType GetService<ServiceType, ProviderType>() 
		where ServiceType : IService 
		where ProviderType : IProvider {

		IProvider provider = GetProvider<ProviderType>();
		Requires.NotNull(provider);

		return provider.GetService<ServiceType>();
	}

	public IList<ServiceType> GetServices<ServiceType>() where ServiceType : IService {
		List<ServiceType> services = new List<ServiceType>();
		foreach (IProvider provider in providers.Values) {
			if (provider == null) {
				continue;
			}

			ServiceType service = provider.GetService<ServiceType>();
			if (service == null) {
				continue;
			}

			services.Add(service);
		}
		return services;
	}
}