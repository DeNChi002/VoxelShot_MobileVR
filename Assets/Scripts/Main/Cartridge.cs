using UnityEngine;
using System.Collections;

/// <summary>
/// 薬莢
/// </summary>
public class Cartridge : MonoBehaviorExpansion {

	[SerializeField, Header("ランダム移動値（上限）")]
	Vector3 vecMaxRandom = Vector3.one;

	[SerializeField, Header("ランダム移動値（下限）")]
	Vector3 vecMinRandom = Vector3.one;

	[SerializeField]
	Rigidbody rigid;

	static readonly float LIFE_TIME = 2.0f;

	void Start () {

		Vector3 dir = new Vector3( Random.Range(vecMinRandom.x, vecMaxRandom.x), Random.Range(vecMinRandom.y, vecMaxRandom.y), Random.Range(vecMinRandom.z, vecMaxRandom.z) );
		rigid.AddForce( dir * 1.0f );

		WaitAfter( LIFE_TIME, ()=> { Destroy(gameObject); } );
	}
}
