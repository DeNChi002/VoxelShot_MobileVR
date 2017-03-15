using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// ロゴ表示シーン
/// </summary>
public class SceneLogo : SingletonMonoBehaviour<SceneLogo> {

	public Image logoImage;

	void Start()
	{
#if !UNITY_EDITOR
		Debug.logger.logEnabled = false;
#endif
		StartCoroutine(LogoSound());
	}

    private IEnumerator LogoSound()
    {
        yield return new WaitForSeconds(1.0f);
        VR_AudioManager.instance.PlaySE(AUDIO_NAME.SE_LOGOSATBOXBEFORE, Vector3.zero);
        yield return new WaitForSeconds(2.0f);
        VR_AudioManager.instance.PlaySE(AUDIO_NAME.SE_LOGOSATBOX, Vector3.zero);
        yield return new WaitForSeconds(3.0f);

		VR_SceneChangeManager.instance.LoadLevel(SCENE_NAME.SCENE_TITLE);
    }
}
