using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField]
        TextMeshProUGUI fieldAIText;

        [SerializeField]
        GameObject aIResponse;

        [SerializeField]
        Button nextButton;

        [SerializeField]
        Button quitButton;

        [SerializeField]
        Transform choiceContainer;

        [SerializeField]
        GameObject responsePrefab;

        [SerializeField]
        TextMeshProUGUI conversantName;

        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(playerConversant.Next);
            quitButton.onClick.AddListener(playerConversant.Quit);
            UpdateUI();
        }

        private void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive()) return;
            conversantName.text = playerConversant.GetCurrentConversantName() ?? string.Empty;
            UpdateChoiceAndResponseVisibility();
            if (playerConversant.IsChoosing())
            {
                BuildResponseList();
            }
            else
            {
                fieldAIText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        private void BuildResponseList()
        {
            ClearResponseButtons();
            CreateResponseButtons();
        }

        private void UpdateChoiceAndResponseVisibility()
        {
            aIResponse.SetActive(!playerConversant.IsChoosing());
            choiceContainer.gameObject.SetActive(playerConversant.IsChoosing());
        }

        private void ClearResponseButtons()
        {
            foreach (Transform button in choiceContainer)
            {
                Destroy(button.gameObject);
            }
        }

        private void CreateResponseButtons()
        {
            foreach (DialogueNode response in playerConversant.GetChoices())
            {
                GameObject responseButton = Instantiate(responsePrefab, choiceContainer);
                responseButton.GetComponentInChildren<TextMeshProUGUI>().text = response.Text;
                responseButton.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    playerConversant.SelectResponse(response);
                });
            }
        }
    }
}
