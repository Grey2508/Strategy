using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform ScaleTransform;
    public Transform Target;
    public float Height;

    private Transform _cameraTransform;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (!Target)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Target.position + Vector3.up * Height;

        transform.rotation = _cameraTransform.rotation;
    }

    public void Setup(Transform target, float height = 2)
    {
        Target = target;
        Height = height;
    }

    public void SetHealth(int health, int maxHealth)
    {
        float xScale = (float)health / maxHealth;
        xScale = Mathf.Clamp01(xScale);

        ScaleTransform.localScale = new Vector3(xScale, 1, 1);
    }
}
