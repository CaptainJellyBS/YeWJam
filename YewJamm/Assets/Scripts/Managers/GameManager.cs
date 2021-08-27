using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    AudioSource auds;

    [Header("Variables")]
    public Material[] colorMats;

    public GameObject blockPref;
    public Text scoreText, timerText;
    public AudioClip yay, vict;
    public GameObject tutPan;

    [Header("GameOver")]
    public GameObject gameOverPanel;
    public Text[] gameOverTexts;
    public Text gameOverScoreText;
    public GameObject[] buttons;

    public int amount;
    bool spawning;
    bool tutorial;
    AudioSource music;

    int timer;
    public int Timer
    {
        get
        {
            return timer;
        }
        set
        {
            timer = value;
            timerText.text = (timer / 60).ToString("D2") + ":" + (timer % 60).ToString("D2");
            //make timer red and music faster when timer is under 10 seconds
            if (timer <= 10)
            {
                if (music != null) { music.pitch = 1.25f; }
                timerText.color = Color.red;
            }
            else
            {
                if (music != null) { music.pitch = 1.0f; }
                timerText.color = Color.black;
            }
        }
    }

    int score;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = score.ToString();
        }
    }

    Vector3[] possibleSpawns;
    float hOff = 2.75f, vOff = 1.0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        music = GameObject.FindGameObjectWithTag("Music")?.GetComponent<AudioSource>();
        auds = GetComponent<AudioSource>();
        spawning = false;
        amount = 0;
        Timer = 30;
        Score = 0;
        possibleSpawns = new Vector3[44];
        PlayerMovement.Instance.canMove = false;
        tutorial = true;
        tutPan.SetActive(false);

        int i = 0;
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -4; y <= 4; y++)
            {
                if (x == 0 && y == 0) { continue; }
                possibleSpawns[i] = new Vector3(x * hOff, y * vOff, 0);
                i++;
            }
        }
        //Cursor.visible = false;
        //SpawnBlocks();
        //StartCoroutine(TimerC());
    }

    public Material BlockColorToMat(BlockColor bc)
    {
        return colorMats[(int)bc];
    }

    public void SpawnBlocks()
    {
        amount = Mathf.Clamp(amount, 1, 8);
        Timer += 31;
        Utility.FisherYates(ref possibleSpawns); //Shuffle spawn positions

        for (int i = 0; i < amount; i++)
        {
            Block a = Instantiate(blockPref, possibleSpawns[2 * i], Quaternion.identity).GetComponent<Block>();
            a.dominant = true;
            a.myColor = (BlockColor)i;
            a.Init();

            Block b = Instantiate(blockPref, possibleSpawns[2 * i + 1], Quaternion.identity).GetComponent<Block>();
            b.dominant = false;
            b.myColor = (BlockColor)i;
            b.Init();
        }
    }

    IEnumerator TimerC()
    {
        while (Timer >= 0)
        {
            yield return new WaitForSeconds(1.0f);
            Timer--;
        }
        timerText.text = "game over";
        Die();
    }

    public void BlockDestroyed(BlockColor c)
    {
        if (tutorial)
        {
            tutorial = false;
            PlayerMovement.Instance.canMove = true;
            StartCoroutine(TimerC());
            StartCoroutine(ControlTutC());
        }

        scoreText.color = BlockColorToMat(c).color;
        StartCoroutine(BlockDestroyedC());
    }

    IEnumerator BlockDestroyedC()
    {
        Score++;
        auds.PlayOneShot(yay, 1.0f);
        yield return new WaitForSeconds(0.5f);

        scoreText.color = Color.black;

        if (FindObjectsOfType<Block>().Length < 1 && !spawning)
        {
            spawning = true;
            auds.PlayOneShot(vict, 0.8f);
            yield return new WaitForSeconds(0.5f);
            amount++;
            SpawnBlocks();
            spawning = false;
        }
    }

    IEnumerator ControlTutC()
    {
        tutPan.SetActive(true);
        while (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        tutPan.SetActive(false);
    }

    #region ManaBasic Functionality
    public void Die()
    {
        Time.timeScale = 0.0f;
        if (music != null) { music.pitch = 1.0f; }
        StartCoroutine(DieC());
    }

    IEnumerator DieC()
    {
        gameOverScoreText.text = Score.ToString();
        gameOverScoreText.color = Color.clear;
        
        foreach (Text tex in gameOverTexts)
        {
            tex.color = Color.clear;
        }
        foreach (GameObject b in buttons)
        {
            b.SetActive(false);
        }

        gameOverPanel.GetComponent<Image>().color = Color.clear;
        gameOverPanel.SetActive(true);

        float t = 0;
        while(t <= 5.0f)
        {
            gameOverPanel.GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, Mathf.Clamp(t/2, 0.0f, 1.0f));
            foreach (Text tex in gameOverTexts)
            {
                tex.color = Color.Lerp(Color.clear, Color.white, Mathf.Clamp(t-1.8f, 0.0f, 1.0f));
            }
            gameOverScoreText.color = Color.Lerp(Color.clear, Color.white, Mathf.Clamp(t-2.5f, 0.0f, 1.0f));
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        foreach(GameObject b in buttons)
        {
            b.SetActive(true);
        }

    }

    #endregion

    #region debug

    #endregion
}
