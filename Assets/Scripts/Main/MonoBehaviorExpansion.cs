using UnityEngine;
using System;
using System.Collections;

public class MonoBehaviorExpansion : MonoBehaviour {

    protected void WaitAfter( float _wait, Action _act )
    {
        if( _act != null)
        {
            StartCoroutine( _WaitAfter(_wait, _act) );
        }
    }

    IEnumerator _WaitAfter(float _wait, Action _act)
    {
        yield return new WaitForSeconds(_wait);

        _act();
    }
}
