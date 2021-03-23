using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    PlayerConversant playerConversant;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI speakerNameText;
    [SerializeField] Image portraitImage;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] Button nextButton;

    void Start()
    {
        playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        playerConversant.onConversationUpdated += UpdateUI;
        nextButton.onClick.AddListener(Next);
    }

    void UpdateUI()
    {
        dialogueBox.SetActive(playerConversant.IsActive());
        if (!playerConversant.IsActive()) return;
        Texture2D newTex = playerConversant.GetConversantSprite();
        portraitImage.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(.5f, .5f), 100.0f);
        dialogueText.text = playerConversant.GetText();
        speakerNameText.text = playerConversant.GetCurrentConversantName();
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
                Next();
            }
        }
    }
}
