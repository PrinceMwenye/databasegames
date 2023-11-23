using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Experimental.AI;
using CodeMonkey.Utils;

public class snake : MonoBehaviour
{
    private enum Direction
    {
        Left, Right, Up, Down
    }
    private Vector2Int gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<Vector2Int> snakeMovePositionList;
    private List<SnakeBodyPart> SnakeBodyPartList;

    private void Awake()
    {
        gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax =1f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = new Vector2Int(1, 0);
        snakeMovePositionList = new List<Vector2Int>();
        snakeBodySize = 1;
        SnakeBodyPartList = new List<SnakeBodyPart>();

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

        gridMoveTimer += Time.deltaTime * 5;
        if(gridMoveTimer > gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            snakeMovePositionList.Insert(0, gridPosition);
            gridPosition += gridMoveDirection;
            if (snakeMovePositionList.Count>= snakeBodySize+ 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);    
            }

        

        }


        transform.position = new Vector3(gridPosition.x, gridPosition.y);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirection) -90);

        UpdateSnakeBodyParts();

       



        bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
        if (snakeAteFood)
        {
            snakeBodySize++;
            CreateSnakeBody();
        }
        
    }


    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < SnakeBodyPartList.Count; i++)
        {
            SnakeBodyPartList[i].SetGridPosition(snakeMovePositionList[i]);
        }
    }

    private void CreateSnakeBody()
    {
        SnakeBodyPartList.Add(new SnakeBodyPart(SnakeBodyPartList.Count));
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

    public List<Vector2Int> GetFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        gridPositionList.AddRange(snakeMovePositionList);
        return gridPositionList;
    }


    public class SnakeBodyPart
    {

        private Vector2Int gridPosition;
        private Transform transform;
        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyGameObject = new("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetGridPosition(Vector2Int gridPosition)
        {
            this.gridPosition = gridPosition;
            transform.position = new Vector3(gridPosition.x, gridPosition.y);
        }

    }
  
  
}
