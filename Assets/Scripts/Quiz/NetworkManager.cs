using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    //private NetworkRunner _runner;

    //void Start()
    //{
    //    StartGame();
    //}

    //async void StartGame()
    //{
    //    _runner = gameObject.AddComponent<NetworkRunner>();
    //    _runner.ProvideInput = true;

    //    var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
    //    var sceneInfo = new NetworkSceneInfo();
    //    if (scene.IsValid)
    //    {
    //        sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
    //    }

    //    await _runner.StartGame(new StartGameArgs()
    //    {
    //        GameMode = GameMode.AutoHostOrClient,
    //        SessionName = "QuizSession",
    //        Scene = scene,
    //        SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
    //    });
    //}

    //public async Task JoinOrCreateRoom(string category)
    //{
    //    var rooms = await _runner.FindSessionsAsync();

    //    foreach (var room in rooms)
    //    {
    //        if (room.SessionInfo.Name == category && room.PlayerCount < 2)
    //        {
    //            await _runner.JoinSession(room);
    //            return;
    //        }
    //    }

    //    await _runner.StartGame(new StartGameArgs()
    //    {
    //        GameMode = GameMode.Host,
    //        SessionName = category,
    //        Scene = SceneManager.GetActiveScene().buildIndex,
    //        SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
    //    });
    //}

}
