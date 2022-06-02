using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive = 0;
    public int round = 0;
    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;
    public Text roundNumber;
    public GameObject endScreen;
    public Text roundsSurvived;
    public GameObject pauseMenu;
    public Animator blackScreenAnimator;

    /*    public GameObject enemyPrefab;
        public Text roundNumber;
        public GameObject endScreen;
        public Text roundsSurvivedNumber;
        public Text enemiesKilledNumber;
        public int enemiesKilled;*/
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (enemiesAlive == 0)
        {
            round++;
            NextWave(round);
            roundNumber.text = round.ToString();
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

    public void NextWave(int round)
    {
        for (var x = 0; x < round; x++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemySpawned = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void EndGame()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        endScreen.SetActive(true);
        roundsSurvived.text = round.ToString();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 0;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        blackScreenAnimator.SetTrigger("FadeIn");
        Invoke("LoadMainMenuScene", 0.4f);
    }

    public void LoadMainMenuScene()
    {
        AudioListener.volume = 1;
        SceneManager.LoadScene(0);
    }
}
