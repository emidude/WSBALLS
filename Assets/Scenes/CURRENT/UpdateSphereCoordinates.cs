//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Demonstrates how to create a simple interactable object
//
//=============================================================================

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem.Sample
{
    //-------------------------------------------------------------------------
    [RequireComponent(typeof(Interactable))]
    public class UpdateSphereCoordinates : MonoBehaviour
    {
       // private TextMesh generalText;
       // private TextMesh hoveringText;
        private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);

        private Interactable interactable;

        //GameObject referenceObject;
        //public SLIDERS_Slicing slicingScript;

        //GameObject referenceObject;
        //public Info sphereInfo;

        public Transform ball;

        public bool attached;

        //-------------------------------------------------
        void Awake()
        {
            attached = false;
            //var textMeshs = GetComponentsInChildren<TextMesh>();
            //generalText = textMeshs[0];
            //hoveringText = textMeshs[1];

            //generalText.transform.parent = pmTransform;
            //hoveringText.transform.parent = pmTransform;

            //generalText.text = "No Hand Hovering";
            //hoveringText.text = "Hovering: False";

            interactable = this.GetComponent<Interactable>();

            //referenceObject = GameObject.FindWithTag("SLIDEY-SLICE");
            //slicingScript = referenceObject.GetComponent<SLIDERS_Slicing>();

          //  referenceObject = gameObject.GetComponentInParent<FourD>();
            //sphereInfo = this.GetComponentInParent<Info>();

        }


        //-------------------------------------------------
        // Called when a Hand starts hovering over this object
        //-------------------------------------------------
        private void OnHandHoverBegin(Hand hand)
        {
            //generalText.text = "Hovering hand: " + hand.name;
        }


        //-------------------------------------------------
        // Called when a Hand stops hovering over this object
        //-------------------------------------------------
        private void OnHandHoverEnd(Hand hand)
        {
            //	generalText.text = "No Hand Hovering";
        }


        //-------------------------------------------------
        // Called every Update() while a Hand is hovering over this object
        //-------------------------------------------------
        private void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

            if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
            {
                // Call this to continue receiving HandHoverUpdate messages,
                // and prevent the hand from hovering over anything else
                hand.HoverLock(interactable);

                // Attach this object to the hand
                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
            }
            else if (isGrabEnding)
            {
                // Detach this object from the hand
                hand.DetachObject(gameObject, false);

                // Call this to undo HoverLock
                hand.HoverUnlock(interactable);

                // Restore position/rotation
                //transform.position =  ball.position;
                //transform.rotation = ball.rotation;


            }
        }


        //-------------------------------------------------
        // Called when this GameObject becomes attached to the hand
        //-------------------------------------------------
        public void OnAttachedToHand(Hand hand)
        {
            attached = true;
        }



        //-------------------------------------------------
        // Called when this GameObject is detached from the hand
        //-------------------------------------------------
        public void OnDetachedFromHand(Hand hand)
        {
            attached = false;
        }


        //-------------------------------------------------
        // Called every Update() while this GameObject is attached to the hand
        //-------------------------------------------------
        private void HandAttachedUpdate(Hand hand)
        {
            ball = hand.transform;

            //Info infoscript = GetComponentInParent<Info>(); //dont need updated in info!:)

            //Debug.Log("on hand attached update:");
            //Debug.Log("this objects position = " + transform.position);
            //Debug.Log("ball position = " + ball.position);
            //Debug.Log("hand position = " + hand.transform.position);

            //generalText.text = string.Format("Attached: {0} :: Time: {1:F2}", hand.name, (Time.time - attachTime));
            //generalText.text = string.Format("Position: {0}", pmTransform.position);

        }

        private bool lastHovering = false;
        private void Update()
        {
            if (interactable.isHovering != lastHovering) //save on the .tostrings a bit
            {
                //  hoveringText.text = string.Format("Hovering: {0}", interactable.isHovering);
                lastHovering = interactable.isHovering;
            }

           
           // Debug.Log("hand position = " + hand.transform.position);
        }


        //-------------------------------------------------
        // Called when this attached GameObject becomes the primary attached object
        //-------------------------------------------------
        private void OnHandFocusAcquired(Hand hand)
        {
        }


        //-------------------------------------------------
        // Called when another attached GameObject becomes the primary attached object
        //-------------------------------------------------
        private void OnHandFocusLost(Hand hand)
        {
        }
    }
}
