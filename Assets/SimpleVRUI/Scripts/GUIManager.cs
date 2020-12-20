using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SimpleVRUI.Gracken
{
	public class GUIManager : MonoBehaviour
	{

        /// <summary>
        /// Reference to the GUI Screens
        /// </summary>
		public GameObject MainMenuVR;
		public GameObject PausedMenuVR;
		public GameObject OptionsMenu;
		public GameObject HUDGameplayVR;
		public GameObject WaveManager;

        /// <summary>
        /// Reference to the Canvas
        /// </summary>
		public Canvas MainVRCanvas;

        /// <summary>
        /// True or False statements for specific states in the game
        /// </summary>
		public bool GameHasStarted;
		public bool GameWasPaused;
		public bool GameWasResumed;
		public bool GameBackToMenu = false;

		// Use this for initialization
		void Start ()
		{

		}

		// Update is called once per frame
		void Update ()
		{
            // This makes sure at all times while the game is active timescale is always 1
			if (GameHasStarted || GameWasResumed) {
				Time.timeScale = 1;
			}
            // This sets the time scale to 0 while the game is paused. Freezing all things in the scene
			if (GameWasPaused) {
				Time.timeScale = 0;
			}
            // Only Called if using the "InitiateWaves" function 
			if (GameHasStarted && HUDGameplayVR.activeSelf == true) {

				WaveManager.SetActive (true);
				GameBackToMenu = false;
			}
				
		}
        /// <summary>
        /// This funtion handles "starting" the game by disabling the Main Menu and brings up the GameHUD
        /// </summary>
		public void StartTheGame ()
		{
			
			HUDGameplayVR.SetActive (true);
			MainMenuVR.SetActive (false);

		}
        /// <summary>
        /// This function handles switching from the Main Menu to the Options Menu
        /// </summary>
		public void SelectOptionsMenu ()
		{
			MainMenuVR.SetActive (false);

			if (MainMenuVR.activeSelf == false) {
				OptionsMenu.SetActive (true);
			}
		}
        /// <summary>
        /// Alternative to starting the game. This is what recognizes the game has started and would be good for spawning waves 
        /// </summary>
		public void InitiateWaves ()
		{
			MainMenuVR.SetActive (false);

			if (MainMenuVR.activeSelf == false) {
				HUDGameplayVR.SetActive (true);
				GameHasStarted = true;
			}
		}
        /// <summary>
        /// This function handles reloading the scene and going back to the Main Menu
        /// </summary>
		public void OnClickBackToMainMenu ()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
		}
        /// <summary>
        /// This function handles pausing the game
        /// </summary>
		public void PauseTheGame ()
		{
			HUDGameplayVR.SetActive (false);
			PausedMenuVR.SetActive (true);
			GameWasResumed = false;
			GameWasPaused = true;

		}
        /// <summary>
        /// This function handles recognizing the state of when the game was resumed, therefore ensuring timescale will be equal to 1
        /// </summary>
		public void GameResuming ()
		{
			HUDGameplayVR.SetActive (true);
			PausedMenuVR.SetActive (false);
			GameWasPaused = false;
			GameWasResumed = true;

		}
	}

}
