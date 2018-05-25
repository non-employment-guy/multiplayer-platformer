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
	void LateUpdate()
	{
	    if (TargeTransform == null) return;
	    var pos = new Vector3(TargeTransform.position.x, TargeTransform.position.y, ZOffset);
	    _transform.position = pos;

	}
}
