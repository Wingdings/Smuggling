using UnityEngine;
using System.Collections;

public class UITweener : MonoBehaviour {

    public delegate void TweenDelegate();

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float time, TweenDelegate callback)
    {
        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / time;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }
        callback();
        //return null;
    }

    IEnumerator SmoothScale(Vector3 startPos, Vector3 endPos, float time, TweenDelegate callback)
    {
        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / time;
            transform.localScale = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }
        callback();
        //return null;
    }

    public void TweenPosition (Vector3 target, float time, TweenDelegate callback)
    {
        StartCoroutine(SmoothMove(gameObject.transform.position, target, time, callback));
        
    }

    public void TweenScale(Vector3 target, float time, TweenDelegate callback)
    {
        StartCoroutine(SmoothScale(gameObject.transform.localScale, target, time, callback));
        
    }

    public void BounceScale(TweenDelegate callback)
    {
        StartCoroutine(SmoothScale(gameObject.transform.localScale, new Vector3(1.1f,1.1f,1f), .15f, delegate()
        {
            StartCoroutine(SmoothScale(gameObject.transform.localScale, new Vector3(1f, 1f, 1f), .15f, callback));
        }));
    }

    public static float easeInQuad(float t, float b, float c, float d) { return c * (t /= d) * t + b; }
    public static float easeOutQuad(float t, float b, float c, float d) { return -c * (t /= d) * (t - 2) + b; }
}
