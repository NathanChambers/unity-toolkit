public class UnityServiceProvider : Provider {
	public UnityServiceProvider() {
		RegisterService<UnityAdService>();
		RegisterService<UnityAnalyticService>();
		RegisterService<UnityNotificationService>();
		RegisterService<UntiyRemoteSettingService>();
	}

	public override void Initialise() {

	}
}