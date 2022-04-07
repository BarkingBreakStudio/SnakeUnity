using NorLib.Timer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player2d : MonoBehaviour
{

    public Vector2Int playFieldSize = new(11, 11);

    private VisualElement rve;

    Queue<Vector2Int> bodyParts = new();
    Vector2Int headPart;
    VisualElement[,] tiles;
    GenericTimer movePlayerTimer;


    Vector2Int moveDir = Vector2Int.right;
    Vector2Int lastMoveDir = Vector2Int.right;

    private const float startMoveIntervall = 0.5f;

    Vector2 oldInputAxis;

    /*Colors*/
    Color headColor =  new Color(112 / 255f, 143 / 255f, 249 / 255f);
    Color bodyColor = new Color(80 / 255f, 118 / 255f, 249 / 255f);


    [Header("Sound")]
    public List<AudioClip> DirChangeSoundUpLeftRightDown = new();
    public AudioClip SlurpingClip;
    public AudioClip GameOverClip;
    public NorLib.Sound.AudioCueEventChannelSO sfxChannel;

    private void Awake()
    {
        movePlayerTimer = GenericTimer.CreateInstance(gameObject, MovePlayer, "MovePlayerTimer", startMoveIntervall);
        movePlayerTimer.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.StateChanged += GameManager_StateChanged;
    }

    private void GameManager_StateChanged(GameManager.eGameStarte newState)
    {
        movePlayerTimer.enabled = (newState == GameManager.eGameStarte.Playing);

        if (newState == GameManager.eGameStarte.Playing && oldState == GameManager.eGameStarte.StartScreen)
        {
            BuildSnake();
            spawnNewFruit();

            moveDir = Vector2Int.right;
            movePlayerTimer.Interval = startMoveIntervall;
        }

        oldState = newState;
    }

    private void PlayMoveDirChnageSound()
    {
        List<Vector2> listOfDirs = new() { Vector2Int.up, Vector2Int.left, Vector2Int.right, Vector2Int.down };
        int idx = listOfDirs.IndexOf(moveDir);
        if (idx >= 0)
        {
            sfxChannel?.Play(DirChangeSoundUpLeftRightDown[idx]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (oldState != GameManager.eGameStarte.Playing)
        {
            return;
        }


        Vector2 curInputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector2Int playerInput = new Vector2Int(0, 0);
        if (curInputAxis.x > 0.5f && oldInputAxis.x <= 0.5f)
        {
            playerInput = Vector2Int.right;
        }
        if (curInputAxis.x < -0.5f && oldInputAxis.x >= -0.5f)
        {
            playerInput = Vector2Int.left;
        }
        if (curInputAxis.y > 0.5f && oldInputAxis.y <= 0.5f)
        {
            playerInput = Vector2Int.up;
        }
        if (curInputAxis.y < -0.5f && oldInputAxis.y >= -0.5f)
        {
            playerInput = Vector2Int.down;
        }
        oldInputAxis = curInputAxis;




        if (playerInput == Vector2Int.right)
        {
            if (moveDir != Vector2Int.right && lastMoveDir != -Vector2Int.right)
            {
                moveDir = Vector2Int.right;
                PlayMoveDirChnageSound();
            }
        }
        if (playerInput == Vector2Int.left)
        {
            if (moveDir != Vector2Int.left && lastMoveDir != -Vector2Int.left)
            {
                moveDir = Vector2Int.left;
                PlayMoveDirChnageSound();
            }
        }
        if (playerInput == Vector2Int.up)
        {
            if (moveDir != Vector2Int.up && lastMoveDir != -Vector2Int.up)
            {
                moveDir = Vector2Int.up;
                PlayMoveDirChnageSound();
            }
        }
        if (playerInput == Vector2Int.down)
        {
            if (moveDir != Vector2Int.down && lastMoveDir != -Vector2Int.down)
            {
                moveDir = Vector2Int.down;
                PlayMoveDirChnageSound();
            }
        }
    }

    public void OnEnable()
    {
        rve = GetComponent<UIDocument>().rootVisualElement;
        VisualElement gameField = rve.Q("GameField");

        /*remove all childs*/
        gameField.Clear();

        tiles = new VisualElement[playFieldSize.x, playFieldSize.y];
        for (int x = 0; x < playFieldSize.x + 2; x++)
        {
            VisualElement veColumn = new VisualElement();
            veColumn.AddToClassList("PlayfieldColumn");

            for(int y = 0; y < playFieldSize.y + 2; y++)
            {
                VisualElement veTile = new VisualElement();
                veTile.AddToClassList("PlayfieldTile");
                if(x == 0 || y == 0 || x == playFieldSize.x+1 || y == playFieldSize.y + 1)
                {
                    veTile.AddToClassList("Wall");
                }
                else if((y + x) % 2 == 0)
                {
                    veTile.AddToClassList("Ground1");
                    tiles[x-1,y-1] = veTile;
                }
                else
                {
                    veTile.AddToClassList("Ground2");
                    tiles[x - 1, y - 1] = veTile;
                }
                veColumn.Add(veTile);
            }

            gameField.Add(veColumn);
        }
    }




    private void BuildSnake()
    {
        tiles[headPart.x, headPart.y].style.backgroundColor = StyleKeyword.Null;
        headPart = new Vector2Int(playFieldSize.x / 2, playFieldSize.y / 2);
        tiles[headPart.x, headPart.y].style.backgroundColor = headColor;


        while (bodyParts.Count > 0)
        {
            Vector2Int oldBodyPart = bodyParts.Dequeue();
            tiles[oldBodyPart.x, oldBodyPart.y].style.backgroundColor = StyleKeyword.Null;
        }

        for (int i = 2; i > 0; i--)
        {
            Vector2Int newBodyPart = Vector2Int.left * i + headPart;
            bodyParts.Enqueue(newBodyPart);
            tiles[newBodyPart.x, newBodyPart.y].style.backgroundColor = bodyColor;
        }
    }

    Vector2Int fruitPos;
    private GameManager.eGameStarte oldState;

    private void spawnNewFruit()
    {
        tiles[fruitPos.x, fruitPos.y].Clear();

        VisualElement fruit = new VisualElement();
        fruit.AddToClassList("Fruit");

        List<Vector2Int> possibleLocations = GetEmptyLocations();
        fruitPos = possibleLocations[UnityEngine.Random.Range(0, possibleLocations.Count)];
        tiles[fruitPos.x, fruitPos.y].Add(fruit);

    }

    private void MovePlayer(GenericTimer gt)
    {
        Vector2Int oldHeadPos = headPart;
        lastMoveDir = moveDir;
        headPart += moveDir;
        if (headPart.x >= playFieldSize.x)
        {
            headPart.x = 0;
        }
        if (headPart.x < 0)
        {
            headPart.x = playFieldSize.x - 1;
        }
        if (headPart.y >= playFieldSize.y)
        {
            headPart.y = 0;
        }
        if (headPart.y < 0)
        {
            headPart.y = playFieldSize.y-1;
        }

        bodyParts.Enqueue(oldHeadPos);
        tiles[oldHeadPos.x, oldHeadPos.y].style.backgroundColor = bodyColor;
        
        if (headPart == fruitPos)
        {
            gt.Interval = (gt.Interval - 0.1f) * 0.95f + 0.1f;
            sfxChannel.Play(SlurpingClip);
            GameManager.IncreaseScore();
            spawnNewFruit();
        }
        else
        {
            var lastPart = bodyParts.Dequeue();
            tiles[lastPart.x, lastPart.y].style.backgroundColor = StyleKeyword.Null; /*sets the color back to defaul from USS*/
        }

        tiles[headPart.x, headPart.y].style.backgroundColor = headColor;

        CheckIfPlayerLost();
    }


    private List<Vector2Int> GetEmptyLocations()
    {
        List<Vector2Int> possibleLocations = new();
        for (int x = 0; x < playFieldSize.x; x++)
        {
            for (int y = 0; y < playFieldSize.y; y++)
            {
                possibleLocations.Add(new Vector2Int(x, y));
            }
        }

        foreach (var bodyPart in bodyParts)
        {
            possibleLocations.Remove(bodyPart);
        }
        possibleLocations.Remove(headPart);

        return possibleLocations;
    }

    private void CheckIfPlayerLost()
    {
        if (bodyParts.Contains(headPart))
        {
            StartCoroutine(PlayLostGame());
        }
    }

    IEnumerator PlayLostGame()
    {
        movePlayerTimer.enabled = false;
        sfxChannel.Play(GameOverClip);
        yield return new WaitForSeconds(1);
        GameManager.PlayerLost();
    }

}
