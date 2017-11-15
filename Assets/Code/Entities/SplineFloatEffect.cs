using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineFloatEffect : MonoBehaviour {

    [SerializeField]
    ParticleSystem Particles;

    [SerializeField]
    string StartSoundKey;

    [SerializeField]
    string EndSoundKey;

    public void Launch(Transform from, Transform to, float speed = 1f, float height = 3f)
    {
        StartCoroutine(LaunchRoutine(from, to, speed, height));
    }

    public IEnumerator LaunchRoutine(Transform from, Transform to, float speed = 1f, float height = 3f)
    {
        Particles.Play();
        AudioControl.Instance.Play(StartSoundKey);

        float t = 0f;
        while(t<1f)
        {
            t += speed * Time.deltaTime;
            transform.position = Game.SplineLerp(from.transform.position, to.transform.position, height, t);

            yield return 0;
        }

        Particles.Stop();
        AudioControl.Instance.Play(EndSoundKey);

        yield return new WaitForSeconds(3f);

        this.gameObject.SetActive(false);
    }
}
