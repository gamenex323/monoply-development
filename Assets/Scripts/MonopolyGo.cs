using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MonopolyGo : MonoBehaviour
{
    public GameObject playerIcon; // The icon that moves
    public Button goButton; // The button to start the move
    public Transform[] tiles; // Array of tile positions
    public float moveDuration = 0.5f; // Time taken to move to the next tile
    public float jumpHeight = 1.0f; // Height of the jump
    public int jumpCount = 1; // Number of jumps to perform

    public Transform dice1; // The Transform of the first dice
    public Transform dice2; // The Transform of the second dice
    public float diceRollDuration = 1f; // Duration for the dice roll animation

    private int currentTileIndex = 0;

    public PlayerClass playerClass;

    public static MonopolyGo instance;

    void Start()
    {
        if (!instance)
            instance = this;
        goButton.onClick.AddListener(OnGoButtonClicked);
    }

    void OnGoButtonClicked()
    {
        StartCoroutine(MovePlayer());
    }

    IEnumerator MovePlayer()
    {
        // Roll the dice to get a random movement sum
        int diceSum = RollDice();

        // Animate the dice rolling before moving
        yield return AnimateDiceRoll(diceSum);

        // Move the player based on the dice sum
        int steps = diceSum; // Use the dice sum as the number of steps
        for (int i = 0; i < steps; i++)
        {
            currentTileIndex = (currentTileIndex + 1) % tiles.Length;
            print("Tile To Jump: " + currentTileIndex);
            Vector3 targetPosition = tiles[currentTileIndex].position;

            // Move to the next tile with a jump
            yield return MoveToPosition(targetPosition);

            // Check if this is the last step
            if (i == steps - 1)
            {
                ActivateStepPanel(currentTileIndex); // Activate panel for the current tile
            }
            else
            {
                CheckTilePrize(currentTileIndex); // Handle prize logic for passing tiles
            }
        }
    }

    // Function to roll two dice
    int RollDice()
    {
        // Roll two dice (each between 1 and 6) and return their sum
        int die1 = Random.Range(1, 7);
        int die2 = Random.Range(1, 7);
        return die1 + die2;
    }

    // Function to animate the dice rolling
    IEnumerator AnimateDiceRoll(int targetSum)
    {
        // Animate the dice rolling (rotating them)
        float timePassed = 0;
        while (timePassed < diceRollDuration)
        {
            dice1.Rotate(Random.Range(90f, 180f) * Time.deltaTime, 0, 0);
            dice2.Rotate(Random.Range(90f, 180f) * Time.deltaTime, 0, 0);
            timePassed += Time.deltaTime;
            yield return null;
        }

        // You can optionally log the dice sum or display it on the UI
        Debug.Log("Dice Rolled: " + targetSum);
    }

    // Function to move the player icon to a new position with a jump
    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        // Use DOTween's DOJump for the jump effect
        playerIcon.transform.DOJump(targetPosition, jumpHeight, jumpCount, moveDuration).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(moveDuration);
    }

    // Function to handle tile panel activation for rewards
    void ActivateStepPanel(int tileIndex)
    {
        TileManager.instance.GiveTileReward(tiles[tileIndex].GetComponent<TileInfo>());
        Debug.Log("Landed on tile: " + tileIndex);
        //Invoke(nameof(StepPanelDisable), 2f);
    }

    void StepPanelDisable()
    {
        UIManager.instance.DisableAllRewardedPanels();
    }

    // Function to handle the prize logic when passing a tile
    void CheckTilePrize(int tileIndex)
    {
        Debug.Log("Passed tile: " + tileIndex);
        if (tiles[tileIndex].GetComponent<TileInfo>().tileName == GlobalData.TileName.Go)
        {
            UIManager.instance.UpdateMoney(tiles[tileIndex].GetComponent<TileInfo>().money);
        }
    }

    public enum PlayerClass
    {
        UpperClass,
        MiddleClass,
        WorkingClass,
        LowerClass
    }
}
