using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Player : MonoBehaviour {
    [HideInInspector] public int currentMovement;
    [HideInInspector] public Hint hint;

    [SerializeField] private LevelManager lvl;
    [SerializeField] private UILevel ui;

    private Animator playerAnim;

    private int positionIndex;

    private int movementDistance;
    private bool moveBack;

    //MobileControl
    private float startPositionX, deltaX, deltaAbsX;
    private float startPositionY, deltaY, deltaAbsY;
    //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    private List<MovementHistory> movementHistory = new List<MovementHistory>();
    private MovementHistory lastMovement;

    public void Initialize(LevelManager lvl, UILevel ui, int position) {
        this.lvl = lvl;
        this.ui = ui;

        this.positionIndex = position;
    }

    private void Awake() {
        playerAnim = GetComponent<Animator>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.W)) {
            PrepareMovement(MovementDirection.UP);
        }
        if(Input.GetKeyDown(KeyCode.S)) {
            PrepareMovement(MovementDirection.DOWN);
        }
        if(Input.GetKeyDown(KeyCode.A)) {
            PrepareMovement(MovementDirection.LEFT);
        }
        if(Input.GetKeyDown(KeyCode.D)) {
            PrepareMovement(MovementDirection.RIGHT);
        }

        MobileControl();
        void MobileControl() {
            if(Input.touchCount == 1) {
                Touch touch = Input.GetTouch(0);

                switch(touch.phase) {
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

                        if(deltaAbsX > deltaAbsY) {
                            if(deltaX > 0.15 && startPositionX != 0) {
                                PrepareMovement(MovementDirection.LEFT);

                                startPositionX = 0;
                                deltaX = 0;
                            }
                            if(deltaX < -0.15 && startPositionX != 0) {
                                PrepareMovement(MovementDirection.RIGHT);

                                startPositionX = 0;
                                deltaX = 0;
                            }
                        } else if(deltaAbsY > deltaAbsX) {
                            if(deltaY > 0.15 && startPositionY != 0) {
                                PrepareMovement(MovementDirection.DOWN);

                                startPositionY = 0;
                                deltaY = 0;
                            }
                            if(deltaY < -0.15 && startPositionY != 0) {
                                PrepareMovement(MovementDirection.UP);

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

    private void PrepareMovement(MovementDirection direction) {
        if(ui.pause.isPaused) { ui.pause.PauseGame(false); }
        if(!lvl.PlayerCanMove) { return; }

        movementDistance = GetMovementDistance();
        if(movementDistance <= 1) { return; }

        //If the hint is enable and the player doesn't make the move that the hint shows.
        if(hint.isEnable) {
            if(hint.GetDirection(currentMovement) != direction) {
                return;
            }
        }

        moveBack = false;
        lvl.PlayerCanMove = false;
        currentMovement++;

        PlaySwipeSound();

        lastMovement = ScriptableObject.CreateInstance<MovementHistory>();
        lastMovement.Initialize(direction, movementDistance);

        movementHistory.Add(lastMovement);
        lvl.ChangeMovementLimit(-1);

        switch(direction) {
            case MovementDirection.UP:
                MovePlayerUp();
                break;
            case MovementDirection.RIGHT:
                MovePlayerRight();
                break;
            case MovementDirection.DOWN:
                MovePlayerDown();
                break;
            case MovementDirection.LEFT:
                MovePlayerLeft();
                break;
        }

        int GetMovementDistance() {
            int movementDistance = 0;
            int posPlatformExtra = Position;

            switch(direction) {
                case MovementDirection.UP:
                    while(PositionIsUnlocked(posPlatformExtra - 18)) {
                        movementDistance++;
                        posPlatformExtra -= 18;
                    }
                    break;
                case MovementDirection.DOWN:
                    while(PositionIsUnlocked(posPlatformExtra + 18)) {
                        movementDistance++;
                        posPlatformExtra += 18;
                    }
                    break;
                case MovementDirection.LEFT:
                    while(PositionIsUnlocked(posPlatformExtra - 1)) {
                        movementDistance++;
                        posPlatformExtra -= 1;
                    }
                    break;
                case MovementDirection.RIGHT:
                    while(PositionIsUnlocked(posPlatformExtra + 1)) {
                        movementDistance++;
                        posPlatformExtra += 1;
                    }
                    break;
            }

            return movementDistance + 1;

            bool PositionIsUnlocked(int position) {
                try {
                    if(lvl.platform.platforms[position].status != PlatformStatus.LOCKED) {
                        return true;
                    }
                } catch(IndexOutOfRangeException) {
                    return false;
                } catch(NullReferenceException) {
                    return false;
                }

                return false;
            }
        }
    }

    public void PrepareMovementBack() {
        //If the player can't move or if the player hasn't moved before.
        if(!lvl.PlayerCanMove) { return; }

        //Stop the movement until that movement is complete.
        lvl.PlayerCanMove = false;

        currentMovement--;
        moveBack = true;

        PlaySwipeSound();

        //Decrease the movement back limit.
        lvl.ChangeMovementBackLimit(-1);
        //Increase the movement limit.
        lvl.ChangeMovementLimit(1);

        lastMovement = movementHistory[movementHistory.Count - 1];

        movementDistance = lastMovement.Distance;
        MovementDirection direction = lastMovement.Direction;

        switch(direction) {
            case MovementDirection.DOWN:
                MovePlayerUp();
                break;
            case MovementDirection.UP:
                MovePlayerDown();
                break;
            case MovementDirection.RIGHT:
                MovePlayerLeft();
                break;
            case MovementDirection.LEFT:
                MovePlayerRight();
                break;
        }

        movementHistory.RemoveAt(movementHistory.Count - 1);
    }

    private void MovePlayer(int position, string animationName) {
        //If the current movement isn't a back movement.
        if(!moveBack) {
            if(lvl.platform.platforms[Position].status == PlatformStatus.UNLOCKED) {
                lastMovement.newBlockPos[lastMovement.indexPos] = Position;
                lastMovement.indexPos++;
                ShowFillBlock(Position);
            }
        }
        //If the current movement is a back movement.
        else {
            for(int i = 0;i < lastMovement.newBlockPos.Length;i++) {
                if(lastMovement.newBlockPos[i] == Position) {
                    ShowFillBlock(Position, false);
                }
            }
        }

        //Decrease the movement distance until the end of the movement.
        movementDistance--;
        //Finish the movement if the movement distance is zero.
        if(movementDistance == 0) {
            FinishPlayerMovement();
            return;
        }

        Position += position;
        PlayMovementAnimation(animationName);
    }

    private void MovePlayerUp() {
        MovePlayer(-18, "MovePlayerUp");
    }
    private void MovePlayerDown() {
        MovePlayer(18, "MovePlayerDown");
    }
    private void MovePlayerLeft() {
        MovePlayer(-1, "MovePlayerLeft");
    }
    private void MovePlayerRight() {
        MovePlayer(1, "MovePlayerRight");
    }

    private void ShowFillBlock(int posPlatform, bool show = true) {
        if(show) {
            if(lvl.platform.platforms[posPlatform].status != PlatformStatus.LOCKED) {
                lvl.platform.ShowFillBlock(posPlatform);
            }
        } else {
            if(lastMovement.CheckNewBlock(posPlatform)) {
                lvl.platform.ShowFillBlock(posPlatform, false);
            }
        }
    }

    //Execute after each movement
    private void FinishPlayerMovement() {
        if(hint.isEnable) {
            ui.hint.ChangeDirection((int)hint.GetDirection(currentMovement));
        }

        StartCoroutine(wait());

        IEnumerator wait() {
            yield return new WaitForSeconds(0.02f);

            if(!lvl.IsFinished()) {
                if(lvl.MovementLimit == 0) {
                    ui.levelFailed.ShowPanel();
                }

                //Rounds the player's position after the move.
                RoundPosition();
            }
        }

        //Rounds the player's position after move.
        void RoundPosition() {
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }
    }

    private void PlayMovementAnimation(string animationName) {
        playerAnim.Play(animationName);
    }
    private void PlaySwipeSound() {
        SoundSystem.GetInstance().PlaySound("playerMovement");
    }

    public int Position {
        get { return positionIndex; }
        set { positionIndex = value; }
    }
}