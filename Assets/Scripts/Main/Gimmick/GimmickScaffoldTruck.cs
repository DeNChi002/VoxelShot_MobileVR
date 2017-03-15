using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック:野営地足場トラック
/// </summary>
public class GimmickScaffoldTruck : GimmickBase {

	[SerializeField]
	Animator truckAnimator;

	[SerializeField]
	Transform truckTransform;

	[SerializeField]
	Transform engineRoom;

	public void OnCall_MoveEnd()
	{
		VR_AudioManager.Instance.StopSE(AUDIO_NAME.SE_DRIVING);
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_CAR_BRAKE, transform.position, 1.0f, 1.0f);
	}

	public void OnCall_MoveStart()
	{
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_DRIVING, transform.position, 1.0f, 1.0f, null, false, true);
	}

	protected override void GimmickStart()
	{
		base.GimmickStart();

		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_CAR_ENGINE,engineRoom.position, 1.0f, 1.0f);

		truckAnimator.enabled = true;
	}
}
