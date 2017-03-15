using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック:壊れるタイヤ
/// </summary>
public class GimmickBreakWheel : GimmickBase {

	[SerializeField]
	BoxCollider supportCollider;

	[SerializeField]
	MeshCollider meshCollider;

	protected override void ToBreak()
	{
		base.ToBreak();

		supportCollider.enabled = false;

		var rigid = gameObject.AddComponent<Rigidbody>();
		rigid.mass = 10;
	}
}
