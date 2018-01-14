using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HittableEntity))]
public class WhackMoleAI : MonoBehaviour {

    HittableEntity m_Hittable;

    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    string HitSoundKey;

    [SerializeField]
    string PopSoundKey;

    private void Awake()
    {
        m_Hittable = GetComponent<HittableEntity>();
    }

    private void Start()
    {
        StartCoroutine(AIRoutine());
    }

    IEnumerator AIRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {

            Pop();

            yield return new WaitForSeconds(Random.Range(0.5f, 2f));

            Hide();

            yield return new WaitForSeconds(Random.Range(5f, 12f));


        }
    }

    void Hide()
    {
        m_Animator.SetTrigger("Hide");
        m_Hittable.Disable();
    }

    void Pop()
    {
        m_Animator.SetTrigger("Pop");
        AudioControl.Instance.Play(PopSoundKey);
        m_Hittable.Enable();
    }

    public void Smack()
    {
        AudioControl.Instance.Play(HitSoundKey);
        m_Animator.SetTrigger("Smack");
        m_Hittable.Disable();
        StopAllCoroutines();
        StartCoroutine(AIRoutine());
    }


}
