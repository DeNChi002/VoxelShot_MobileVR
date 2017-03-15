using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック:蛍光灯
/// </summary>
public class GimmickFluorescentLamp : GimmickBase {

	[SerializeField]
	MeshRenderer meshRenderer;

	protected override void ToBreak()
	{
		base.ToBreak();

		VR_AudioManager.Instance.PlaySE( AUDIO_NAME.SE_GLASS_BREAK, transform.position);

		meshRenderer.material.SetColor("_Color", new Color(0.5f,0.5f,0.5f,1.0f));
		meshRenderer.material.SetColor("_EmissionColor", new Color( 0.3f, 0.3f, 0.3f, 1.0f ));

		var resEffect = Resources.Load("FX_GlassBreak") as GameObject;
		var instanceEffect = Instantiate(resEffect) as GameObject;

		instanceEffect.transform.parent = transform;
		instanceEffect.transform.localPosition = resEffect.transform.localPosition;
		instanceEffect.transform.localScale = resEffect.transform.localScale;
	}
}
