using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 空港エリアシーン管理クラス
/// </summary>
public class SceneAirport : MonoBehaviour {
	
	void Start () {
		VR_AudioManager.Instance.PlayBGM(AUDIO_NAME.BGM_MAIN);
	}
}
