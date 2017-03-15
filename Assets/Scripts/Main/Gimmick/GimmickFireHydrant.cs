using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック:消火栓
/// </summary>
public class GimmickFireHydrant : GimmickBase {

	static readonly float BREAK_DIR_POWER = 10.0f;

	[SerializeField]
	Transform root;

	[SerializeField] // 水エフェクト
	GameObject prefabEffectWater;

	protected override void ToBreak()
	{
		base.ToBreak();

		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_SPLASH, transform.position);

		GameObject obj = Instantiate(prefabEffectWater, root.position, prefabEffectWater.transform.localRotation, root.transform.parent ) as GameObject;
		ParticleSystem psWater = obj.GetComponent<ParticleSystem>();

		psWater.Play();

		var rigid = GetComponent<Rigidbody>();


		var forceDir = (transform.position - posGenDamage).normalized;
		
		forceDir.x = forceDir.x * BREAK_DIR_POWER;
		forceDir.y = (forceDir.y + Random.Range(5.0f, 5.5f)) * BREAK_DIR_POWER;
		forceDir.z = forceDir.z * BREAK_DIR_POWER;

		rigid.AddForce( forceDir, ForceMode.Impulse );

		WaitAfter( 7.0f, ()=> {
			psWater.Stop();
		} );

		WaitAfter(10.0f, () => {
			Destroy(obj);
		});
	}
}
