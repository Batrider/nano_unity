using System;
using StarX;

public class NetworkManager
{
	static StarXClient client = new StarXClient ();
	public static StarXClient StarXService
	{
		get{
			return client;
		}
	}

    public static void StartConnect(Action callback)
    {
        client.Init("127.0.0.1", 23456, () =>
        {
            client.Connect((data) =>
            {
                if (callback != null)
                {
                    callback();
                }
            });
        });
    }

    public static void EnterWorld(Action<byte[]> callback)
    {
        client.Request("World.Enter", new byte[] { }, callback);
    }
}
