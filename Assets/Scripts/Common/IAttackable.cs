using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 他への損傷攻撃可能オブジェクト
/// </summary>
interface IAttackable<T> {
	// 損傷付与値の取得
	int GetAttackValue();
}
