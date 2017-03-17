using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ギミック:突進カー
/// </summary>
public class GimmickChargeCar : GimmickBase {

	[SerializeField] // アニメーター
	Animator carAnimator;

	[SerializeField]
	Transform carTransform;

	[SerializeField]
	Rigidbody rigid;

	//[SerializeField] // ヘッドライト
	//GameObject[] arrayHeadLight;

	[SerializeField]
	ParticleSystem[] arrayTireSmoke;

	//[SerializeField]
	//NavMeshObstacle navMeshObstacle;

	void FixedUpdate()
	{
		rigid.MovePosition(carTransform.position);
		rigid.MoveRotation(carTransform.rotation);
	}

	protected override void GimmickStart()
	{
		base.GimmickStart();
		
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_CAR_ENGINE_START, transform.position, 2.0f, 1.0f);
		
		foreach (ParticleSystem ps in arrayTireSmoke)
		{
			ps.Play();
		}

		//WaitAfter( 1.0f, () => {

		//	//foreach (GameObject go in arrayHeadLight)
		//	//{
		//	//	go.SetActive(true);
		//	//}
		//});

		WaitAfter( 2.0f, () => {

			//navMeshObstacle.enabled = false;
			carAnimator.enabled = true;
			VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_CAR_ROCKET_START, transform.position, 2.0f, 1.0f, gameObject);
		});

		WaitAfter( 10.0f, () => {

			Destroy(gameObject);
		});

		////DangerDisp.Instance.Set( truckTransform.gameObject, 5.0f );

		//foreach (GameObject go in arrayHeadLight)
		//{
		//	go.SetActive(true);
		//}

		//truckAnimator.enabled = true;

		//WaitAfter(truckAnimator.GetCurrentAnimatorStateInfo(0).length - 0.1f, () => {

		//	VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 20.0f, 1.0f);
		//	VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 20.0f, 1.0f);
		//	effectExplosion.SetActive(true);
		//});
		//WaitAfter(truckAnimator.GetCurrentAnimatorStateInfo(0).length - 0.1f, () => { foreach (GameObject go in arrayHeadLight) { go.SetActive(false); } });

		//WaitAfter(truckAnimator.GetCurrentAnimatorStateInfo(0).length + 0.5f, () => {

		//	foreach (GameObject go in arrayFire) { go.SetActive(true); }
		//	popPoint.SetActive(true);
		//});
	}
}
