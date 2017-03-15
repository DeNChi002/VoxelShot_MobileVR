using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 視線による銃操作
/// </summary>
public class ControllerLineOfSight : MonoBehaviour {

	// 射撃間隔
	static readonly float RAPPID_INTERVAL = 0.1f;

	[SerializeField]
	GunBase currentGun;

	float rappidTime;

	void LateUpdate()
	{
		RaycastHit[] hits;
		hits = Physics.RaycastAll(transform.position, transform.forward, 100.0f);

		for (int i = 0; i < hits.Length; ++i)
		{
			RaycastHit hit = hits[i];

			if (hit.transform.tag.Contains(TAG_NAME.TAG_ENEMY))
			{
				rappidTime = Mathf.Clamp( rappidTime - Time.deltaTime, 0.0f, RAPPID_INTERVAL );
				if (rappidTime <= 0.0f)
				{
					if (currentGun.IsReload)
					{   // 空撃ち時
						VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_BLANK_TRIGGER, transform.position);
					}
					else 
					{
						currentGun.CallTrigger();
					}
					rappidTime = RAPPID_INTERVAL;
				}
				break;
			}
		}
	}
}
