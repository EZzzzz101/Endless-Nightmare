using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
   //panel上的UI元素
   [Header("UI元素引用")]
    public TMP_Text npcNameText;        // NPC 名字
    public TMP_Text npcText;            // NPC 说的话
    public Transform optionsContainer;  // 选项按钮放哪
    public GameObject optionButtonPrefab; // 选项按钮预制体

    // 继续提示
    public GameObject continueHint;       // 箭头 + 文字 "点击继续"
    public Button fullPanelButton;        // 覆盖全面板的透明按钮


    void Awake()
    {
        //订阅节点变化
        DialogueManager.Instance.OnNodeChanged += Refresh;
        //监听点击进入下一句话事件
        fullPanelButton.onClick.AddListener(OnClickContinue);
    }

     void OnDestroy()
    {
        DialogueManager.Instance.OnNodeChanged -= Refresh;
    }


    public void Refresh(DialogueNode node,int lineIndex)
    {

        //1.更新npc名字和文字
        npcNameText.text =  DialogueManager.Instance.CurrentNPCName;
        npcText.text = node.npcLines[lineIndex];

        

        //2.删除旧选项按钮
        ClearOptions();

        Debug.Log($"Refresh: lineIndex={lineIndex}, text=[{node.npcLines[lineIndex]}], npcText is null? {npcText == null}");


        // 3. 判断：还有下一句？还是已经最后一句了？
        bool isLastLine = (lineIndex >= node.npcLines.Count - 1);
        bool hasNoOptions = (node.playerOptions.Count == 0);

        //最后一句且没有选项再点隐藏panel
        if (isLastLine && hasNoOptions)
        {
            continueHint.SetActive(true);
            fullPanelButton.gameObject.SetActive(true);
            
        }

        //最后一句但是有选项
        else if (isLastLine)
        {
            // 所有句子播完 → 显示选项，隐藏继续提示
            continueHint.SetActive(false);
            fullPanelButton.gameObject.SetActive(false);
            ShowOptions(node);
        }
        else
        {
            // 还有下一句 → 隐藏选项，显示继续提示
            continueHint.SetActive(true);
            fullPanelButton.gameObject.SetActive(true);
        }
    }

    void OnClickContinue()
    {
        DialogueManager.Instance.ContinueDialogue();
    }

    void ClearOptions()
    {
        foreach (Transform child in optionsContainer)
            Destroy(child.gameObject);
    }

    void ShowOptions(DialogueNode node)
    {
        for (int i = 0; i < node.playerOptions.Count; i++)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer);
            btnObj.GetComponentInChildren<TMP_Text>().text = node.playerOptions[i].playerText;

            int index = i;
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                DialogueManager.Instance.SelectOption(index);
            });
        }
    }

}
