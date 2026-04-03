using UnityEngine;
using Photon.Pun;

// 联机管理器：负责连接Photon服务器，全局单例
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    void Awake()
    {
        // 单例：保证全局只有一个NetworkManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 场景切换不销毁
        DontDestroyOnLoad(gameObject);
        // 核心：所有玩家同步加载场景
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // 连接Photon服务器
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("正在连接Photon服务器...");
        }
    }
}