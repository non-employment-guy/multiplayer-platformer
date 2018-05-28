using UnityEngine;

public class BuffObject : MonoBehaviour
{
    public BuffType Type;

    public bool IsActive;

    public float BuffValue;

    public float BuffDuration;

    public float RestorationTime;

    private float _currentRestorationTime;

    private Material _buffMaterial;

    void Start()
    {
        _buffMaterial = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (!IsActive)
        {
            _currentRestorationTime -= Time.deltaTime;
            if (_currentRestorationTime <= 0)
            {
                IsActive = true;
                ChangeAlpha(1f);
            }
        }
    }

    private void ChangeAlpha(float value)
    {
        var color = _buffMaterial.color;
        color.a = value;
        _buffMaterial.color = color;
    }

    public Buff Use()
    {
        IsActive = false;
        _currentRestorationTime = RestorationTime;
        ChangeAlpha(0.3f);
        return new Buff {Duration = BuffDuration, Value = BuffValue, Type = Type};
    }
}

public class Buff
{
    public BuffType Type;    

    public float Value;

    public float Duration;    
}

public enum BuffType
{
    Shield,
    Freeze,
    Damage
}