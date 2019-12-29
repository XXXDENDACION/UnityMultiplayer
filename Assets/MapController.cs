using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapController : MonoBehaviour, IOnEventCallback
{
    public GameObject CellPrefab;
    private GameObject[,] cells;
    private double lastTimeTick;
    private List<PlayerControls> players = new List<PlayerControls>();
    public void AddPlayer(PlayerControls player)
    {
        players.Add(player);
        cells[player.GamePostiton.x, player.GamePostiton.y].SetActive(false);
    }




    // Start is called before the first frame update
    private void Start()
    {
        cells = new GameObject[20, 10];
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                cells[x, y] = Instantiate(CellPrefab, new Vector3(x, y), Quaternion.identity, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.Time > lastTimeTick + 1 && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {

            //Разослать все события
            Vector2Int[] directions = players
                .OrderBy(p => p.photonView.Owner.ActorNumber)
                .Select(p => p.Direction)
                .ToArray();
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(42, directions, options, sendOptions);
            //Сделать шаг игры
            PerfromTick(directions);
        }
    }



    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 42:
                Vector2Int[] directions = (Vector2Int[])photonEvent.CustomData;
                PerfromTick(directions);
                break;
        }
    }

    private void PerfromTick(Vector2Int[] directions)
    {
        if (players.Count != directions.Length) return;
        PlayerControls[] sortedPlayers = players
            .OrderBy(p => p.photonView.Owner.ActorNumber)
            .ToArray();
        int i = 0;
        foreach (var player in sortedPlayers)
        {
            player.Direction = directions[i++];
            MinePlayerBlock(player);


        }
        foreach (var player in sortedPlayers)
        {
            MovePlayer(player);


        }
        lastTimeTick = PhotonNetwork.Time;

    }
    private void MinePlayerBlock(PlayerControls player)
    {
        Vector2Int targetPosition = player.GamePostiton  + player.Direction;
        if (targetPosition.x < 0) return;
        if (targetPosition.y < 0) return;
        if (targetPosition.x >= cells.GetLength(0)) return;
        if (targetPosition.y >= cells.GetLength(1)) return;
        cells[targetPosition.x, targetPosition.y].SetActive(false);

        Vector2Int pos = targetPosition;
        PlayerControls minePlayer = players.First(p => p.photonView.IsMine);
        if (minePlayer != player)
        {
            while (pos.y < cells.GetLength(1) && !cells[pos.x, pos.y].activeSelf)
            {
                if (pos == minePlayer.GamePostiton)
                {
                    PhotonNetwork.LeaveRoom();
                    break;
                }
                    pos.y++;
            }
        }
    }
    private void  MovePlayer(PlayerControls player)
    {
      
        player.GamePostiton += player.Direction;
        if (player.GamePostiton.x < 0) player.GamePostiton.x = 0;
        if (player.GamePostiton.y < 0) player.GamePostiton.y = 0;
        if (player.GamePostiton.x >= cells.GetLength(0)) player.GamePostiton.x = cells.GetLength(0) - 1;
        if (player.GamePostiton.y >= cells.GetLength(1)) player.GamePostiton.y = cells.GetLength(1) - 1;
        cells[player.GamePostiton.x, player.GamePostiton.y].SetActive(false);
        int ladderlength = 0;
        Vector2Int pos = player.GamePostiton;
        while (pos.y > 0 && !cells[pos.x, pos.y - 1].activeSelf)
        {
            ladderlength++;
            pos.y--;
        }
        player.SetLadderLength(ladderlength);
    }
public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}

