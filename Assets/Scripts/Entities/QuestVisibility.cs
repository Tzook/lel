using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestVisibility : MonoBehaviour {

    public string HideQuestKey;
    public QuestState.QuestEnumState HideState;

    private void Start()
    {
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(Initialize());
        }
        
    }

    IEnumerator Initialize()
    {
        while(Game.Instance.CurrentScene == null)
        {
            yield return 0;
        }

        Game.Instance.CurrentScene.AddQuestVisibilityObject(this);

        Refresh();
    }

    public void Refresh()
    {
        switch (HideState)
        {
            case QuestState.QuestEnumState.Completed:
                {
                    if (LocalUserInfo.Me.ClientCharacter.CompletedQuests.Contains(HideQuestKey))
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }

                    break;
                }
            case QuestState.QuestEnumState.InProgress:
                {
                    Quest tempQuest = LocalUserInfo.Me.ClientCharacter.GetQuest(HideQuestKey);
                    if (tempQuest != null && !tempQuest.CanBeCompleted)
                    {
                        Debug.Log(this.gameObject.name + "HIDE");
                        Hide();
                    }
                    else
                    {
                        Debug.Log(this.gameObject.name + "SHOW");
                        Show();
                    }

                    break;
                }
            case QuestState.QuestEnumState.NotCompleted:
                {
                    if (!LocalUserInfo.Me.ClientCharacter.CompletedQuests.Contains(HideQuestKey))
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }

                    break;
                }
            case QuestState.QuestEnumState.NotInProgress:
                {
                    if (LocalUserInfo.Me.ClientCharacter.GetQuest(HideQuestKey) == null)
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }

                    break;
                }
            case QuestState.QuestEnumState.CanBeCompleted:
                {
                    if (LocalUserInfo.Me.ClientCharacter.GetQuest(HideQuestKey) != null && LocalUserInfo.Me.ClientCharacter.GetQuest(HideQuestKey).CanBeCompleted)
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }

                    break;
                }
            case QuestState.QuestEnumState.NeverStarted:
                {
                    if (LocalUserInfo.Me.ClientCharacter.GetQuest(HideQuestKey) == null && !LocalUserInfo.Me.ClientCharacter.CompletedQuests.Contains(HideQuestKey))
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }

                    break;
                }
            case QuestState.QuestEnumState.IsAvailable:
                {
                    Quest tempQuest = LocalUserInfo.Me.ClientCharacter.GetQuest(HideQuestKey);
                    if (tempQuest.IsAvailable(LocalUserInfo.Me.ClientCharacter))
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }

                    break;
                }
            case QuestState.QuestEnumState.IsUnavailable:
                {
                    Quest tempQuest = LocalUserInfo.Me.ClientCharacter.GetQuest(HideQuestKey);
                    if (!tempQuest.IsAvailable(LocalUserInfo.Me.ClientCharacter))
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }

                    break;
                }
            default:
                {
                    this.gameObject.SetActive(true);
                    break;
                }
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }
}

