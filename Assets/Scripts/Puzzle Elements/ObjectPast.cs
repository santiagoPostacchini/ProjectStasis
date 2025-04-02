using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPast : MonoBehaviour
{
    public ObjectFuture objectFuture;
    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.velocity != Vector3.zero)
        {
            if(objectFuture.gameObject.activeSelf) objectFuture.gameObject.SetActive(false);
        } 
        else
        {
            if (objectFuture.gameObject.activeSelf) return;
            objectFuture.gameObject.SetActive(true);
            UpdateObjectFuture();
        }
    }
    public void UpdateObjectFuture()
    {
        objectFuture.transform.position = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        objectFuture.transform.rotation = transform.rotation;
    }
}
