using UnityEngine;
using System.Collections;
//using VRTK;

/// <summary>
/// コントローラー管理クラス
/// </summary>
public class ControllerManager : SingletonMonoBehaviour<ControllerManager> {

	[SerializeField] // 視点カメラ
	GameObject eyeCamera;
	[SerializeField] // プレイヤー追従UI用カメラ
	GameObject playerUICamera;

	[SerializeField] // 右手モデル
	SkinnedMeshRenderer rightModelMesh;
	[SerializeField] // 左手モデル
	SkinnedMeshRenderer leftModelMesh;

	public GameObject EyeCameraObj { get { return eyeCamera; } }
	public GameObject PlayerUICameraObj { get { return playerUICamera; } }

	public enum HandType {
		RIGHT	= 0,
		LEFT	= 1,
	};
	
	public void SetVisible ( GameObject _usingObject, bool _isVisible ){

		//if (VRTK_SDK_Bridge.IsControllerRightHand(_usingObject))
		//	rightModelMesh.enabled = _isVisible;
		//else
		//	leftModelMesh.enabled = _isVisible;
	}
}
