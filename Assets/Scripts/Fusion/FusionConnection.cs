using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] bool connectOnAwake = true;
    [HideInInspector] public NetworkRunner runner { get; private set; }

    private void Awake()
    {
        if (connectOnAwake)
        {
            ConnectToRunner();
        }
    }

    async void ConnectToRunner()
    {
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        var customProps = new Dictionary<string, SessionProperty>()
        {
            { "category", PlayerPrefs.GetString("SelectedCategory") },
        };

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionProperties = customProps,
            PlayerCount = 2
        });

        if (result.Ok)
        {
            Debug.Log("aaaa Started");
            Debug.Log("aaaa Session info: " + runner.SessionInfo);

            
        }
        else
        {
            Debug.LogError($"aaaa Failed to Start: {result.ShutdownReason}");
        }

    }


    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("AAAA Connected to server");
        Debug.Log("AAAA playerid: " + runner.LocalPlayer.PlayerId);

        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("AAAA Player joined: " + player);

        FusionQuizManager quizManager = FindObjectOfType<FusionQuizManager>();
        if (quizManager != null)
        {
            Debug.Log("AAAA getting question");
            Debug.Log(quizManager.gameObject.name);
            quizManager.GetQuestion();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
