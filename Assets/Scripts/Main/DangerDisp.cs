using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// イベントに注視を促す表示
/// </summary>
public class DangerDisp : SingletonMonoBehaviour<DangerDisp> {

	[SerializeField]
	SpriteRenderer dangerIcon;

	[SerializeField]
	SpriteRenderer arrowRight;

	[SerializeField]
	SpriteRenderer arrowLeft;

	GameObject lookTarget;
	float lookTime;

	static readonly float LOOK_FOCUS_IN_ANGLE = 20.0f;

	public void Set( GameObject _target, float _time )
	{
		dangerIcon.enabled = true;

		lookTarget = _target;
		lookTime = _time;

		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GIMMICK_WARNING, ControllerManager.Instance.EyeCameraObj.transform.position);
	}

	public float GetAim(Vector3 p1, Vector3 p2)
	{
		float dx = p2.x - p1.x;
		float dy = p2.y - p1.y;
		float rad = Mathf.Atan2(dy, dx);
		return rad * Mathf.Rad2Deg;
	}

	/// <summary>
	/// 回転方向左右判別
	/// </summary>
	private bool _checkClockwise(float current, float target)
	{
		return target > current ? !(target - current > 180f)
							  : current - target > 180f;
	}

	void Update()
	{
		if (lookTime > 0.0f)
		{
			lookTime -= Time.deltaTime;

			Debug.Log("look_pos:" + lookTarget.transform.position);

			var dir = lookTarget.transform.position - transform.position;
			var cameraEulerY = transform.rotation.eulerAngles.y;
			dir = Quaternion.Euler(0.0f, - cameraEulerY, 0.0f) * dir;
			var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
			
			if (Mathf.Abs(angle) < LOOK_FOCUS_IN_ANGLE)
			{
				dangerIcon.enabled = false;
				arrowRight.enabled = false;
				arrowLeft.enabled = false;
			}
			else
			{
				dangerIcon.enabled = true;
				arrowRight.enabled = !_checkClockwise(angle, 0.0f);
				arrowLeft.enabled = _checkClockwise(angle, 0.0f);
			}

			if (lookTime <= 0.0f)
			{
				dangerIcon.enabled = false;
				arrowRight.enabled = false;
				arrowLeft.enabled = false;
			}
		}
	}
}
