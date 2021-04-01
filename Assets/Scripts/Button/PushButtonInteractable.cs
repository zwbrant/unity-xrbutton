using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PushButtonInteractable : XRBaseInteractable
{
    public PushButton PushButton;

    private XRBaseInteractor _currHoverInteractor;

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    { 
        base.ProcessInteractable(updatePhase);

        if (_currHoverInteractor != null && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            UpdateButtonPress();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        _currHoverInteractor = args.interactor;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        _currHoverInteractor = null;
    }


    // if we have an action-based controller, try to read Activate action to interact with XRButton 
    private void UpdateButtonPress()
    {

        if (IsControllerActionBased(out ActionBasedController controller))
        {

            float activateValue = -1;
            try
            {
                activateValue = controller.activateAction.action.ReadValue<float>();
            } catch (Exception e)
            {
                throw new Exception(
                    string.Format("Failed to read value of Activate action on {0}: {1}", controller.name, e.Message));
            }

            PushButton.SetButtonPressed(activateValue);
        }
    }

    private bool IsControllerActionBased(out ActionBasedController controller)
    {
        controller = null;

        if (_currHoverInteractor is XRBaseControllerInteractor interactor)
        {
            if (interactor.xrController is ActionBasedController actionController)
                controller = actionController;
        }

        return controller != null;
    }

}
