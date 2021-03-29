using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRButton : MonoBehaviour
{
    [SerializeField]
    public XRButtonEvent OnDown;
    [SerializeField]
    public XRButtonEvent OnUp;

    [Header("Component Dependencies")]
    public Transform Button;
    public SpringJoint SpringJoint;
    public Renderer ProgressRenderer;
    public AudioSource DownSound;
    public AudioSource UpSound;


    [Header("Buttom Parameters")]
    [Range(1f, 25f)]
    public float ThrowDepth = 15f;
    [Range(0f, 100f)]
    public float ActivationDuration = 1f;

    public Color ProgressRadialColor = Color.green;
    public Material ActiveMaterial;
    public Material InactiveMaterial;

    public bool IsDown
    {
        get => _activated;
    }

    private bool _activated = false;
    private bool _bttnReset = true;
    private float _activationTime = -1f;

    private Rigidbody _switchRbody;
    private MeshCollider _collider;
    private float _initSwitchLocalY;
    private float _initSpringPower;


    // Start is called before the first frame update
    void Start()
    {
        _initSwitchLocalY = Button.localPosition.y;
        _initSpringPower = SpringJoint.spring;
        _switchRbody = Button.GetComponent<Rigidbody>();
        ProgressRenderer.sharedMaterial.SetColor("_Color", ProgressRadialColor);
        _collider = Button.GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        var activationHeight = _initSwitchLocalY - InchesToScaledMeters(ThrowDepth);

        UpdateProgressRadial();

        // button reached activation depth
        if (!_activated && _bttnReset && Button.localPosition.y <= activationHeight)
            Activate();

        // activation duration elapsed
        if (_activated && Time.time >= _activationTime + ActivationDuration)
            Inactivate();
    }

    private void FixedUpdate()
    {
        // button has reached top of throw, force it to stop
        if (Button.localPosition.y >= _initSwitchLocalY)
            StopAndResetButton();
        // restore spring strength if it was disabled
        else if (SpringJoint.spring != _initSpringPower)
            SpringJoint.spring = _initSpringPower;
    }

    // force rigidbody to stop moving at top
    private void StopAndResetButton()
    {
        _switchRbody.velocity = Vector3.zero;
        SpringJoint.spring = 0;
        Button.localPosition =
            new Vector3(Button.localPosition.x, _initSwitchLocalY, Button.localPosition.z);

        if (!_bttnReset)
        {
            print("Button Reset");
            _bttnReset = true;
            _collider.enabled = true;
        }
    }

    private void Activate()
    {
        print("Button Activated");

        OnDown.Invoke();
        if (DownSound != null)
            DownSound.Play();

        _activated = true;
        _bttnReset = false;
        _collider.enabled = false;

        Button.GetComponent<MeshRenderer>().material = ActiveMaterial;

        // freeze button and ensure it's no deeper than it should be
        _switchRbody.constraints = RigidbodyConstraints.FreezeAll;
        Button.localPosition = new Vector3(Button.localPosition.x,
            _initSwitchLocalY - InchesToScaledMeters(ThrowDepth),
            Button.localPosition.z);

        // if duration > zero
        _activationTime = Time.time;
    }

    private void Inactivate()
    {
        print("Button Inactivated");

        OnUp.Invoke();
        if (UpSound != null)
            UpSound.Play();

        _activated = false;

        Button.GetComponent<MeshRenderer>().material = InactiveMaterial;

        // allow button to physically reset
        _switchRbody.constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotation |
            RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
    }

    // update shader _cutoff property to visualize press amount
    private void UpdateProgressRadial()
    {
        // if the button is no longer active, but it hasn't returned to the top position, set radial to zero progress
        if (!_activated && !_bttnReset)
        {
            ProgressRenderer.sharedMaterial.SetFloat("_Cutoff", 0f);
            return;
        }

        var progress = Mathf.Abs(_initSwitchLocalY - Button.localPosition.y) / InchesToScaledMeters(ThrowDepth) /** Switch.lossyScale.y*/;

        progress = (float)System.Math.Round(progress, 2);

        ProgressRenderer.sharedMaterial.SetFloat("_Cutoff", progress);
    }

    /// <summary>
    /// Change the button's physical state. Button will activate if fully pressed.
    /// If button is already activated, press amount won't be set.
    /// </summary>
    /// <param name="pressedAmount">Clamped to 1-0. 1 = fully pressed (activated), 0 = not pressed </param>
    public void SetButtonPressed(float pressedAmount) {

        if (_activated)
            return;

        pressedAmount = Mathf.Clamp(pressedAmount, 0, 1);

        var pressedDistance = pressedAmount * InchesToScaledMeters(ThrowDepth);

        Button.localPosition = new Vector3(Button.localPosition.x, _initSwitchLocalY - pressedDistance, Button.localPosition.z);
    }

    public float InchesToScaledMeters(float inches)
    {
        return inches / 39.3701f * Button.lossyScale.y;
    }
}
