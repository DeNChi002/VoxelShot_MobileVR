using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 軍事基地エリアシーン管理クラス
/// </summary>
public class SceneBase : MonoBehaviour {
	
	void Start () {
		VR_AudioManager.Instance.PlayBGM(AUDIO_NAME.BGM_MAIN);
	}
}
