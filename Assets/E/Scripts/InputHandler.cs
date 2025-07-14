using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            gameManager.HoldTetromino();
        }
    }
}    