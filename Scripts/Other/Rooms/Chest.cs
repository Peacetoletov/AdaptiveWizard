using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: change the sprite to an open chest once the player opens this chest for the first time
public class Chest : MonoBehaviour
{
    // used for instantiates
    public GameObject interactionPromptIconPrefab;


    private Vector2 CHEST_SIZE;
    private GameObject interactionPromptIcon = null;
    private bool currentlyOpen = false;
    private ChestLocalContent content;

    private void Start() {
        this.CHEST_SIZE = Utility.SpriteSize("Sprites/Environment/chestClosedSpr");
        this.content = new ChestLocalContent();
    }

    private void Update() {
        if (MainGameManager.IsGameActive()) {
            // prompt an interaction
            CreateInteractionPromptIfPlayerIsClose();

            if (interactionPromptIcon != null && Input.GetKeyDown(KeyCode.F) && !currentlyOpen) {
                // open the chest, set the game state as partially active, remove the interaction prompt
                this.currentlyOpen = true;
                MainGameManager.GetUI_Manager().GetUI_ChestContentManager().ShowChestContent(content);
                MainGameManager.SetGameState(MainGameManager.GameState.PARTIALLY_ACTIVE);
                RemoveInteractionPrompt();
            }
        } else if (MainGameManager.IsGamePartiallyActive()) {
            if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape)) && currentlyOpen) {
                // close the chest, set the game state as active
                this.currentlyOpen = false;
                MainGameManager.GetUI_Manager().GetUI_ChestContentManager().HideChestContent();
                MainGameManager.SetGameState(MainGameManager.GameState.ACTIVE);
            }
        }
    }

    private void CreateInteractionPromptIfPlayerIsClose() {
        // checks if player is close, and spawns a prompt icon if so
        Collider2D collider = Physics2D.OverlapBox(gameObject.transform.position, BoxSize(), 0, LayerMask.GetMask("Player"));
        if (interactionPromptIcon == null && collider != null) {
            // Create a prompt
            this.interactionPromptIcon = Instantiate(interactionPromptIconPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        }
        else if (interactionPromptIcon != null && collider == null) {
            RemoveInteractionPrompt();
        }
    }

    private void RemoveInteractionPrompt() {
        Destroy(interactionPromptIcon);
        this.interactionPromptIcon = null;
    }

    private Vector2 BoxSize() {
        // How close the player must be (as a multiple of chest's larger side's size) for the prompt to show up.
        // This can be tweaked to spawn the prompt farther/closer.
        const float promptDistance = 1.5f;

        float overlapBoxSizeMinusChestSize = Mathf.Max(CHEST_SIZE.x * (promptDistance - 1), CHEST_SIZE.y * (promptDistance - 1));
        return new Vector2(CHEST_SIZE.x + overlapBoxSizeMinusChestSize, CHEST_SIZE.y + overlapBoxSizeMinusChestSize);
    }
}
