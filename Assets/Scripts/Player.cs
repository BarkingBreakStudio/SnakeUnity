using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Transform BodyPartPrefab;
    public Transform HeadPartPrefab;
    public FruitSystem FruitSys;

    public Vector2Int PlayFieldHalfSize = new(5, 5);
    public Transform WallPrefab;


    Queue<BodyPart> bodyParts = new();
    BodyPart headPart;

    Vector2Int moveDir = Vector2Int.up;
    Vector2Int? lastMoveDir = null;
    Vector2Int headPos;

    GenericTimer movePlayerTimer;

    public List<Transform> GroundPrefabs = new();
    private List<Transform> grounds = new();

    [Header("Sound")]
    public List<AudioClip> DirChangeSoundUpLeftRightDown = new();
    public AudioClip SlurpingClip;
    public AudioClip GameOverClip;
    public NorLib.Sound.AudioCueEventChannelSO sfxChannel;

    private const float startMoveIntervall = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        movePlayerTimer = GenericTimer.CreateInstance(gameObject, MovePlayer, "MovePlayerTimer", startMoveIntervall);
        movePlayerTimer.enabled = false;

        GameManager.StateChanged += GameManager_StateChanged;
    }

    private void BuildSnake()
    {
        while(bodyParts.Count > 0)
        {
            Destroy(bodyParts.Dequeue().Go.gameObject);
        }

        for (int i = 0; i < 2; i++)
        {
            bodyParts.Enqueue(new BodyPart() { Go = Instantiate<Transform>(BodyPartPrefab, this.transform) });
        }

        if (headPart is not null && headPart.Go)
        {
            Destroy(headPart.Go.gameObject);
        }
        headPart = new BodyPart() { Go = Instantiate<Transform>(HeadPartPrefab, this.transform) };

        movePlayerTimer.Interval = startMoveIntervall;
    }

    GameManager.eGameStarte oldState;
    private void GameManager_StateChanged(GameManager.eGameStarte newState)
    {
     
        movePlayerTimer.enabled = (newState == GameManager.eGameStarte.Playing);

        if(newState == GameManager.eGameStarte.Playing && oldState == GameManager.eGameStarte.StartScreen)
        {
            FruitSys.DestroyFruit();
            FruitSys.ConsumeFruit(GetEmptyLocations());
            headPos = new Vector2Int(0, 0);

            BuildSnake();
            BuildWalls();
            BuildGround();

            moveDir = Vector2Int.up;
        }

        oldState = newState;
    }

    Vector2 oldInputAxis;

    // Update is called once per frame
    void Update()
    {
        if(oldState != GameManager.eGameStarte.Playing)
        {
            return;
        }

        Vector2 curInputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector2Int playerInput = new Vector2Int(0, 0);
        if(curInputAxis.x > 0.5f && oldInputAxis.x <= 0.5f)
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
            if (lastMoveDir is not null && moveDir != Vector2Int.right && lastMoveDir != -Vector2Int.right)
            {
                moveDir = Vector2Int.right;
                PlayMoveDirChnageSound();
            }
        }
        if (playerInput == Vector2Int.left)
        {
            if (lastMoveDir is not null && moveDir != Vector2Int.left && lastMoveDir != -Vector2Int.left)
            {
                moveDir = Vector2Int.left;
                PlayMoveDirChnageSound();
            }
        }
        if (playerInput == Vector2Int.up)
        {
            if (lastMoveDir is not null && moveDir != Vector2Int.up && lastMoveDir != -Vector2Int.up)
            {
                moveDir = Vector2Int.up;
                PlayMoveDirChnageSound();
            }
        }
        if (playerInput == Vector2Int.down)
        {
            if (lastMoveDir is not null && moveDir != Vector2Int.down && lastMoveDir != -Vector2Int.down)
            {
                moveDir = Vector2Int.down;
                PlayMoveDirChnageSound();
            }
        }

    }

    void PlayMoveDirChnageSound()
    {
        List<Vector2> listOfDirs = new() { Vector2Int.up , Vector2Int.left, Vector2Int.right, Vector2Int.down };
        int idx = listOfDirs.IndexOf(moveDir);
        if ( idx >= 0)
        {
            sfxChannel?.Play(DirChangeSoundUpLeftRightDown[idx]);
        }
    }

    

    private void MovePlayer(GenericTimer gt)
    {
        /*
        transform.position += (Vector3.right * Input.GetAxisRaw("Horizontal")
                                + Vector3.forward * Input.GetAxisRaw("Vertical"));
        */

        Vector2Int oldHeadPos = headPos;
        lastMoveDir = moveDir;
        headPos += (Vector2Int)(moveDir);
        if (headPos.x > PlayFieldHalfSize.x)
        {
            headPos.x = -PlayFieldHalfSize.x;
        }
        if (headPos.x < -PlayFieldHalfSize.x)
        {
            headPos.x = PlayFieldHalfSize.x;
        }
        if (headPos.y > PlayFieldHalfSize.y)
        {
            headPos.y = -PlayFieldHalfSize.y;
        }
        if (headPos.y < -PlayFieldHalfSize.y)
        {
            headPos.y = PlayFieldHalfSize.y;
        }

        headPart.Position = headPos;

        if (Vector3.Distance(headPart.Go.position, FruitSys.GetFruitPosition()) < 0.3f)
        {
            BodyPart newPart = new BodyPart() { Go = Instantiate<Transform>(BodyPartPrefab, this.transform) };
            newPart.Position = oldHeadPos;
            bodyParts.Enqueue(newPart);
            FruitSys.ConsumeFruit(GetEmptyLocations());
            gt.Interval = (gt.Interval - 0.1f) * 0.95f + 0.1f;
            sfxChannel.Play(SlurpingClip);
            GameManager.IncreaseScore();
        }
        else
        {
            var lastPart = bodyParts.Dequeue();
            lastPart.Position = oldHeadPos;
            bodyParts.Enqueue(lastPart);
        }

        CheckIfPlayerLost();
    }

    private List<Vector2Int> GetEmptyLocations()
    {
        List<Vector2Int> possibleLocations = new();
        for (int x = -PlayFieldHalfSize.x; x <= PlayFieldHalfSize.x; x++)
        {
            for (int y = -PlayFieldHalfSize.y; y <= PlayFieldHalfSize.x; y++)
            {
                possibleLocations.Add(new Vector2Int(x, y));
            }
        }

        foreach (var part in bodyParts)
        {
            possibleLocations.Remove(part.Position);
        }
        possibleLocations.Remove(headPos);

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

    class BodyPart
    {
        Vector2Int _position;
        public Vector2Int Position
        {
            set
            {
                _position = value;
                Go.position = (Vector3.right * value.x + Vector3.forward * value.y);
            }
            get
            {
                return _position;
            }
        }
        public Transform Go;

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj is null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                BodyPart p = (BodyPart)obj;
                return (Position == p.Position);
            }

        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

    }

    private List<Transform> Walls = new();

    void BuildWalls()
    {
        foreach (var wall in Walls)
        {
            Destroy(wall.gameObject);
        }
        Walls.Clear();

        for (int x = -PlayFieldHalfSize.x - 1; x <= PlayFieldHalfSize.x + 1; x++)
        {
            Transform newTF1 = Instantiate<Transform>(WallPrefab, new Vector3(x, 0, PlayFieldHalfSize.y + 1), Quaternion.identity, transform);
            Transform newTF2 = Instantiate<Transform>(WallPrefab, new Vector3(x, 0, -PlayFieldHalfSize.y - 1), Quaternion.identity, transform);
            Walls.Add(newTF1);
            Walls.Add(newTF2);
        }

        for (int y = -PlayFieldHalfSize.x; y <= PlayFieldHalfSize.x; y++)
        {
            Transform newTF1 = Instantiate<Transform>(WallPrefab, new Vector3(PlayFieldHalfSize.x + 1, 0, y), Quaternion.identity, transform);
            Transform newTF2 = Instantiate<Transform>(WallPrefab, new Vector3(-PlayFieldHalfSize.x - 1, 0, y), Quaternion.identity, transform);
            Walls.Add(newTF1);
            Walls.Add(newTF2);
        }
    }

    void BuildGround()
    {
        foreach (var ground in grounds)
        {
            Destroy(ground.gameObject);
        }
        grounds.Clear();

        for (int x = -PlayFieldHalfSize.x - 1; x <= PlayFieldHalfSize.x + 1; x++)
        {
            for (int y = -PlayFieldHalfSize.x; y <= PlayFieldHalfSize.x; y++)
            {
                Transform newGround = Instantiate<Transform>(GroundPrefabs[Math.Abs(x + y) % 2], new Vector3(x, -1, y), Quaternion.identity, transform);
                grounds.Add(newGround);
            }
        }
    }


}
