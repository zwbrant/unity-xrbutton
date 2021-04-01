using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButton : XRButton
{
    [Header("Component Dependencies")]
    public Transform Button;
    public SpringJoint SpringJoint;
    public Renderer ProgressRenderer;

    [Header("Buttom Parameters")]
    [Range(1f, 5f)]
    public float ThrowDepth = 1f;
    [Range(0f, 100f)]
    public float ActivationDuration = 1f;

    public Color ProgressRadialColor = Color.green;
    public Material ActiveMaterial;
    public Material InactiveMaterial;

    public override bool IsDown { get => _isDown; }

    private bool _isDown = false;
    private bool _bttnReset = true;
    private float _timeDown = -1f;

    private Rigidbody _buttonRbody;
    private MeshCollider _collider;
    private float _initButtonLocalY;
    private float _initSpringPower;

    void Start()
    {
        // initialize references 
        _initButtonLocalY = Button.localPosition.y;
        _initSpringPower = SpringJoint.spring;
        _buttonRbody = Button.GetComponent<Rigidbody>();
        ProgressRenderer.sharedMaterial.SetColor("_Color", ProgressRadialColor);
        _collider = Button.GetComponent<MeshCollider>();
    }

    void Update()
    {
        if (Button.localPosition.y == _initButtonLocalY)
            return;

        CheckIfDown();
        UpdateProgressRadial();
    }

    private void FixedUpdate()
    {
        StopAtTopOfThrow();
    }

    private void CheckIfDown()
    {
        var downPosition = _initButtonLocalY - ThrowDepth.InchesToMeters();

        // button reached activation depth
        if (!_isDown && _bttnReset && Button.localPosition.y <= downPosition)
            ButtonDown();

        // activation duration elapsed
        if (_isDown && Time.time >= _timeDown + ActivationDuration)
            ButtonUp();
    }

    private void StopAtTopOfThrow()
    {
        // button has reached top of throw, force it to stop
        if (Button.localPosition.y >= _initButtonLocalY)
            StopAndResetButton();
        // restore spring strength if it was disabled
        else if (SpringJoint.spring != _initSpringPower)
            SpringJoint.spring = _initSpringPower;
    }

    // force rigidbody to stop moving at top
    private void StopAndResetButton()
    {
        _buttonRbody.velocity = Vector3.zero;
        SpringJoint.spring = 0;
        Button.localPosition =
            new Vector3(Button.localPosition.x, _initButtonLocalY, Button.localPosition.z);

        if (!_bttnReset)
        {
            //print("Button Reset");
            _bttnReset = true;
            _collider.enabled = true;
        }
    }

    protected override void ButtonDown()
    {
        base.ButtonDown();

        _isDown = true;
        _bttnReset = false;
        _collider.enabled = false;

        Button.GetComponent<MeshRenderer>().material = ActiveMaterial;

        // freeze button and ensure it's no deeper than it should be
        _buttonRbody.constraints = RigidbodyConstraints.FreezeAll;
        Button.localPosition = new Vector3(Button.localPosition.x,
            _initButtonLocalY - ThrowDepth.InchesToMeters(),
            Button.localPosition.z);

        // if duration > zero
        _timeDown = Time.time;
    }

    protected override void ButtonUp()
    {
        base.ButtonUp();

        _isDown = false;

        Button.GetComponent<MeshRenderer>().material = InactiveMaterial;

        // allow button to physically reset
        _buttonRbody.constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotation |
            RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
    }

    // update shader _cutoff property to visualize press amount
    private void UpdateProgressRadial()
    {
        // if the button is no longer active, but it hasn't returned to the top position, set radial to zero progress
        if (!_isDown && !_bttnReset)
        {
            ProgressRenderer.sharedMaterial.SetFloat("_Cutoff", 0f);
            return;
        }

        var progress = Mathf.Abs(_initButtonLocalY - Button.localPosition.y) / ThrowDepth.InchesToMeters();

        progress = (float)System.Math.Round(progress, 2);

        ProgressRenderer.sharedMaterial.SetFloat("_Cutoff", progress);
    }

    /// <summary>
    /// Change the button's physical state. Button is "down" if press amount reaches 1.
    /// If button is already is down and hasn't reset, press amount won't be set.
    /// </summary>
    /// <param name="pressedAmount">Clamped to 1-0. 1 = fully pressed (activated), 0 = not pressed </param>
    public void SetButtonPressed(float pressedAmount)
    {
        if (_isDown)
            return;

        pressedAmount = Mathf.Clamp(pressedAmount, 0, 1);

        var pressedDistance = pressedAmount * ThrowDepth.InchesToMeters();

        Button.localPosition = new Vector3(Button.localPosition.x, _initButtonLocalY - pressedDistance, Button.localPosition.z);
    }
}
