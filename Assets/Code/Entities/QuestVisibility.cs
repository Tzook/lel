using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestVisibility : MonoBehaviour {

    public string HideQuestKey;
    public QuestState.QuestEnumState HideState;

    private void Start()
    {
        switch(HideState)
        {
            case QuestState.QuestEnumState.Completed:
                {
                    if(LocalUserInfo.Me.ClientCharacter.CompletedQuests.Contains(HideQuestKey))
                    {
                        Hide();
                    }

                    break;
                }
            case QuestState.QuestEnumState.InProgress:
                {
                    if (LocalUserInfo.Me.ClientCharacter.GetQuest(HideQuestKey) != null)
                    {
                        Hide();
                    }

                    break;
                }
            case QuestState.QuestEnumState.NotCompleted:
                {
                    if (!LocalUserInfo.Me.ClientCharacter.CompletedQuests.Contains(HideQuestKey))
                    {
                        Hide();
                    }

                    break;
                }
            case QuestState.QuestEnumState.NotInProgress:
                {
                    if (LocalUserInfo.Me.ClientCharacter.GetQuest(HideQuestKey) == null)
                    {
                        Hide();
                    }

                    break;
                }
        }
    }

    void Hide()
    {
        this.gameObject.SetActive(false);
    }
}

