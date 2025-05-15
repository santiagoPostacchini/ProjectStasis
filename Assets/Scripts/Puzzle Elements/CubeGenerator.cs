using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    private bool isColision = false;

    [SerializeField]private GameObject box;
    [SerializeField]private Transform posRespawn;
    [SerializeField] private Transform posAux;

    [SerializeField] private VFXCube vfxCubeOn;
    [SerializeField] private VFXCube vfxCubeOff;

    

    public int amount = 1;


    private void Update()
    {
        if (isColision)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                GenerateCube();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            isColision = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if(player != null)
        {
            isColision = false;
        }
    }

    public void GenerateCube()
    {
        StartCoroutine(Wait());

    }
    IEnumerator Wait()
    {
        box.GetComponent<Rigidbody>().useGravity = false;
        var CubeOff = Instantiate(vfxCubeOff, transform.position, transform.rotation);
        StartCoroutine(CubeOff.ActivateParticlesVFXcubeOFF(box.transform));
        StartCoroutine(ScaleRoutine(box.transform, vfxCubeOff.maxScale, vfxCubeOff.minScale, 0.75f));
        yield return new WaitForSeconds(1.2f);
        box.transform.position = posAux.position;
        Destroy(CubeOff);
        var CubeOn = Instantiate(vfxCubeOn, transform.position, transform.rotation);
        CubeOn.transform.position = posRespawn.transform.position;
        StartCoroutine(CubeOn.ActivateParticlesVFXcubeOn());
        yield return new WaitForSeconds(1.5f);
        box.transform.position = CubeOn.transform.position;
        box.transform.localScale = CubeOn.maxScale;
        box.GetComponent<Rigidbody>().useGravity = true;
        Destroy(CubeOn);
        //box.transform.position = posRespawn.position;
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
}
