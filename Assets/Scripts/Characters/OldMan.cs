using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OldMan : MonoBehaviour
{
    [Header("对话内容")]
    [TextArea] public string[] introLines;
    [TextArea] public string[] congratsLines;

    [Header("UI")]
    public TMP_Text dialogueText;

    [Header("打字机设置")]
    public float typeSpeed = 0.04f; // 每个字的间隔

    [Header("讲话音效")]
    public AudioClip[] speakClips; // 拖入你的音效组

    private int dialogueIndex = 0;
    private bool isCongratsPhase = false;
    private bool playerInRange = false;
    private Coroutine typingCoroutine;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowNextLine();
        }
    }

    private void ShowNextLine()
    {
        string[] lines = isCongratsPhase ? congratsLines : introLines;

        if (dialogueIndex < lines.Length)
        {
            ShowTextWithTypewriter(lines[dialogueIndex]);
            dialogueIndex++;
        }
        else
        {
            // 重复最后一句
            ShowTextWithTypewriter(lines[lines.Length - 1]);
        }
    }

    private void ShowTextWithTypewriter(string text)
    {
        if (speakClips != null && speakClips.Length > 0)
        {
            var clip = speakClips[Random.Range(0, speakClips.Length)];
            AudioManager.Instance.PlaySfx(clip);
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            dialogueIndex = 0; // 每次靠近重置对话进度
            ShowNextLine();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            dialogueText.text = "";
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
        }
    }

    // 玩家收集满三角时调用
    public void OnPlayerCollectAll()
    {
        isCongratsPhase = true;
        dialogueIndex = 0; // 进入新阶段时重置对话进度
    }
}