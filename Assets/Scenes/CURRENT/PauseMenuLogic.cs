using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PauseMenuLogic : MonoBehaviour
{
    public Transform playArea;
    public Transform headOrientation;
    public Transform pauseLocation;
    public Transform gameLocation;

    protected bool inPauseMenu = false;

    public List<GameObject> pauseItems;
    public List<GameObject> gameItems;

    public GameObject teleportationRelease;
    public GameObject teleportationPress;
    
    private float currentFadeTime = 0.0f;
    private Player player = null;

    private void Start()
    {
        player = Player.instance;
        if ( player == null )
        {
            Debug.LogError("<b>[SteamVR Interaction]</b> Teleport: No Player instance found in map.");
            Destroy( this.gameObject );
            return;
        }
    }

    public void TeleportOnPause()
    {
        Debug.Log("MADEIT!");
        SteamVR_Fade.Start( Color.clear, currentFadeTime );
        //TeleportPoint teleportPoint = teleportingToMarker as TeleportPoint;
        
        //THIS DOES NOT WORK-but we dont need to teleport on pause so its ok
        //TODO: need to investigate player transform and tracking origin transform for hyperplane tings
        player.trackingOriginTransform = pauseLocation;
    }
    
//    
//    public void SwitchRooms() {
//        TransformData teleportDestination = new TransformData(gameLocation);
//        if (!inPauseMenu) {
//            gameLocation.position = new Vector3(headOrientation.position.x, playArea.position.y, headOrientation.position.z);
//
//            Vector3 right = Vector3.Cross(playArea.up, headOrientation.forward);
//            Vector3 forward = Vector3.Cross(right, playArea.up);
//
//            gameLocation.rotation = Quaternion.LookRotation(forward, playArea.up);
//
//            teleportDestination = new TransformData(pauseLocation);
//        }
//
//        teleporter.Teleport(teleportDestination);
//        inPauseMenu = !inPauseMenu;
//
//        foreach (GameObject item in pauseItems) {
//            item.SetActive(inPauseMenu);
//        }
//
//        foreach (GameObject item in gameItems) {
//            item.SetActive(!inPauseMenu);
//        }
//    }
//    public void ResetGame() {
//        SceneManager.LoadScene("Final", LoadSceneMode.Single);
//    }
//
//    public void SwitchTeleportationToPress(bool value) {
//        teleportationRelease.SetActive(!value);
//        teleportationPress.SetActive(value);
//    }
//    public void SwitchTeleportationToRelease(bool value) {
//        SwitchTeleportationToPress(!value);
//    }
}
