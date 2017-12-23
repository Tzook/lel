using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbandonQuestWindowUI : MonoBehaviour {

    [SerializeField]
    Text QuestTitle;

    Quest CurrentQuest;
    public void Show(Quest quest)
    {
        CurrentQuest = quest;
        this.gameObject.SetActive(true);
        QuestTitle.text = quest.Name;
    }

    public void Confirm()
    {
        SocketClient.Instance.SendQuestAborted(CurrentQuest.Key);
        LocalUserInfo.Me.ClientCharacter.AbandonQuest(CurrentQuest.Key);
        InGameMainMenuUI.Instance.RefreshQuestProgress();
        Game.Instance.CurrentScene.UpdateQuestProgress(CurrentQuest.Key);
        Game.Instance.HandleAbandonOkRoutines(CurrentQuest.Key);

        CurrentQuest = null;

        this.gameObject.SetActive(false);
    }
}
