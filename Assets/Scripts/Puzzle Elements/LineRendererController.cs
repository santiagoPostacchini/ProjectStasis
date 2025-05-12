using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField] private Transform[] _criystalTransform;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _lineRenderer.positionCount = _criystalTransform.Length;
        for (int i = 0; i < _criystalTransform.Length; i++)
        {
            _lineRenderer.SetPosition(i, _criystalTransform[i].position);
        }
    }
}
