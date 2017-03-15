using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック:突進トラック
/// </summary>
public class GimmickChargeTruck : GimmickBase {

	[SerializeField] // アニメーター
	Animator truckAnimator;

	[SerializeField]
	Transform truckTransform;

	[SerializeField]
	Rigidbody rigid;

	[SerializeField] // アニメ終了時の追加敵出現地点
	GameObject popPoint;

	[SerializeField] // 爆発エフェクト
	GameObject effectExplosion;

	[SerializeField] // 炎上エフェクト
	GameObject[] arrayFire;

	[SerializeField] // ヘッドライト
	GameObject[] arrayHeadLight;

	public void OnCall_PlayBrakeSE()
	{
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_TRUCK_BRAKE, transform.position, 40.0f);
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_TRUCK_BRAKE, transform.position, 40.0f);

		WaitAfter(
			3.0f, ()=> { 
				VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 40.0f, 1.0f);
			}
		);

		WaitAfter(
			3.1f, () =>
			{
				VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_TRUCK_GLASS_BREAK, transform.position, 40.0f, 1.0f);
			}
		);
	}

	void FixedUpdate()
	{
		rigid.MovePosition(truckTransform.position);
		rigid.MoveRotation(truckTransform.rotation);
	}

	protected override void GimmickStart()
	{
		base.GimmickStart();

		//DangerDisp.Instance.Set( truckTransform.gameObject, 5.0f );

		foreach (GameObject go in arrayHeadLight)
		{
			go.SetActive(true);
		}

		truckAnimator.enabled = true;

		WaitAfter(truckAnimator.GetCurrentAnimatorStateInfo(0).length - 0.1f, () => { 

			VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 20.0f, 1.0f);
			VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 20.0f, 1.0f);
			effectExplosion.SetActive(true); 
		});
		WaitAfter(truckAnimator.GetCurrentAnimatorStateInfo(0).length - 0.1f, () => { foreach (GameObject go in arrayHeadLight) { go.SetActive(false); } });

		WaitAfter(truckAnimator.GetCurrentAnimatorStateInfo(0).length + 0.5f, () => { 

			foreach (GameObject go in arrayFire) { go.SetActive(true); }
			popPoint.SetActive(true);
		});
	}
}
