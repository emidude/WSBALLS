//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Valve.VR.InteractionSystem.Sample
{
    public class ThumbMoving : MonoBehaviour
    {
        public SteamVR_Action_Boolean plantAction;

        public SteamVR_Action_Vector2 thumbXY;

        public Hand hand;

        public GameObject prefabToPlant;



        private void OnEnable()
        {
            if (hand == null)
                hand = this.GetComponent<Hand>();

            if (plantAction == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> No plant action assigned");
                return;
            }

            //calling the AddOnChangeListener method and passing in a method we want called when the action changes
          //  thumbXY.AddOnChangeListener(OnThumbPositionChange, hand.handType);
           
          //  SteamVR_Input.{nameOfYourBinding}.inActions.{yourActionName}.GetAxis(SteamVR_Input_Sources.Any);
           // SteamVR_Input.MoveWithThumb.inActions.GetAxis(SteamVR_Input_Sources.Any);
        }

        private void OnDisable()
        {
            if (plantAction != null){}
             //   thumbXY.RemoveOnChangeListener(OnThumbPositionChange, hand.handType);
        }

        private void OnThumbPositionChange(SteamVR_Action_Vector2 actionIn, SteamVR_Input_Sources inputSource, bool newValue)
        {
            if (newValue)
            {
                Plant();
            }
        }

        public void Plant()
        {
            StartCoroutine(DoPlant());
        }

        private IEnumerator DoPlant()
        {
            Vector3 plantPosition;

            RaycastHit hitInfo;
            bool hit = Physics.Raycast(hand.transform.position, Vector3.down, out hitInfo);
            if (hit)
            {
                plantPosition = hitInfo.point + (Vector3.up * 0.05f);
            }
            else
            {
                plantPosition = hand.transform.position;
                plantPosition.y = Player.instance.transform.position.y;
            }

            GameObject planting = GameObject.Instantiate<GameObject>(prefabToPlant);
            planting.transform.position = plantPosition;
            planting.transform.rotation = Quaternion.Euler(0, Random.value * 360f, 0);

            planting.GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));

            Rigidbody rigidbody = planting.GetComponent<Rigidbody>();
            if (rigidbody != null)
                rigidbody.isKinematic = true;



            Vector3 initialScale = Vector3.one * 0.01f;
            Vector3 targetScale = Vector3.one * (1 + (Random.value * 0.25f));

            float startTime = Time.time;
            float overTime = 0.5f;
            float endTime = startTime + overTime;

            while (Time.time < endTime)
            {
                planting.transform.localScale = Vector3.Slerp(initialScale, targetScale, (Time.time - startTime) / overTime);
                yield return null;
            }


            if (rigidbody != null)
                rigidbody.isKinematic = false;
        }
    }
}