using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public DialogueData dialogueData;       // 引用你建的 Dialogue_Test.asset
    public GameObject interactionHint;      // "按 F 对话" 提示文字

    private bool playerInRange = false;

    void Start()
    {
        interactionHint.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactionHint.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionHint.SetActive(false);
            
            // 走远自动关对话
            if (DialogueManager.Instance.IsTalking)
                DialogueManager.Instance.EndDialogue();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !DialogueManager.Instance.IsTalking)
        {
            DialogueManager.Instance.StartDialogue(dialogueData);
        }
    }
}
