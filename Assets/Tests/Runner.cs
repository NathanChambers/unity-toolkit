using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxyLocalService : ILocalService {

    public ProxyLocalService() {
        Debug.Log("ProxyLocalService");
    }
    public void GetUserData(IUser user, string key, Action<string, Dictionary<string, string>, Error> response) {
        Dictionary<string, string> data = new Dictionary<string, string>{
            {"key","value"},
        };
        response.Invoke(key, data, null);
    }

	public void SetUserData(IUser user, string key, Dictionary<string, string> data, Action<Error> response) {
        response.Invoke(null);
    }

	public void DeleteUserData(IUser user, string key, Action<Error> response) {
        response.Invoke(null);
    }

	public bool UserDataExists(IUser user, string key) {
        return false;
    }

}

public class ProxyProvider : IProvider {
    private Dictionary<int, IService> services = new Dictionary<int, IService>();

    public ProxyProvider() {
        Debug.Log("ProxyProvider");
        RegisterService<ProxyLocalService>();
    }

    public void Initialise() {

    }

    public void Cleanup() {

    }


    public ServiceType GetService<ServiceType>() where ServiceType : IService {
        int typeHash = typeof(ServiceType).GetHashCode();
        if(services.ContainsKey(typeHash) == true) {
            return (ServiceType)services[typeHash];
        }

        foreach(var service in services.Values) {
            if(service is ServiceType) {
                return (ServiceType)service;
            }
        }
        
        throw new UnityException($"Failed to get service. {typeof(ServiceType).Name} not registered with {GetType().Name}");
    }
    

    private void RegisterService<ServiceType>() where ServiceType : IService, new() {
        int typeHash = typeof(ServiceType).GetHashCode();
        var service = new ServiceType();
        services.Add(typeHash, service);
    }
}

public class Runner : Toolkit.MonoSingleton<Runner> {
    public void Start() {
        ServiceManager.Instance.RegisterProvider<ProxyProvider>();
        ServiceManager.Instance.Initialise();


        var localService = ServiceManager.Instance.GetService<ILocalService, ProxyProvider>();
        localService.GetUserData(null, "private-data", (key, data, err) => {
            if(err != null) {
                Debug.LogError(err);
                return;
            }

            Debug.Log($"local key = {key}");
            foreach(var kvp in data) {
                Debug.Log($"data: {kvp.Key}, {kvp.Value}");
            }
        });
    }
}
