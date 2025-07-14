using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public float fallInterval = 1f;
    private float fallTimer;
    private float moveTimer;
    private float moveInterval = 0.1f;
    private Vector3 rotationPoint;

    private void Start()
    {
        rotationPoint = transform.GetChild(1).localPosition;
        if (!IsValidPosition())
        {
            FindObjectOfType<GameManager>().GameOver();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandleInput();
        HandleFalling();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
    }

    private void HandleFalling()
    {
        fallTimer += Time.deltaTime;
        if (fallTimer >= fallInterval)
        {
            MoveDown();
            fallTimer = 0;
        }
    }

    private void MoveLeft()
    {
        transform.position += Vector3.left;
        if (!IsValidPosition())
        {
            transform.position += Vector3.right;
        }
        else
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);
        }
    }

    private void MoveRight()
    {
        transform.position += Vector3.right;
        if (!IsValidPosition())
        {
            transform.position += Vector3.left;
        }
        else
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);
        }
    }

    private void MoveDown()
    {
        transform.position += Vector3.down;
        if (!IsValidPosition())
        {
            transform.position += Vector3.up;
            FindObjectOfType<GameManager>().LockTetromino(this);
            FindObjectOfType<GameManager>().SpawnNextTetromino();
            enabled = false;
        }
        else
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);
        }
    }

    private void HardDrop()
    {
        while (IsValidPosition())
        {
            transform.position += Vector3.down;
        }
        transform.position += Vector3.up;
        FindObjectOfType<GameManager>().LockTetromino(this);
        FindObjectOfType<GameManager>().SpawnNextTetromino();
        enabled = false;
    }

    private void Rotate()
    {
        transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, 90);
        if (!IsValidPosition())
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, -90);
        }
        else
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);
        }
    }

    private bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            Vector2 pos = FindObjectOfType<GameManager>().RoundPosition(child.position);
            if (!FindObjectOfType<GameManager>().IsInsideGrid(pos))
            {
                return false;
            }
            if (FindObjectOfType<GameManager>().GetTransformAtGridPosition(pos) != null && 
                FindObjectOfType<GameManager>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }
}    