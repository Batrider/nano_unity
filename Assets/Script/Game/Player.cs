using System.Text;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData playerData;
    public float moveSpeed = 8f;
    void Update()
    {
        if (playerData == null)
            return;
        if (playerData.id == GameManager.mainPlayerUid)
        {
            OnPlayerInput();
        }
        SyncPlayerPos();
    }
    /// <summary>
    /// sync player pos by lerp
    /// </summary>
    void SyncPlayerPos()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition,
            new Vector3(playerData.posX, playerData.posY), moveSpeed * Time.deltaTime);
    }
    /// <summary>
    /// monitor player's input
    /// </summary>
    void OnPlayerInput()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            SendServerNotify(Vector3.up);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            SendServerNotify(Vector3.down);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            SendServerNotify(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            SendServerNotify(Vector3.right);
        }
    }
    /// <summary>
    /// send the change of player's pos to server
    /// </summary>
    /// <param name="step"></param>
    void SendServerNotify(Vector3 step)
    {
        if (playerData == null)
            return;

        PlayerData data = new PlayerData();
        data.id = this.playerData.id;
        data.posX = transform.localPosition.x + step.x;
        data.posY = transform.localPosition.y + step.y;
        string jsonData = SimpleJson.SimpleJson.SerializeObject(data);
        NetworkManager.StarXService.Notify("World.Update", Encoding.UTF8.GetBytes(jsonData));
    }

    public void UpdatePlayerData(PlayerData data)
    {
        this.playerData.posX = data.posX;
        this.playerData.posY = data.posY;
    }
}

/// <summary>
/// the data use exchange data with server
/// </summary>
public class PlayerData
{
    public ulong id;
    public float posX;
    public float posY;
}
