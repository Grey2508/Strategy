using System.Collections;
using UnityEngine;

public class Mine : Building
{
    [Header("Mine")]

    public GameObject[] MiningIndicators;

    public bool IsMined;

    public int GoldPerTime = 1;
    public float Period = 2;

    public int MaxMiners = 3;

    private int _countMiners = 0;
    private float _timer;

    protected override void Prepaire()
    {
        base.Prepaire();

        for (int i = 0; i < MiningIndicators.Length; i++)
            MiningIndicators[i].SetActive(false);
    }

    public void IncCountMiners()
    {
        if (++_countMiners > 0)
            StartCoroutine(Mining());

        if (_countMiners > MaxMiners)
            _countMiners = MaxMiners;

        MiningIndicators[_countMiners - 1].SetActive(true);
    }
    public void DecCountMiners()
    {
        if (--_countMiners <= 0)
        {
            _countMiners = 0;

            StopCoroutine(Mining());
        }

        MiningIndicators[_countMiners].SetActive(false);
    }

    IEnumerator Mining()
    {
        while(true)
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
