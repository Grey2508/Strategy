using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Building
{
    public GameObject MiningIndicator;
    
    public bool IsMined;

    public int GoldPerTime = 1;
    public float Period = 2;

    public int MaxMiners = 3;

    private int _countMiners = 0;
    private float _timer;

    public override void Start()
    {
        base.Start();

        MiningIndicator.SetActive(false);
    }
    
    public void ToggleIndicator(bool stage)
    {
        MiningIndicator.SetActive(stage);
    }

    public void IncCountMiners()
    {
        if (++_countMiners > 0)
            StartCoroutine("Mining");

        if (_countMiners > MaxMiners)
            _countMiners = MaxMiners;
    }
    public void DecCountMiners()
    {
        if(--_countMiners<=0)
        {
            _countMiners = 0;
            
            ToggleIndicator(false);

            StopCoroutine("Mining");
        }
    }

    IEnumerator Mining()
    {
        ToggleIndicator(true);

        for (; ; )
        {
            _timer += Time.deltaTime;

            if (_timer >= Period)
            {
                Resources.Gold += GoldPerTime * _countMiners;
                _timer = 0;
            }

            yield return null;
        }
    }
}
