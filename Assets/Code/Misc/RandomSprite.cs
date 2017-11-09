using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour {

    [SerializeField]
    List<Sprite> sCollection = new List<Sprite>();

    SpriteRenderer sRenderer;

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        sRenderer.sprite = sCollection[Random.Range(0, sCollection.Count)];
    }
}
