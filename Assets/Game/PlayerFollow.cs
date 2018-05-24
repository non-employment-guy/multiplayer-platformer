using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{

    public Transform TargeTransform;

    public float ZOffset;

    private Transform _transform;

	// Use this for initialization
    void Start()
    {
        _transform = transform;
    }

    // Update is called once per frame
	void FixedUpdate()
	{
	    if (TargeTransform == null) return;
	    var pos = new Vector3(TargeTransform.position.x, _transform.position.y, ZOffset);
	    _transform.position = Vector3.Lerp(_transform.position, pos, Time.deltaTime);
	}
}
