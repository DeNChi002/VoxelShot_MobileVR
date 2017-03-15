using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム終了時カウントダウン演出
/// </summary>
public class EffectGameEnd : SimpleEffect {

	public void OnCall_CountDownSE()
	{
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_COUNT_DOWN, ControllerManager.Instance.EyeCameraObj.transform.position);
	}

	public void OnCall_EndSE()
	{
		VR_AudioManager.Instance.StopBGM();
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GAME_END, ControllerManager.Instance.EyeCameraObj.transform.position);
	}
}
