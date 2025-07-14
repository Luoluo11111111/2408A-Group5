using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 20;
    public GameObject[] tetrominoes;
    public Transform[,] grid;
    public GameObject nextTetrominoDisplay;
    public GameObject holdTetrominoDisplay;
    public int score = 0;
    public TMPro.TMP_Text scoreText;
    public TMPro.TMP_Text gameOverText;
    public TMPro.TMP_Text levelText;
    public int level = 1;
    public float fallSpeedMultiplier = 0.9f;
    private GameObject currentTetromino;
    private GameObject nextTetromino;
    private GameObject heldTetromino;
    private bool canHold = true;

    private void Start()
    {
        grid = new Transform[gridWidth, gridHeight];
        gameOverText.gameObject.SetActive(false);
        SpawnNextTetromino();
        UpdateScore();
        UpdateLevel();
    }

    public void SpawnNextTetromino()
    {
        if (nextTetromino == null)
        {
            nextTetromino = Instantiate(tetrominoes[Random.Range(0, tetrominoes.Length)], 
                new Vector3(gridWidth / 2, gridHeight - 1, 0), Quaternion.identity);
        }
        else
        {
            nextTetromino.transform.position = new Vector3(gridWidth / 2, gridHeight - 1, 0);
        }
        currentTetromino = nextTetromino;
        nextTetromino = Instantiate(tetrominoes[Random.Range(0, tetrominoes.Length)], 
            nextTetrominoDisplay.transform.position, Quaternion.identity);
        nextTetromino.transform.SetParent(nextTetrominoDisplay.transform);
        canHold = true;
        UpdateGrid(currentTetromino.GetComponent<Tetromino>());
    }

    public void HoldTetromino()
    {
        if (!canHold) return;

        canHold = false;
        GameObject temp = null;

        if (heldTetromino == null)
        {
            heldTetromino = currentTetromino;
            Destroy(currentTetromino);
            SpawnNextTetromino();
        }
        else
        {
            temp = currentTetromino;
            heldTetromino.transform.position = new Vector3(gridWidth / 2, gridHeight - 1, 0);
            currentTetromino = heldTetromino;
            currentTetromino.GetComponent<Tetromino>().enabled = true;
            UpdateGrid(currentTetromino.GetComponent<Tetromino>());
            heldTetromino = temp;
        }

        foreach (Transform child in heldTetromino.transform)
        {
            child.gameObject.SetActive(false);
        }

        heldTetromino.transform.position = holdTetrominoDisplay.transform.position;
        heldTetromino.transform.SetParent(holdTetrominoDisplay.transform);

        foreach (Transform child in heldTetromino.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void UpdateGrid(Tetromino tetromino)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null && grid[x, y].parent == tetromino.transform)
                {
                    grid[x, y] = null;
                }
            }
        }

        foreach (Transform child in tetromino.transform)
        {
            Vector2 pos = RoundPosition(child.position);
            if (IsInsideGrid(pos))
            {
                grid[(int)pos.x, (int)pos.y] = child;
            }
        }
    }

    public void LockTetromino(Tetromino tetromino)
    {
        foreach (Transform child in tetromino.transform)
        {
            Vector2 pos = RoundPosition(child.position);
            if (IsInsideGrid(pos))
            {
                grid[(int)pos.x, (int)pos.y] = child;
            }
        }

        CheckLines();
        UpdateScore();
        UpdateLevel();
    }

    private void CheckLines()
    {
        int linesCleared = 0;
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            if (IsLineComplete(y))
            {
                linesCleared++;
                ClearLine(y);
                MoveLinesDown(y);
                y++;
            }
        }

        if (linesCleared > 0)
        {
            score += CalculateScore(linesCleared);
            AdjustFallSpeed();
        }
    }

    private bool IsLineComplete(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    private void ClearLine(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
            }
            grid[x, y] = null;
        }
    }

    private void MoveLinesDown(int y)
    {
        for (int i = y; i < gridHeight - 1; i++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, i + 1] != null)
                {
                    grid[x, i] = grid[x, i + 1];
                    grid[x, i + 1] = null;
                    grid[x, i].position += Vector3.down;
                }
            }
        }
    }

    private int CalculateScore(int linesCleared)
    {
        switch (linesCleared)
        {
            case 1: return 100 * level;
            case 2: return 300 * level;
            case 3: return 500 * level;
            case 4: return 800 * level;
            default: return 0;
        }
    }

    private void AdjustFallSpeed()
    {
        level = score / 1000 + 1;
        Tetromino[] tetrominoes = FindObjectsOfType<Tetromino>();
        foreach (Tetromino tetromino in tetrominoes)
        {
            tetromino.fallInterval = Mathf.Max(0.1f, 1f * Mathf.Pow(fallSpeedMultiplier, level - 1));
        }
    }

    public Vector2 RoundPosition(Vector3 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }

    public bool IsInsideGrid(Vector2 position)
    {
        return ((int)position.x >= 0 && (int)position.x < gridWidth && (int)position.y >= 0);
    }

    public Transform GetTransformAtGridPosition(Vector2 position)
    {
        if (!IsInsideGrid(position)) return null;
        return grid[(int)position.x, (int)position.y];
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    private void UpdateLevel()
    {
        levelText.text = "Level: " + level.ToString();
    }
}    