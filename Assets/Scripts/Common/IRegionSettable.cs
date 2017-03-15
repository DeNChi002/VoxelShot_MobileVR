using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 部位設定有効オブジェクト
/// </summary>
public interface IRegionSettable {
	// 部位設定
	void SetRegionType( Region.TYPE _type );
}
