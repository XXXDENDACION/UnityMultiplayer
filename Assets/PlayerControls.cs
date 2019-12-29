using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour, IPunObservable
{

    public PhotonView photonView;
    private SpriteRenderer spriteRenderer;
    private bool isRed;
    public Vector2Int Direction;
    public Vector2Int GamePostiton;
    public Sprite OtherPlayerSprite;
    public Transform ladder;

    public void SetLadderLength(int length)
    {
        for(int i = 0; i < ladder.childCount; i++)
        {
            ladder.GetChild(i).gameObject.SetActive(i < length);
        }
        while(ladder.childCount < length)
        {
            Transform lastTile = ladder.GetChild(ladder.childCount - 1);
            Instantiate(lastTile, lastTile.position + Vector3.down, Quaternion.identity, ladder);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Direction);
        }
        else
        {
            Direction = (Vector2Int) stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        GamePostiton = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        FindObjectOfType<MapController>().AddPlayer(this);
        if (!photonView.IsMine) spriteRenderer.sprite = OtherPlayerSprite;
    }

    // Update is called once per frame
   private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) Direction = Vector2Int.left;
            if (Input.GetKey(KeyCode.RightArrow)) Direction = Vector2Int.right;
            if (Input.GetKey(KeyCode.DownArrow)) Direction = Vector2Int.down;
            if (Input.GetKey(KeyCode.UpArrow)) Direction = Vector2Int.up;
        }
        if (Direction == Vector2Int.left) spriteRenderer.flipX = false;
        if (Direction == Vector2Int.right) spriteRenderer.flipX = true;
        transform.position = Vector3.Lerp(transform.position, (Vector2)GamePostiton, Time.deltaTime * 3);
    }
}
