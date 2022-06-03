using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager instance;

    private void Awake()
    {
        if(instance)
        {
            Destroy(instance);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-3, 3), 2, Random.Range(-3, 3));
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.Instantiate("First_Person_Player", spawnPosition, Quaternion.identity);
        }
        else
        {
            Instantiate(Resources.Load("First_Person_Player"), spawnPosition, Quaternion.identity);
        }
    }

}
