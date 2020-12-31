public class StaticSingleton<T> where T : class, new() {
	private static bool valid = false;
	private static T instance = null;

	public static T Instance {
		get {
			if (valid == false) {
				instance = new T();
				valid = true;
			}
			return instance;
		}
	}

	public static bool InstanceExists {
		get {
			return valid;
		}
	}
}
