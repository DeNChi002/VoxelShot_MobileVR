using UnityEngine;
using System.Collections;

public class SimpleEffect : MonoBehaviorExpansion {

	[SerializeField]
	float waitTime;

	virtual protected void Start () {

		WaitAfter( waitTime, ()=> { Destroy(gameObject); } );
	}
}
