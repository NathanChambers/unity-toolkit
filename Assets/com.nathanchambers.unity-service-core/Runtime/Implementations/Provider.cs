using System.Collections.Generic;
using UnityEngine;

public abstract class Provider : IProvider {
    private Dictionary<int, IService> services = new Dictionary<int, IService>();

    public virtual void Initialise() {

    }

    public virtual void Cleanup() {

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
    

    protected void RegisterService<ServiceType>() where ServiceType : IService, new() {
        int typeHash = typeof(ServiceType).GetHashCode();
        var service = new ServiceType();
        services.Add(typeHash, service);
    }
}