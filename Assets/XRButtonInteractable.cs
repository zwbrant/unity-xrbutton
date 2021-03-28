using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRButtonInteractable : XRBaseInteractable
{
    public XRButton XRButton;

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            UpdateButtonPress();
        }
    }

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
                    "Interacting controller doesn't have an Activate action configured as an value / axis: " + e.Message);
            }


            XRButton.SetButtonPressed(activateValue);
        }
    }

    private bool IsControllerActionBased(out ActionBasedController controller)
    {
        controller = null;

        if (selectingInteractor is XRBaseControllerInteractor interactor)
        {
            if (interactor.xrController is ActionBasedController actionController)
                controller = actionController;
        }

        return false;
    }
}
