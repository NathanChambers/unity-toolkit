using System;
public interface IContentService : IService {
	void GetDownloadURL(string key, Action<string, Error> response);
}