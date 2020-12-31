public interface IProvider {
    void Initialise();
    void Cleanup();
    ServiceType GetService<ServiceType>() where ServiceType : IService;
}