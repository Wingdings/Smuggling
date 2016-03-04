using UnityEngine;
using System.Collections;

public class UITweener : MonoBehaviour {

    public delegate void TweenDelegate();

    private Vector3 _initialPosition;
    private Vector3 _targetPosition;
    private float _positionTimeTarget;
    private float _positionTimeElapsed;
    private TweenDelegate _positionCallback;

    private Vector3 _initialScale;
    private Vector3 _targetScale;
    private float _scaleTimeTarget;
    private float _scaleTimeElapsed;
    private TweenDelegate _scaleCallback;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        
        float oldPosPercent = _positionTimeElapsed / _positionTimeTarget;
        if (oldPosPercent < 1)
        {
            _positionTimeElapsed += Time.deltaTime;
            float newPosPercent = _positionTimeElapsed / _positionTimeTarget;
            gameObject.transform.position = Vector3.Lerp(_initialPosition, _targetPosition, newPosPercent);
            
            if (newPosPercent >= 1)
            {
                _positionCallback();
            }
        }

        float oldScalePercent = _scaleTimeElapsed / _scaleTimeTarget;
        if (oldScalePercent < 1)
        {
            _scaleTimeElapsed += Time.deltaTime;
            float newScalePercent = _scaleTimeElapsed / _scaleTimeTarget;
            gameObject.transform.localScale = Vector3.Lerp(_initialScale, _targetScale, newScalePercent);

            if (newScalePercent >= 1)
            {
                _scaleCallback();
            }
        }
    }

    public void TweenPosition (Vector3 target, float time, TweenDelegate callback) {
        _positionTimeTarget = time;
        _positionTimeElapsed = 0f;

        _initialPosition = gameObject.transform.position;
        _targetPosition = target;

        _positionCallback = callback;
    }

    public void TweenScale(Vector3 target, float time, TweenDelegate callback)
    {
        _scaleTimeTarget = time;
        _scaleTimeElapsed = 0f;

        _initialScale = gameObject.transform.localScale;
        _targetScale = target;

        _scaleCallback = callback;
    }
}
