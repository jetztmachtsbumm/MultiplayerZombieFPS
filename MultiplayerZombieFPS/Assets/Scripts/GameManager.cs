using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int enemiesAlive = 0;
    public int round = 0;
    public GameObject[] spawnPoints;
    public Text roundNumber;
    public GameObject endScreen;
    public Text roundsSurvived;
    public GameObject pauseMenu;
    public Animator blackScreenAnimator;
    public PhotonView photonView;

    /*    public GameObject enemyPrefab;
        public Text roundNumber;
        public GameObject endScreen;
        public Text roundsSurvivedNumber;
        public Text enemiesKilledNumber;
        public int enemiesKilled;*/
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawner");
    }

    void Update()
    {
        if (PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            if (enemiesAlive == 0)
            {
                round++;
                NextWave(round);
                if (PhotonNetwork.InRoom)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("currentRound", round);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash); 
                }
                else
                {
                    DisplayNextRound(round);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeSelf)
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }
    }

    private void DisplayNextRound(int round)
    {
        roundNumber.text = round.ToString();
    }

    public void NextWave(int round)
    {
        for (var x = 0; x < round; x++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemySpawned;

            if (PhotonNetwork.InRoom)
            {
                enemySpawned = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                enemySpawned = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject;
            }
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void EndGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        Cursor.lockState = CursorLockMode.None;
        endScreen.SetActive(true);
        roundsSurvived.text = round.ToString();
    }

    public void Restart()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 0;
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        pauseMenu.SetActive(true);
    }

    public void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        pauseMenu.SetActive(false);
    }

    public void BackToMainMenu()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        blackScreenAnimator.SetTrigger("FadeIn");
        Invoke("LoadMainMenuScene", 0.4f);
    }

    public void LoadMainMenuScene()
    {
        AudioListener.volume = 1;
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (photonView.IsMine)
        {
            if(changedProps["currentRound"] != null)
            {
                DisplayNextRound((int)changedProps["currentRound"]);
            }
        }
    }

}
