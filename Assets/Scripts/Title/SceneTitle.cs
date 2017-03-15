using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルシーン管理クラス
/// </summary>
public class SceneTitle : MonoBehaviour {

	[SerializeField]
	GameObject eyeCamera;

	[SerializeField]
	GameObject subCamera;

	private void Start()
	{
		VR_AudioManager.Instance.PlayBGM(AUDIO_NAME.BGM_TITLE);
	}
}