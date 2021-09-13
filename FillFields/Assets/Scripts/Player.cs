using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private LevelManager lvl;
    private UISystem ui;

    private AudioSource audioSource;
    private Animator playerAnim;

    private int _posPlatform;

    private int movementDistance;
    private bool moveBack;

    [HideInInspector]public int currentMove;

    //MobileControl
    float startPositionX, deltaX, deltaAbsX;
    float startPositionY, deltaY, deltaAbsY;
    //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    List<MovementHistory> movementHistory = new List<MovementHistory>();
    MovementHistory lastMovement;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MovePlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MovePlayer(2);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MovePlayer(3);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MovePlayer(4);
        }

        MobileControl();
        void MobileControl()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        startPositionX = touch.position.x;
                        startPositionY = touch.position.y;
                        deltaX = 0;
                        deltaY = 0;
                        break;
                    case TouchPhase.Moved:
                        deltaX = startPositionX - touch.position.x;
                        deltaY = startPositionY - touch.position.y;

                        deltaX = deltaX / Screen.dpi;
                        deltaY = deltaY / Screen.dpi;

                        deltaAbsX = Mathf.Abs(deltaX);
                        deltaAbsY = Mathf.Abs(deltaY);

                        if (deltaAbsX > deltaAbsY)
                        {
                            if (deltaX > 0.15 && startPositionX != 0)
                            {
                                MovePlayer(3);

                                startPositionX = 0;
                                deltaX = 0;
                            }
                            if (deltaX < -0.15 && startPositionX != 0)
                            {
                                MovePlayer(4);

                                startPositionX = 0;
                                deltaX = 0;
                            }
                        }
                        else if (deltaAbsY > deltaAbsX)
                        {
                            if (deltaY > 0.15 && startPositionY != 0)
                            {
                                MovePlayer(2);

                                startPositionY = 0;
                                deltaY = 0;
                            }
                            if (deltaY < -0.15 && startPositionY != 0)
                            {
                                MovePlayer(1);

                                startPositionY = 0;
                                deltaY = 0;
                            }
                        }
                        break;
                    case TouchPhase.Ended:
                        startPositionY = 0;
                        deltaY = 0;
                        break;
                }
            }
        }
    }

    void MovePlayer(int directionID)
    {
        if (ui.isPaused) { ui.PauseGame(false); }
        if (!lvl.PlayerCanMove) { return; }

        if (lvl.Hint)
        {
            if (lvl.GetHintDirectionID(currentMove) != directionID)
            {
                return;
            }
        }

        movementDistance = GetMovementDistance();

        moveBack = false;
        if (movementDistance > 1)
        {
            lvl.PlayerCanMove = false;
            currentMove++;

            PlaySwipeSound();

            lastMovement = new MovementHistory(directionID, movementDistance);
            lastMovement.newBlockPos = new int[movementDistance];

            movementHistory.Add(lastMovement);
            lvl.ChangeMovementLimit(-1);

            switch (directionID)
            {
                case 1:
                    MovePlayerUp();
                    break;
                case 2:
                    MovePlayerDown();
                    break;
                case 3:
                    MovePlayerLeft();
                    break;
                case 4:
                    MovePlayerRight();
                    break;
            }
        }

        int GetMovementDistance()
        {
            int movementDistance = 0;
            int posPlatformExtra = PosPlatform;

            switch (directionID)
            {
                case 1:
                    while (lvl.platformValue[posPlatformExtra - 18] != 0)
                    {
                        movementDistance++;
                        posPlatformExtra -= 18;
                    }
                    break;
                case 2:
                    while (lvl.platformValue[posPlatformExtra + 18] != 0)
                    {
                        movementDistance++;
                        posPlatformExtra += 18;
                    }
                    break;
                case 3:
                    while (lvl.platformValue[posPlatformExtra - 1] != 0)
                    {
                        movementDistance++;
                        posPlatformExtra -= 1;
                    }
                    break;
                case 4:
                    while (lvl.platformValue[posPlatformExtra + 1] != 0)
                    {
                        movementDistance++;
                        posPlatformExtra += 1;
                    }
                    break;
            }

            return (movementDistance + 1);
        }
    }

    public void MovePlayerBack()
    {
        //If the player can't move or if the player hasn't moved before.
        if (!lvl.PlayerCanMove) { return; }

        //Stop the movement until that movement is complete.
        lvl.PlayerCanMove = false;

        currentMove--;

        PlaySwipeSound();

        moveBack = true;

        //Decrease the movement back limit.
        lvl.gameSystem.ChangeMovementBackLimit(-1);
        //Increase the movement limit.
        lvl.ChangeMovementLimit(1);

        lastMovement = movementHistory[movementHistory.Count - 1];
        movementDistance = lastMovement.MovementDistance;
        int directionID = lastMovement.MovementDirection;
        switch (directionID)
        {
            case 1:
                MovePlayerDown();
                break;
            case 2:
                MovePlayerUp();
                break;
            case 3:
                MovePlayerRight();
                break;
            case 4:
                MovePlayerLeft();
                break;
        }

        movementHistory.RemoveAt(movementHistory.Count - 1);
    }

    private void MovePlayerUp()
    {
        //If the current movement isn't a back movement.
        if (!moveBack)
        {
            if (lvl.platformValue[PosPlatform] == 1)
            {
                lastMovement.newBlockPos[lastMovement.indexPos] = PosPlatform;
                lastMovement.indexPos++;
                ShowBlock(PosPlatform);
            }
        }
        //If the current movement is a back movement.
        else
        {
            for(int i = 0; i < lastMovement.newBlockPos.Length; i++)
            {
                if (lastMovement.newBlockPos[i] == PosPlatform)
                {
                    ShowBlock(PosPlatform, false);
                }
            }
        }

        //Decrease the movement distance until the end of the movement.
        movementDistance--;
        //Finish the movement if the movement distance is zero.
        if (movementDistance == 0)
        {
            FinishPlayerMovement();
            return;
        }

        PosPlatform -= 18;
        PlayAnimation("MovePlayerUp");
    }
    private void MovePlayerDown()
    {
        //If the current movement isn't a back movement.
        if (!moveBack)
        {
            if (lvl.platformValue[PosPlatform] == 1)
            {
                lastMovement.newBlockPos[lastMovement.indexPos] = PosPlatform;
                lastMovement.indexPos++;
                ShowBlock(PosPlatform);
            }
        }
        //If the current movement is a back movement.
        else
        {
            for (int i = 0; i < lastMovement.newBlockPos.Length; i++)
            {
                if (lastMovement.newBlockPos[i] == PosPlatform)
                {
                    ShowBlock(PosPlatform, false);
                }
            }
        }

        //Decrease the movement distance until the end of the movement.
        movementDistance--;
        //Finish the movement if the movement distance is zero.
        if (movementDistance == 0)
        {
            FinishPlayerMovement();
            return;
        }

        PosPlatform += 18;
        PlayAnimation("MovePlayerDown");
    }
    private void MovePlayerLeft()
    {
        //If the current movement isn't a back movement.
        if (!moveBack)
        {
            if (lvl.platformValue[PosPlatform] == 1)
            {
                lastMovement.newBlockPos[lastMovement.indexPos] = PosPlatform;
                lastMovement.indexPos++;
                ShowBlock(PosPlatform);
            }
        }
        //If the current movement is a back movement.
        else
        {
            for (int i = 0; i < lastMovement.newBlockPos.Length; i++)
            {
                if (lastMovement.newBlockPos[i] == PosPlatform)
                {
                    ShowBlock(PosPlatform, false);
                }
            }
        }

        //Decrease the movement distance until the end of the movement.
        movementDistance--;
        //Finish the movement if the movement distance is zero.
        if (movementDistance == 0)
        {
            FinishPlayerMovement();
            return;
        }

        PosPlatform -= 1;
        PlayAnimation("MovePlayerLeft");
    }
    private void MovePlayerRight()
    {
        //If the current movement isn't a back movement.
        if (!moveBack)
        {
            if (lvl.platformValue[PosPlatform] == 1)
            {
                lastMovement.newBlockPos[lastMovement.indexPos] = PosPlatform;
                lastMovement.indexPos++;

                ShowBlock(PosPlatform);
            }
        }
        //If the current movement is a back movement.
        else
        {
            for (int i = 0; i < lastMovement.newBlockPos.Length; i++)
            {
                if (lastMovement.newBlockPos[i] == PosPlatform)
                {
                    ShowBlock(PosPlatform, false);
                }
            }
        }

        //Decrease the movement distance until the end of the movement.
        movementDistance--;
        //Finish the movement if the movement distance is zero.
        if (movementDistance == 0)
        {
            FinishPlayerMovement();
            return;
        }

        PosPlatform += 1;
        PlayAnimation("MovePlayerRight");
    }

    private void ShowBlock(int posPlatform, bool show = true)
    {
        if (show)
        {
            if (lvl.platformValue[posPlatform] != 0)
            {
                lvl.ShowBlock(posPlatform);
            }
        }
        else {
            if (lastMovement.CheckNewBlock(posPlatform))
            {
                lvl.ShowBlock(posPlatform, false);
            }
        }
    }

    //Execute after each movement
    private void FinishPlayerMovement()
    {
        if (lvl.Hint && lvl.GetHintDirectionID(currentMove) != 0)
        {
            ui.RefreshHintDirection(lvl.GetHintDirectionID(currentMove));
        }

        StartCoroutine(wait());

        IEnumerator wait()
        {
            yield return new WaitForSeconds(0.02f);
            //Rounds the player's position after the move.
            RoundPosition();

            if (lvl.CheckFinish())
            {
                lvl.FinishLevel();
            }
            else
            {
                if (lvl.movementLimit == 0)
                {
                    ui.ShowLevelFailed(true);
                }
            }
        }

        //Rounds the player's position after move.
        void RoundPosition()
        {
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }
    }

    private void PlayAnimation(string animationName)
    {
        playerAnim.Play(animationName);
    }

    //Execute after initialize the player object.
    public void Initialize(int posPlatform, int posX, int posY, LevelManager lvl, UISystem ui, AudioSource audioSource)
    {
        transform.name = "Player";
        transform.position = new Vector2(posX, posY);
        PosPlatform = posPlatform;
        this.lvl = lvl;
        this.ui = ui;

        this.audioSource = audioSource;
        playerAnim = GetComponent<Animator>();
    }

    private void PlaySwipeSound()
    {
        audioSource.Play();
    }

    public int PosPlatform
    {
        get { return _posPlatform; }
        set { _posPlatform = value; }
    }
}

public class MovementHistory
{
    int _movementDirection; //1-up,2-down,3-left,4-right
    int _movementDistance;
    public int[] newBlockPos;
    public int indexPos;
    public MovementHistory(int movementValue, int movementDistance)
    {
        MovementDirection = movementValue;
        MovementDistance = movementDistance;
    }
    
    public bool CheckNewBlock(int posPlatform)
    {
        for(int i = 0; i < newBlockPos.Length; i++)
        {
            if (posPlatform == newBlockPos[i])
            {
                return true;
            }
        }
        return false;
    }

    public int MovementDirection
    {
        get { return _movementDirection; }
        set { _movementDirection = value; }
    }
    public int MovementDistance
    {
        get { return _movementDistance; }
        set { _movementDistance = value; }
    }
}