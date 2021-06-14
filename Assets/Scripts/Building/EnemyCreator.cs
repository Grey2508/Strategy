using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    public float Period = 5;
    public GameObject EnemyPrefab;

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;

        if(_timer>Period)
        {
            Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
            _timer = 0;
        }
    }
}
