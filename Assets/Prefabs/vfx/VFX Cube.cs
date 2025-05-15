using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXCube : MonoBehaviour
{
    public ParticleSystem particlesIn;

    public List<ParticleSystem> particlesForm = new List<ParticleSystem>();
    public GameObject form;
    private Vector3 originalFormScale;
    private Coroutine currentCoroutine;
    public List<ParticleSystem> particlesExplosion = new List<ParticleSystem>();
    public List<ParticleSystem> particlesExplosionParticles = new List<ParticleSystem>();

    public Vector3 minScale = new Vector3(0.2f, 0.2f, 0.2f);
    public Vector3 maxScale = new Vector3(2, 2, 2);


    public List<Transform> transformParticlesOff = new List<Transform>();
    private bool canRotate = false;
    // Start is called before the first frame update
    void Start()
    {
        if(form != null)
        originalFormScale = form.gameObject.transform.localScale;
    }
    private void Update()
    {
        Rotate();
    }
    public void DecreaseChildrenScale(GameObject parent, float duration = 0.75f)
    {
        foreach (Transform child in parent.transform)
        {
            child.localScale = maxScale;
            Vector3 originalScale = child.localScale;
            Vector3 targetScale = originalScale * 0.1f;
            StartCoroutine(ScaleRoutine(child, originalScale, targetScale, duration));
        }
    }

    public void IncreaseChildrenScale(GameObject parent, float duration = 0.75f)
    {
        foreach (Transform child in parent.transform)
        {
            child.localScale = minScale;
            Vector3 currentScale = child.localScale;
            Vector3 targetScale = maxScale; 
            StartCoroutine(ScaleRoutine(child, currentScale, targetScale, duration));
        }
    }

    private IEnumerator ScaleRoutine(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            target.localScale = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = to;
    }
    public IEnumerator ActivateParticlesVFXcubeOn()
    {
        if (!particlesIn.isPlaying)
        {
            particlesIn.Play();

            ParticleSystem ps1 = form.transform.GetChild(0).GetComponent<ParticleSystem>();
            ParticleSystem ps2 = form.transform.GetChild(1).GetComponent<ParticleSystem>();
            ParticleSystem ps3 = form.transform.GetChild(2).GetComponent<ParticleSystem>();
           
            ps1.Play();
            ps2.Play();
            ps3.Play();
            IncreaseChildrenScale(form, 0.75f);
            yield return new WaitForSeconds(1.4f);
            particlesExplosion[0].Play();
            yield return new WaitForSeconds(0.2f);
            foreach (var item in particlesExplosionParticles)
            {
                item.Play();
            }
            yield return new WaitForSeconds(0.75f);
            ps1.Stop();
            ps2.Stop();
            ps3.Stop();

        } 
    }
    public IEnumerator ActivateParticlesVFXcubeOFF(Transform t)
    {
        if (transform.parent != t)
        {
            transform.SetParent(t.transform);
            transform.position = t.transform.position;
            yield return new WaitForSeconds(0.05f);
            foreach (var item in transformParticlesOff)
            {
                var a = item.GetComponent<ParticleSystem>();
                a.Play();
                canRotate = true;
                StartCoroutine(ScaleRoutine(item, maxScale, minScale, 0.75f));
                
            }
            yield return new WaitForSeconds(1f);
            canRotate = false;
            foreach (var item in transformParticlesOff)
            {
                item.SetParent(null);
                Destroy(item.gameObject);
            }
        }
        
    }
    public void Rotate()
    {
        if (!canRotate) return;
        if(transformParticlesOff.Count > 1)
        {
            for (int i = 0; i < transformParticlesOff.Count; i++)
            {
                if(i == 0)
                {
                    transformParticlesOff[i].Rotate(Vector3.up * 200 * Time.deltaTime);
                }
                else
                {
                    transformParticlesOff[i].Rotate(Vector3.right * 200 * Time.deltaTime);
                }
            }
        }
    }
    IEnumerator Rotate(Transform t)
    {
        t.Rotate(Vector3.up * 10 * Time.deltaTime);
        yield return new WaitForSeconds(3f);
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
