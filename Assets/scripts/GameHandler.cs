using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{

    [SerializeField] private snake snake;

    private LevelGrid levelGrid;
    private void Awake()
    {

        levelGrid = new LevelGrid(20, 20);
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
      
    }

    void Update()
    {
        
    }
}
