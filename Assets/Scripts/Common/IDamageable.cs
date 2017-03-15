using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 損傷オブジェクト
/// </summary>
public interface IDamageable<T> {
	// 損傷時
	void Damage(T _damageValue);
	// ダメージ発生源位置, 移動影響値
	void SetGenPos(Vector3 _pos, float _power);
	// 吹き飛ばし
	void BlowingOff(Vector3 _pos, float _power);
}
