using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using VRTK;

/// <summary>
/// 攻撃ヘリミサイル
/// </summary>
public class GimmickHeliMissile : GimmickBase {

	// 有効射程
	protected static readonly float EFFECTIVE_RANGE = 100.0f;

	[SerializeField]
	ParticleSystem psFire;

	[SerializeField]
	Transform centerAnchor;

	[SerializeField]
	protected ParticleSystem prefabExplosion;

	public void Shot( Transform _heliTransform, float _delay )
	{
		//var diff = (_heliTransform.position - transform.position) + (centerAnchor.position - transform.position);
		//var playerPos = VRTK_SDK_Bridge.GetPlayArea().position - diff * 0.25f;

		//iTween.MoveTo(gameObject,
		//			  iTween.Hash("x", playerPos.x, "y", playerPos.y, "z", playerPos.z,
		//                  "easeType", iTween.EaseType.easeInQuad,
		//                  "time", 5.0f, "delay", _delay));

		//psFire.Play();
	}

	void OnCollisionEnter(Collision col)
	{
	}

	void OnTriggerEnter(Collider other)
	{
		var obj = Instantiate(prefabExplosion, transform.position, prefabExplosion.transform.rotation);
		obj.name = "Missile_Explosion";
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 20.0f, 1.0f);

		HealthDisp.Instance.Sub();

		Destroy(gameObject);
	}
}
