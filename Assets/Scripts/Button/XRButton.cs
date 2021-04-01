using UnityEngine;

public abstract class XRButton : MonoBehaviour
{
    [SerializeField]
    public XRButtonEvent OnDown;
    [SerializeField]
    public XRButtonEvent OnUp;

    public AudioSource DownSound;
    public AudioSource UpSound;

    public abstract bool IsDown
    {
        get;
    }

    protected virtual void ButtonDown()
    {
        if (OnDown != null)
            OnDown.Invoke();
        if (DownSound != null)
            DownSound.Play();
    }

    protected virtual void ButtonUp()
    {
        if (OnUp != null)
            OnUp.Invoke();
        if (UpSound != null)
            UpSound.Play();
    }
}
