using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float RaiseTime = 2f;

    private float _timer = 0f;
    private Vector3 _initPosition;
    private bool _raised = false;

    // Start is called before the first frame update
    void Start()
    {
        _initPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RaiseDoor()
    {
        if (!_raised)
            StartCoroutine(RaiseRoutine());
    }

    public void LowerDoor()
    {
        if (_raised)
            StartCoroutine(LowerRoutine());
    }

    private IEnumerator RaiseRoutine()
    {
        while (_timer < RaiseTime)
        {
            float lerpRatio = _timer / RaiseTime;

            var newPos = Vector3.Lerp(_initPosition, _initPosition + Vector3.up * 2.2f, lerpRatio);

            transform.position = newPos;

            _timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _raised = true;
        _timer = 0f;
    }

    private IEnumerator LowerRoutine()
    {
        while (_timer < RaiseTime)
        {
            float lerpRatio = _timer / RaiseTime;

            var newPos = Vector3.Lerp(_initPosition + Vector3.up * 2.2f, _initPosition, lerpRatio);

            transform.position = newPos;

            _timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _raised = false;
        _timer = 0f;
    }
}
