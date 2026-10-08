using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using DnD.Service;
using DnD.Player;

public class PlayerMovement : MonoBehaviour
{
    public string horizontalMovementKey;
    public string horizontalNegMovementKey;
    public string verticalMovementKey;
    public string verticaNeglMovementKey;

    public static bool isMoving = true;

    public bool isMovingAlongX = false;
    public bool isMovingAlongY = false;

    public float movementSpeed;

    public PlayerView playerView;
    public ViewDistanceController viewDistanceController;
    public PlayerAnimation playerAnimation;
    public ObjectTransformer playerTransformer;

    private Vector3 prevPosition;
    private Vector3 genRequestDistance;

    public float genDistance;
    public bool isFlipped;
    public bool isRunning;

    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]

    private float prevXposition;
    public float doublePressTime = 1.0f;

    private float initialMovementSpeed;
    private float lastPressTime = -1f;
    private float lastPressRightTime = -1f;

    private float minInterval = 0.05f;
    private float lastSendTime;

    private void Start()
    {
        genRequestDistance = transform.position;
        prevPosition = transform.position;
        initialMovementSpeed = movementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (WorldData.tilesPopulated)
        {
            playerView.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClientRequestHandler.getNPCInstance();
        }

       float distance = Vector3.Distance(transform.position, prevPosition);
       genDistance = Vector3.Distance(transform.position, genRequestDistance);

       int verticalDirection = GetVerticalDirection(transform.position, prevPosition);
       controlDiagonalSpeed();
       if (isMovementApplied())
        {
            isMoving = true;
            checkAndMoveAlongX();
            checkAndMoveAlongY();
            
            //playerAnimation.walk(isFlipped);
            //playerAnimator.SetBool("isWalking", true);
            //playerAnimator.SetBool("isFlipped", isFlipped);
            if (distance > 1f && (Time.time - lastSendTime > minInterval))
            {
                isMoving = false;
                playerView.fetchAndUpdateTiles(transform.position, verticalDirection);
                prevPosition = transform.position;
                lastSendTime = Time.time;
            }

            if (genDistance > 10.0f)
            {
                //ClientRequestHandler.getTerrainData(transform.position.x, transform.position.y, viewDistanceController.viewDistance);
                genRequestDistance = transform.position;
            }
        } else
        {
            //playerAnimator.speed = 1.0f;
            playerAnimation.speed = 1.0f;
            //playerAnimation.idle();
            playerTransformer.idle();
            movementSpeed = initialMovementSpeed;
            isMovingAlongX = false;
            isMovingAlongY = false;
            isMoving = false;
            //playerAnimator.SetBool("isWalking", false);
        }
    }

    private void controlDiagonalSpeed()
    {
        if (isMovingAlongX && isMovingAlongY)
        {
            movementSpeed = initialMovementSpeed * 0.7f;
        } else
        {
            movementSpeed = initialMovementSpeed;
        }
    }

    private int GetVerticalDirection(Vector3 currentPosition, Vector3 prevPosition)
    {
        if (currentPosition.y > prevPosition.y)
            return -1; // moving up
        else if (currentPosition.y < prevPosition.y)
            return 1;  // moving down
        else
            return 0;  // no vertical movement
    }

    private bool isMovementApplied()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        } else
        {
            isRunning = false;
        }

        if (Input.GetKeyDown(horizontalMovementKey))
        {
            if (Time.time - lastPressTime <= doublePressTime)
            {
                lastPressTime = -1f;
                isFlipped = true;
            } else
            {
                lastPressTime = Time.time;
            }
            return true;
        } else if (Input.GetKeyDown(horizontalNegMovementKey))
        {
            if (Time.time - lastPressRightTime <= doublePressTime)
            {
                lastPressRightTime = -1f;
                isFlipped = false;
            } else
            {
                lastPressRightTime = Time.time;
            }
            return true;
        } else if (Input.GetKey(horizontalMovementKey) || Input.GetKey(horizontalNegMovementKey) || Input.GetKey(verticalMovementKey) || Input.GetKey(verticaNeglMovementKey))
        {
            return true;
        }
        return false;
    }

    private void checkAndMoveAlongX()
    {
        float valueX = Input.GetAxis("Horizontal");
        if (valueX != 0)
        {
            isMovingAlongX = true;
        } else
        {
            isMovingAlongX = false;
        }

        float movementSpeedMultiplier = movementSpeed;
        playerAnimation.speed = 1.0f;
        //playerAnimator.speed = 1.0f;

        if (isRunning)
        {
            movementSpeedMultiplier *= 2f;
            playerAnimation.speed = 2f;
        }

        if ((valueX > 0 && !isFlipped) || (valueX < 0 && isFlipped)) {
            movementSpeedMultiplier /= 4.0f;
            playerAnimation.speed /= 2f;
        }

        float value = valueX * movementSpeedMultiplier;
        if (value != 0)
        {
            playerTransformer.moveAlongX(value, isFlipped);
            ClientRequestHandler.updatePlayerMovementAlongX(value, isFlipped);
        }
    }

    private void checkAndMoveAlongY()
    {
        float valueY = Input.GetAxis("Vertical");
        if(valueY != 0)
        {
            isMovingAlongY = true;
        } else
        {
            isMovingAlongY = false;
        }

        float movementSpeedMultiplier = movementSpeed;
        playerAnimation.speed = 1f;

        if (isRunning)
        {
            movementSpeedMultiplier *= 2f;
            playerAnimation.speed = 2f;
        }
        playerTransformer.moveAlongY(valueY * movementSpeedMultiplier, isFlipped);
    }
}
