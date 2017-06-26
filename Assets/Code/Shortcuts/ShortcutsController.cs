using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutsController : MonoBehaviour 
{
    private bool inMood = false;
    private int keyIndex = 0;

    private Dictionary<string, Action> keyToAction = new Dictionary<string, Action>();

    public void Update()
    {
        if (inMood)
        {
            // TODO disable all other actions when in mood
            // TODO add UI for being in the mood
            WaitForActionKey();
        }
        else 
        {
            // TODO use mapping for shortcuts key
            if (Input.GetKeyDown(KeyCode.Tab)) 
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
        inMood = false;
    }

    private void EnterMood()
    {
        inMood = true;
        keyIndex = 0;
        keyToAction.Clear();
    }

    private void TrimActions(char foundKey)
    {
        foreach (KeyValuePair<string, Action> keyAndAction in keyToAction)
        {
            if (keyAndAction.Key[keyIndex] != foundKey)
            {
                // the key typed didn't match this action - remove it
                keyToAction.Remove(keyAndAction.Key);
            }
        }
        keyIndex++;
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
            foreach (AbstractShortcuts shortcuts in keyValuePair.Value)
            {
                foreach (KeyAction keyAction in shortcuts.GetActions())
                {
                    string key = actionsFirstKey + index + keyAction.key;
                    keyToAction[key] = keyAction.action;
                    Debug.Log("Type key " + key + " to invoke action.");
                }
                index++;
            }
        }
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
