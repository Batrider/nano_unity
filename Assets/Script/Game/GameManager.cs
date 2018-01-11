using SimpleJson;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    public Player playerPrefab;
    public static ulong mainPlayerUid;
    public List<Player> players;
    public Queue<PlayerData> playerUpdateQueue = new Queue<PlayerData>();
    /// <summary>
    /// net connect
    /// </summary>
    void Start()
    {
        NetworkManager.StartConnect(() =>
        {
            NetworkManager.EnterWorld((msg) =>
            {
                NetworkManager.StarXService.On("update", UpdateHandler);
                CreateMainPlayer(msg);
            });
        });
    }
    /// <summary>
    /// on player data update
    /// </summary>
    /// <param name="msg"></param>
    void UpdateHandler(byte[] msg)
    {
        string jsonData = Encoding.UTF8.GetString(msg);
        PlayerData pData = SimpleJson.SimpleJson.DeserializeObject<PlayerData>(jsonData);
        Player curPlayer = players.Find(x => x.playerData.id == pData.id);
        if (curPlayer != null)
        {
            curPlayer.UpdatePlayerData(pData);
        }
        else
        {
            playerUpdateQueue.Enqueue(pData);
        }
    }
    /// <summary>
    /// player clone queue 
    /// </summary>
    public void Update()
    {
        if (playerUpdateQueue.Count > 0)
        {
            SyncPlayer(playerUpdateQueue.Dequeue());
        }
    }
    /// <summary>
    /// create self
    /// </summary>
    /// <param name="msg"></param>
    void CreateMainPlayer(byte[] msg)
    {
        string jsonData = Encoding.UTF8.GetString(msg);
        JsonObject jsonObj = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(jsonData);
        PlayerData pData = new PlayerData();
        if (jsonObj.ContainsKey("id"))
        {
            mainPlayerUid = Convert.ToUInt64(jsonObj["id"]);
            pData.id = mainPlayerUid;
        }
        playerUpdateQueue.Enqueue(pData);
    }
    /// <summary>
    /// update player position
    /// if player is nil,create
    /// if player != nil,update pos
    /// </summary>
    /// <param name="data"></param>
    void SyncPlayer(PlayerData data)
    {
        Player curPlayer = players.Find(x => x.playerData.id == data.id);
        if (curPlayer != null)
        {
            curPlayer.transform.localPosition = new Vector3(data.posX, data.posY);
        }
        else
        {
            Player player = Instantiate(playerPrefab) as Player;
            player.playerData = data;
            player.transform.position = new Vector3(data.posX, data.posY);
            player.transform.localScale = Vector3.one;
            players.Add(player);
        }
    }
}
