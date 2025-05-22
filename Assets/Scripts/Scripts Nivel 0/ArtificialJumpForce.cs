using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NuevoInteractor;

public class ArtificialJumpForce : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float forwardForce = 5f;
    [SerializeField] private NewPhysicsBox box;
   
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(wait());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.5f);
        ApplyForce(box.rb);
    }
    private void ApplyForce(Rigidbody rb)
    {
        Vector3 jumpDirection = (transform.forward * forwardForce) + (transform.up * jumpForce);
        rb.AddForce(jumpDirection, ForceMode.VelocityChange);
        StartCoroutine(FreezeObject());
    }
    IEnumerator FreezeObject()
    {
        yield return new WaitForSeconds(0.04f);
        box.StatisEffectActivate();
    }
}
