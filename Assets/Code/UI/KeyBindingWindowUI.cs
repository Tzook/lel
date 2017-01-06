using UnityEngine;
using System.Collections;
using System.Linq;

public class KeyBindingWindowUI : MonoBehaviour {

    [SerializeField]
    Transform Container;

	public void Open()
    {
        this.gameObject.SetActive(true);
        Clear();

        GameObject tempPiece;
        string key;
        for(int i=0;i<InputMap.Map.Count;i++)
        {
            tempPiece = ResourcesLoader.Instance.GetRecycledObject("KeyBindingPiece");
            tempPiece.transform.SetParent(Container, false);
            key = InputMap.Map.Keys.ElementAt(i);
            tempPiece.GetComponent<KeyBindingPieceUI>().SetInfo(key, InputMap.Map[key]);
        }
    }

    private void Clear()
    {
        for(int i=0;i<Container.childCount;i++)
        {
            Container.GetChild(i).gameObject.SetActive(false);
        }
    }
}
