using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック:壊れるオブジェクト（ボックス形状）
/// </summary>
public class GimmickBreakBox : GimmickBase {

	[SerializeField, Header("損壊後の質量")]
	int settingMass = 10;

	protected override void ToBreak()
	{
		base.ToBreak();

		var rigid = gameObject.AddComponent<Rigidbody>();
		rigid.mass = settingMass;
	}
}
