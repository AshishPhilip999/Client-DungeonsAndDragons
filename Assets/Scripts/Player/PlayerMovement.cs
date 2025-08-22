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

    public float movementSpeed;

    public PlayerView playerView;
    public ViewDistanceController viewDistanceController;

    private Vector3 prevPosition;

    private void Start()
    {
        prevPosition = transform.position;
    }

    private void Update()
    {
        if (WorldData.tilesPopulated)
        {
            playerView.enabled = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       float distance = Vector3.Distance(transform.position, prevPosition);
       int verticalDirection = GetVerticalDirection(transform.position, prevPosition);
       if(isMovementApplied())
        {
            moveAlongX();
            moveAlongY();
            ClientRequestHandler.updatePlayerData(transform.position.x, transform.position.y);
            if (distance > 0.25f)
            {
                isMoving = false;
                ClientRequestHandler.getTerrainData(transform.position.x, transform.position.y, viewDistanceController.viewDistance);
                playerView.updateTiles(transform.position, verticalDirection);
                prevPosition = transform.position;
            }
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
        if(Input.GetKey(horizontalMovementKey) || Input.GetKey(horizontalNegMovementKey) || Input.GetKey(verticalMovementKey) || Input.GetKey(verticaNeglMovementKey))
        {
            return true;
        }

        return false;
    }

    private void moveAlongX()
    {
        float valueX = Input.GetAxis("Horizontal");
        Vector3 moveX = new Vector3(valueX * movementSpeed * Time.deltaTime, 0, 0);
        transform.position += moveX;
    }

    private void moveAlongY()
    {
        float valueY = Input.GetAxis("Vertical");
        Vector3 moveY = new Vector3(0, valueY * movementSpeed * Time.deltaTime, 0);
        transform.position += moveY;
    }
}
