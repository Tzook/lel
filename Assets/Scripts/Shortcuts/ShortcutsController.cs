using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutsController : MonoBehaviour 
{
    private bool inMood = false;
    private int keyIndex = 0;
    private Dictionary<string, Action> keyToAction = new Dictionary<string, Action>();
    private List<GameObject> bubbles = new List<GameObject>();

    public void DontUpdate()
    {
       if (!Game.Instance.InGame || Game.Instance.InChat)
       {
           return;
       }
       if (inMood)
       {
           // TODO disable other actions when in mood
           // TODO improve UI for being in the mood
           WaitForActionKey();
           WaitForExitingKey();
       }
       else 
       {
           if (Input.GetKeyDown(InputMap.Map["Shortcuts"])) 
           {
               MapActionsToKeys();
           }
       }
    }

    private void WaitForActionKey()
    {
        char? foundKey = null;
        foreach (KeyValuePair<string, Action> keyAndAction in keyToAction)
        {
            if (Input.GetKeyDown(keyAndAction.Key[keyIndex].ToString()))
            {
                if (keyAndAction.Key.Length == keyIndex + 1)
                {
                    // it's the last key of the action - call the action and finish the business
                    keyAndAction.Value();
                    ExitMood();
                }
                else 
                {
                    foundKey = keyAndAction.Key[keyIndex];
                }
                break;
            }
        }

        if (foundKey != null)
        {
            TrimActions((char)foundKey);
        }
    }

    private void ExitMood()
    {
        Debug.Log("Exiting shortcuts mood");
        inMood = false;
        SetBlur(false);
        removeBubbles();
    }

    private void EnterMood()
    {
        Debug.Log("Entering shortcuts mood");
        inMood = true;
        keyIndex = 0;
        keyToAction.Clear();
        SetBlur(true);
    }

    private void SetBlur(bool state)
    {
        GameCamera.Instance.SetBlurMode(state);
    }

    private void removeBubbles()
    {
        foreach (GameObject bubble in bubbles) 
        {
            bubble.SetActive(false);
        }
    }

    private void TrimActions(char foundKey)
    {
        List<string> removalList = new List<string>();
        foreach (KeyValuePair<string, Action> keyAndAction in keyToAction)
        {
            if (keyAndAction.Key[keyIndex] != foundKey)
            {
                // the key typed didn't match this action - remove it
                removalList.Add(keyAndAction.Key);
            }
        }
        foreach (string key in removalList)
        {
            keyToAction.Remove(key);

        }

        keyIndex++;
    }

    private void WaitForExitingKey()
    {
        UnityEngine.KeyCode[] keys = {KeyCode.Escape, KeyCode.Tab, KeyCode.Return, KeyCode.Space};
        foreach (UnityEngine.KeyCode key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                ExitMood();
                return;
            }
        }
    }

    // go through all the AbstractShortcuts instances in the scene and assign keys for each action in them
    private void MapActionsToKeys()
    {
        EnterMood();

        AbstractShortcuts[] shortcutsList = FindObjectsOfType(typeof(AbstractShortcuts)) as AbstractShortcuts[];
        Dictionary<string, List<AbstractShortcuts>> typeToShortcuts = MapByFirstKey(shortcutsList);

        // TODO handle cases of more than 10 objects with the same type (cause p1 will be done before p11 so we have to begin with p11 and not p1)
        // TODO get only the shortcuts in the map (closest to you first)
        foreach (KeyValuePair<string, List<AbstractShortcuts>> keyValuePair in typeToShortcuts)
        {
            int index = 1;
            string actionsFirstKey = keyValuePair.Value[0].GetActionsFirstKey();
            bool haveIndex = keyValuePair.Value.Count > 1;
            foreach (AbstractShortcuts shortcuts in keyValuePair.Value)
            {
                shortcuts.ResetPosition();
                foreach (KeyAction keyAction in shortcuts.GetActions())
                {
                    string key = actionsFirstKey + (haveIndex ? index.ToString() : "") + keyAction.key;
                    keyToAction[key] = keyAction.action;
                    ShowActionBubble(shortcuts, keyAction, key);
                    Debug.Log("Type key " + key + " to invoke action " + keyAction.text);
                }
                index++;
            }
        }
    }

    private void ShowActionBubble(AbstractShortcuts shortcuts, KeyAction keyAction, string key)
    {
        // TODO connect bubble to its parent so it moves with it
        GameObject shortcutBubble = ResourcesLoader.Instance.GetRecycledObject("ShortcutBubble");
        shortcutBubble.transform.position = shortcuts.GetPosition();
        shortcutBubble.transform.GetChild(0).GetComponent<Text>().text = key + " - " + keyAction.text;
        Button btn = shortcutBubble.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(delegate
        {
            keyAction.action();
            ExitMood();
        });
        bubbles.Add(shortcutBubble);
    }

    private Dictionary<string, List<AbstractShortcuts>> MapByFirstKey(AbstractShortcuts[] shortcutsList)
    {
        Dictionary<string, List<AbstractShortcuts>> dictionary = new Dictionary<string, List<AbstractShortcuts>>();

        foreach (AbstractShortcuts shortcuts in shortcutsList)
        {
            string key = shortcuts.GetActionsFirstKey();
            if (shortcuts.enabled)
            {
                // some objects might be disabled due to recycling
                if (!dictionary.ContainsKey(key))
                {
                    dictionary[key] = new List<AbstractShortcuts>();
                }
                dictionary[key].Add(shortcuts);
            }
        }

        return dictionary;
    }
}
