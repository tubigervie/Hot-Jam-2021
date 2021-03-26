using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    const string kAlphaCode = "<color=#00000000>";
    const float kMaxTextTime = .1f;
    public static int TextSpeed = 5;

    Coroutine dialogueTextCoroutine;
    AudioSource audioSource;
    PlayerConversant playerConversant;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI speakerNameText;
    [SerializeField] Image portraitImage;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] Button nextButton;
    [SerializeField] GameObject leafCursor;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    void Start()
    {
        playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        if (playerConversant == null) 
        {
            Debug.Log("No player conversant found.");
            return;
        }
        playerConversant.onConversationUpdated += UpdateUI;
        nextButton.onClick.AddListener(Next);
    }
    //-330, 20, 400, 550
    //-410, 114, 400, 600
    void UpdateUI()
    {
        dialogueBox.SetActive(playerConversant.IsActive());
        if (!dialogueBox.activeSelf) return;
        Texture2D newTex = playerConversant.GetConversantSprite();
        portraitImage.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(.5f, .5f), 100.0f);
        portraitImage.GetComponent<RectTransform>().sizeDelta = playerConversant.GetDialogueSpriteSize();
        portraitImage.GetComponent<RectTransform>().anchoredPosition = playerConversant.GetSpritePosition();
        speakerNameText.text = playerConversant.GetCurrentConversantName();
        dialogueTextCoroutine = StartCoroutine(DisplayDialogueText(playerConversant.GetText()));
    }

    private IEnumerator DisplayDialogueText(string text)
    {
        dialogueText.text = "";
        leafCursor.SetActive(false);
        string originalText = playerConversant.GetText();
        string displayedText = "";
        int alphaIndex = 0;
        foreach(char c in playerConversant.GetText().ToCharArray())
        {
            alphaIndex++;
            dialogueText.text = originalText;
            displayedText = dialogueText.text.Insert(alphaIndex, kAlphaCode);
            if(alphaIndex % 6 == 0 && playerConversant.GetDialogueFX() != null)
                audioSource.PlayOneShot(playerConversant.GetDialogueFX(), playerConversant.GetDialogueVolume());
            dialogueText.text = displayedText;
            yield return new WaitForSeconds(kMaxTextTime / TextSpeed);
        }
        leafCursor.SetActive(true);
        dialogueTextCoroutine = null;
        yield return null;
    }


    void Next()
    {
        playerConversant.Next();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueBox.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(dialogueTextCoroutine != null)
                {
                    StopAllCoroutines();
                    dialogueText.text = playerConversant.GetText();
                    dialogueTextCoroutine = null;
                    leafCursor.SetActive(true);
                }
                else
                    Next();
            }
        }
    }
}
