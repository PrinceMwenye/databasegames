using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snake : MonoBehaviour
{
    private Vector2Int gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;

    private void Awake()
    {
        gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax =1f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = new Vector2Int(1, 0);

    }


    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("levelGrid: " + levelGrid);  // Add this line for debugging

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (gridMoveDirection.y != -1) { gridMoveDirection.x = 0; gridMoveDirection.y = +1; } }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDirection.y != +1) { gridMoveDirection.x = 0; gridMoveDirection.y = -1; }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDirection.x != +1) { gridMoveDirection.x = -1; gridMoveDirection.y = 0; }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDirection.x != -1) { gridMoveDirection.x = +1; gridMoveDirection.y = 0; }
        }

        gridMoveTimer += Time.deltaTime;
        if(gridMoveTimer > gridMoveTimerMax)
        {
            gridPosition += gridMoveDirection;
            gridMoveTimer -= gridMoveTimerMax;
        }


        transform.position = new Vector3(gridPosition.x, gridPosition.y);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirection) -90);
        levelGrid.SnakeMoved(gridPosition);
        
    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
}
