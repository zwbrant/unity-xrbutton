using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRButton : XRBaseInteractable
{
    [SerializeField]
    public XRButtonEvent OnDown;
    [SerializeField]
    public XRButtonEvent OnUp;

    [Header("Component Dependencies")]
    public Transform Switch;
    public SpringJoint SpringJoint;
    public Renderer ProgressRenderer;
    public AudioSource DownSound;
    public AudioSource UpSound;
    [Tooltip("Axis by which button can be pressed when hovered")]
    public InputActionReference PressAxis;

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
        _initSwitchLocalY = Switch.localPosition.y;
        _initSpringPower = SpringJoint.spring;
        _switchRbody = Switch.GetComponent<Rigidbody>();
        ProgressRenderer.sharedMaterial.SetColor("_Color", ProgressRadialColor);
        _collider = Switch.GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        var activationHeight = _initSwitchLocalY - InchesToScaledMeters(ThrowDepth);

        if (isHovered && _bttnReset && !_activated)
            ReceiveTriggerInput();

        // update progress shader
        var progress = Mathf.Abs(_initSwitchLocalY - Switch.localPosition.y) / InchesToScaledMeters(ThrowDepth) /** Switch.lossyScale.y*/;
        ProgressRenderer.sharedMaterial.SetFloat("_Cutoff", progress);

        // button reached activation depth
        if (!_activated && _bttnReset && Switch.localPosition.y <= activationHeight)
            Activate();

        // activation duration elapsed
        if (_activated && Time.time >= _activationTime + ActivationDuration)
            Inactivate();
    }



    private void FixedUpdate()
    {
        // button has reached top of throw, force it to stop
        if (Switch.localPosition.y >= _initSwitchLocalY)
        {
            _switchRbody.velocity = Vector3.zero;
            SpringJoint.spring = 0;
            Switch.localPosition = 
                new Vector3(Switch.localPosition.x, _initSwitchLocalY, Switch.localPosition.z);

            if (!_bttnReset)
            {
                print("Button Reset");
                _bttnReset = true;
                _collider.enabled = true;
            }

        }
        // reset spring if we disabled it
        else if (SpringJoint.spring != _initSpringPower)
        {
            SpringJoint.spring = _initSpringPower;
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

        Switch.GetComponent<MeshRenderer>().material = ActiveMaterial;

        // freeze button and ensure it's no deeper than it should be
        _switchRbody.constraints = RigidbodyConstraints.FreezeAll;
        Switch.localPosition = new Vector3(Switch.localPosition.x,
            _initSwitchLocalY - InchesToScaledMeters(ThrowDepth),
            Switch.localPosition.z);

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

        Switch.GetComponent<MeshRenderer>().material = InactiveMaterial;

        // allow button to physically reset
        _switchRbody.constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotation |
            RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;

    }

    private void ReceiveTriggerInput()
    {
        var axisValue = PressAxis.action.ReadValue<float>();

        if (axisValue <= 0)
            return;

        print(axisValue);

        var distanceFromTop = axisValue * InchesToScaledMeters(ThrowDepth);

        Switch.localPosition = new Vector3(Switch.localPosition.x, _initSwitchLocalY - distanceFromTop, Switch.localPosition.z);
    }

    public float InchesToScaledMeters(float inches)
    {
        return inches / 39.3701f * Switch.lossyScale.y;
    }
}
