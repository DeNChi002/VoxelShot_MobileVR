using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム開始時カウントダウン演出
/// </summary>
public class EffectGameStart : SimpleEffect {

	public void OnCall_CountDownSE()
	{
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_COUNT_DOWN, ControllerManager.Instance.EyeCameraObj.transform.position);
	}

	public void OnCall_StartSE()
	{
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GAME_START, ControllerManager.Instance.EyeCameraObj.transform.position);
	}
}
