using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    // Start is called before the first frame update
    private void Start()
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(1, 15), UnityEngine.Random.Range(1, 5));
        PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        PhotonPeer.RegisterType(typeof(Vector2Int), 242, SerializeVector2Int, DeserializeVector2Int);
    }

    // Update is called once per frame
   
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // Когда текущий игрок покинул комнату
        SceneManager.LoadScene(0);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player (0) entered room", newPlayer.NickName);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player (0) left room", otherPlayer.NickName);
    }
    public static object DeserializeVector2Int(byte[] data)
    {
        Vector2Int result = new Vector2Int();
        result.x = BitConverter.ToInt32(data, 0);
        result.y = BitConverter.ToInt32(data, 4);
        return result;
    }
    public static byte[] SerializeVector2Int(object obj)
    {
        Vector2Int vector = (Vector2Int)obj;
        byte[] result = new byte[8];   
        BitConverter.GetBytes(vector.x).CopyTo(result, 0);
        BitConverter.GetBytes(vector.y).CopyTo(result, 4);
        return result;
    }
}
