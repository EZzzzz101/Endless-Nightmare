using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

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

    //打字机字体出现间隔
    public float myTime=0.05f;
    //打字机音效
    public AudioClip audioClip;

    private AudioSource npcAudio;

    private Coroutine _typeCoroutine;
    private string _fullText;
    private bool _isTyping;



    void Awake()
    {
        //订阅节点变化
        DialogueManager.Instance.OnNodeChanged += Refresh;
        //监听点击进入下一句话事件
        fullPanelButton.onClick.AddListener(OnClickContinue);
        //获取音频组件
        npcAudio = GetComponent<AudioSource>();

    }

     void OnDestroy()
    {
        DialogueManager.Instance.OnNodeChanged -= Refresh;
    }


    public void Refresh(DialogueNode node,int lineIndex)
    {

        //1.更新npc名字和文字
        npcNameText.text =  DialogueManager.Instance.CurrentNPCName;
       
        //打字机效果
        _fullText = node.npcLines[lineIndex];
        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        _typeCoroutine = StartCoroutine(TypeText(myTime));


        

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
        if (_isTyping)
        {
            // 字没打完 → 跳过动画，直接显示全文
            StopCoroutine(_typeCoroutine);
            npcText.text = _fullText;
            _isTyping = false;
            return;
        }
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

    //打字机协程
    IEnumerator TypeText(float time)
    {
        _isTyping=true;
        npcText.text="";

        for(int i = 0; i < _fullText.Length; i++)
        {

            npcText.text+=_fullText[i];
            if (!char.IsWhiteSpace(_fullText[i]) && !char.IsPunctuation(_fullText[i]))
            npcAudio.PlayOneShot(audioClip);

            yield return new WaitForSecondsRealtime(time);
        }

        _isTyping = false;
    }

}
