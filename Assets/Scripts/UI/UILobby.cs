using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor.XR;

/// <summary>
/// 大厅UI控制脚本
/// </summary>
public class UILobby : MonoBehaviourPunCallbacks
{
    public Button btnCreateRoom;
    public Button btnJoinRoom;
    public Text txtStatus;

    void Start()
    {
        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        txtStatus.text = "正在连接服务器...";
    }

    /// <summary>
    /// 连接到服务器成功
    /// </summary>
    public override void OnConnectedToMaster()
    {
        btnCreateRoom.interactable = true;
        btnJoinRoom.interactable = true;
        txtStatus.text = "已连接服务器";
    }

    /// <summary>
    /// 与服务器断开连接
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        txtStatus.text = "断开连接：" + cause;
    }

    /// <summary>
    /// 点击创建房间按钮
    /// </summary>
    public void OnClickCreateRoom()
    {
        txtStatus.text = "正在创建房间...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    /// <summary>
    /// 点击快速加入按钮
    /// </summary>
    public void OnClickJoinRoom()
    {
        txtStatus.text = "正在寻找房间...";
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 成功加入房间
    /// </summary>
    public override void OnJoinedRoom()
    {
        txtStatus.text = "进入房间，加载游戏场景...";
        PhotonNetwork.LoadLevel("Game");
    }

    /// <summary>
    /// 加入随机房间失败
    /// </summary>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        txtStatus.text = "没有找到房间，请创建房间";
    }

    /// <summary>
    /// 创建房间失败
    /// </summary>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        txtStatus.text = "创建房间失败：" + message;
    }
}