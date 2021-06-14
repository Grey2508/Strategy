using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public List<EnemyCreator> EnemyCreators;
    public float TimeToActiveEnemyCreators = 30;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ActiveEnemyCreators", TimeToActiveEnemyCreators);
    }

    void ActiveEnemyCreators()
    {
        for (int i = 0; i < EnemyCreators.Count; i++)
            EnemyCreators[i].gameObject.SetActive(true);
    }
}
