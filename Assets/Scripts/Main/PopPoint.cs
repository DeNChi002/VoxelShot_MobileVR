using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 敵出現ポイント
/// </summary>
public class PopPoint : MonoBehaviorExpansion {

	[SerializeField, Header("目標地点選出タイプ")]
	SelectType selectType;

	[SerializeField, Header("開始時待ち時間")]
	float startWaitTime = 0.0f;

	[SerializeField, Header("消滅時間(0.0f=永続)")]
	float endTime = 0.0f;

	[SerializeField, Header("攻撃設定")]
	bool isAttacker = true;

	[SerializeField, Header("出現間隔")]
	float popInterval = 0.0f;

	[SerializeField, Header("最大同時出現数")]
	int maxEnemy = 10;

	[SerializeField, Header("ボーナス加算スコア")]
	int bonusScore = 0;

	[SerializeField]
	GameObject[] arrayMoveTarget;

    [SerializeField]
    GameObject[] arrayEnemyPrefab;

	List<GameObject> listEnemys;

	public enum SelectType
	{	// 目標決定のタイプ
		RANDOM	= 0,
		CLOSEST = 1,
	}

	bool isLimit = false;
    
    void CallRepop ()
    {
		if (listEnemys.Count < maxEnemy)
		{
			StartCoroutine(_Pop());
		}
		else 
		{
			isLimit = true;
		}
    }

	void Start () 
	{
		listEnemys = new List<GameObject>();

		WaitAfter(startWaitTime, () => { 
			StartCoroutine(_Pop());
		});

		if (endTime > 0.0f)
		{
			WaitAfter(endTime,
				() =>
				{
					for (int i = 0; i < listEnemys.Count; ++i)
					{
						if (listEnemys[i] != null)
						{
							Destroy(listEnemys[i]);
						}
					}
					Destroy(gameObject);
				}
			);
		}
	}

	void Update()
	{
		for (int i = 0; i < listEnemys.Count; ++i)
		{
			if(listEnemys[i] == null)
			{
				listEnemys.RemoveAt(i);
			}
		}

		if (isLimit)
		{
			if (listEnemys.Count < maxEnemy)
			{
				isLimit = false;
				CallRepop();
			}
		}
	}

    IEnumerator _Pop()
    {
        var enemy = Instantiate(arrayEnemyPrefab[ Random.Range(0, arrayEnemyPrefab.Length) ], transform.position, Quaternion.identity) as GameObject;

		var minIdx = 0;

		switch (selectType)
		{
			case SelectType.RANDOM:
				// ランダム
				minIdx = Random.Range(0, arrayMoveTarget.Length);
				break;
			case SelectType.CLOSEST:
				// 一番近いところを探す
				for (int i = 0; i < arrayMoveTarget.Length; i++)
				{
					if (Vector3.Distance(arrayMoveTarget[i].transform.position, transform.position) <
						Vector3.Distance(arrayMoveTarget[minIdx].transform.position, transform.position))
					{
						minIdx = i;
					}
				}
				break;
		}

		// 移動目標とボーナススコアを設定
		enemy.GetComponent<EnemyBase>().SetPopData( arrayMoveTarget[minIdx].transform, bonusScore, isAttacker);

		listEnemys.Add(enemy);

        yield return new WaitForSeconds( Random.Range(popInterval, popInterval + 3.0f) );

        CallRepop();
    }
}
