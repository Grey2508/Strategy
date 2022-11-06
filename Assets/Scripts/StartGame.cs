using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public List<GameObject> EnemyCreatorsGroups;
    public float TimeToActiveEnemyCreators = 30;

    public PulseText AlarmText;

    private int _index = 0;

    void Start()
    {
        InvokeRepeating(nameof(ActiveEnemyCreators), TimeToActiveEnemyCreators, TimeToActiveEnemyCreators);
    }

    void ActiveEnemyCreators()
    {
        AlarmText.StartEffect();

        EnemyCreatorsGroups[_index++].SetActive(true);

        if (_index >= EnemyCreatorsGroups.Count)
            CancelInvoke(nameof(ActiveEnemyCreators));
    }
}
