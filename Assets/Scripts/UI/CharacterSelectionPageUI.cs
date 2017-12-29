using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionPageUI : MonoBehaviour
{

    [SerializeField]
    protected Transform CharactersContainer;

    [SerializeField]
    protected CharacterInfoPageUI m_CharacterInfoUI;

    [SerializeField]
    protected ui_pageMenu m_CharacterSelectionPageMenu;

    [SerializeField]
    protected Button createCharacterButton;

    [SerializeField]
    protected GameObject NoCharactersHint;

    protected const int MAX_CHARACTERS = 8;

    public IEnumerator LoadCharactersCoroutine(User user)
    {
        yield return 0;
        ClearPlayerCharacters();

        NoCharactersHint.gameObject.SetActive(user.Characters.Count <= 0);

        for (int i = 0; i < user.Characters.Count; i++)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("CharSpot");
            tempObj.transform.SetParent(CharactersContainer, false);
            tempObj.GetComponent<CharspotUI>().Load(user.Characters[i]);
            AddListenerToCharspot(tempObj.GetComponent<Button>(), user.Characters[i]);
        }
        // enable creating more characters only if we have less than the maximum characters
        createCharacterButton.interactable = user.Characters.Count < MAX_CHARACTERS;
    }

    protected void ClearPlayerCharacters()
    {
        for (int i = 0; i < CharactersContainer.childCount; i++)
        {
            CharactersContainer.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            CharactersContainer.GetChild(i).gameObject.SetActive(false);
        }
    }

    protected void AddListenerToCharspot(Button charspotButton, ActorInfo info)
    {
        charspotButton.onClick.AddListener(delegate
        {
            m_CharacterSelectionPageMenu.SwitchTo(1);
            m_CharacterInfoUI.SetInfo(info);
        });
    }
}