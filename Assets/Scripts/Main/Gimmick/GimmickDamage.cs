using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵を吹き飛ばしていくギミック
/// </summary>
public class GimmickDamage : GimmickBase {

	// 吹き飛ばしの物理影響値
	static readonly float BOMB_FORCE = 4.0f;

	void OnCollisionEnter(Collision collision)
	{
		IDamageable<int> i_damage = collision.gameObject.GetComponent<IDamageable<int>>();
		if (i_damage != null)
		{
			i_damage.BlowingOff( transform.position, BOMB_FORCE);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		IDamageable<int> i_damage = other.gameObject.GetComponent<IDamageable<int>>();
		if (i_damage != null)
		{
			i_damage.BlowingOff(transform.position, BOMB_FORCE);
		}
	}
}
