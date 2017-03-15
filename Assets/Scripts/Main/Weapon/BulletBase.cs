using UnityEngine;
using System.Collections;

/// <summary>
/// 弾丸基本クラス
/// </summary>
public class BulletBase : MonoBehaviorExpansion {

	static readonly int BASE_POWER = 1;
	static readonly float CLOSE_RANGLE = 12.25f;

	protected int bulletPower = BASE_POWER;

	public void Set( Vector3 _endPos )
	{
		Instantiate(Resources.Load("FX_BulletLanding"), _endPos, Quaternion.LookRotation(transform.position - _endPos));

		float landingTime =  Mathf.Clamp( Vector3.Distance(transform.position, _endPos) / 100.0f, 0.0f, 0.1f );

		iTween.MoveTo(gameObject, iTween.Hash( "position", _endPos, "time", landingTime, "easeType", iTween.EaseType.linear));

		WaitAfter(landingTime + 0.1f, () => { Destroy(gameObject); });
	}
}
