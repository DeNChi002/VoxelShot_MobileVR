using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 野営地エリアシーン管理クラス
/// </summary>
public class SceneCamp : MonoBehaviour {

	void Start()
	{
		VR_AudioManager.Instance.PlayBGM(AUDIO_NAME.BGM_MAIN);
	}
}
