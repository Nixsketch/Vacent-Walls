// Game Quality Settings
// AliyerEdon in Winter 2022
//				
//

// This script used to load game settings in the gameplay scene
using UnityEngine;
using System.Collections;

public class Load_Settings : MonoBehaviour
{
	#region Variables
	[Space(7)]
	[Header("and quality only on the first scene of the game")]
	[Header("the first scene .You need to apply resolution")]
	[Header("Use this to apply resolution/quality in ")]
	public bool isFirstScene = true;

	[Space(7)]
	[Header("FPS Display")]
	public Color fpsColor = Color.yellow;
	public TextAnchor alignment = TextAnchor.UpperRight;
	public FontStyle fontStyle = FontStyle.Bold;

	[Space(7)]
	[Header("Effects Volume")]
	public UnityEngine.Rendering.PostProcessing.PostProcessVolume globalVolume;

	#endregion

	void Start ()
	{
		#region Default Settings

		// Set the game default setting when it is the first run on the device
		if (PlayerPrefs.GetInt("The First Run") != 1) // 1 = true; others = 0
		{
			// Se the default settings for effects
			PlayerPrefs.SetInt("Display FPS", 0);
			PlayerPrefs.SetInt("Bloom Effect", 0);
			PlayerPrefs.SetInt("Post Effects", 0);
			PlayerPrefs.SetInt("Fog Effect", 0);
			PlayerPrefs.SetInt("Dynamic Resolution", 0);
			PlayerPrefs.SetInt("vSync", 0);
			PlayerPrefs.SetInt("TAA", 0);
			PlayerPrefs.SetInt("HDR", 0);

			// Set the default quality level
			PlayerPrefs.SetInt("Quality Level", 0);

			// Store the device original resolution
			PlayerPrefs.SetInt("OriginalX", Screen.width);

			PlayerPrefs.SetInt("OriginalY", Screen.height);

			// The is not the first run anymore
			PlayerPrefs.SetInt("The First Run", 1);
		}
		
		#endregion

		#region Apply Settings

		Camera[] cams = GameObject.FindObjectsOfType<Camera>();
		//_________________________________________________
		if (PlayerPrefs.GetInt("Display FPS") == 1)
		{
			if (!gameObject.GetComponent<Display_FPS>())
				gameObject.AddComponent<Display_FPS>();

			gameObject.GetComponent<Display_FPS>().color = fpsColor;
			gameObject.GetComponent<Display_FPS>().fontStyle = fontStyle;
			gameObject.GetComponent<Display_FPS>().alignment = alignment;
		}
		else
		{
			Display_FPS[] dFPS = GameObject.FindObjectsOfType<Display_FPS>();

			for (int a = 0; a < dFPS.Length; a++)
				Destroy(dFPS[a]);
		}

		//_________________________________________________
		UnityEngine.Rendering.PostProcessing.Bloom vBloom;

		if(globalVolume)
			globalVolume.sharedProfile.TryGetSettings(out vBloom);
		else
			GameObject.FindObjectOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().sharedProfile.TryGetSettings(out vBloom);
		
		if (vBloom != null)
		{
			if (PlayerPrefs.GetInt("Bloom Effect") == 1)
				vBloom.active = true;
			else
				vBloom.active = false;
		}
		//_________________________________________________
		if (PlayerPrefs.GetInt("Post Effects") == 1)
		{
			for (int a = 0; a < cams.Length; a++)
			{
				if (cams[a].GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>())
					cams[a].GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().enabled = true;
			}
		}
		else
		{
			for (int a = 0; a < cams.Length; a++)
			{
				if (cams[a].GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>())
					cams[a].GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().enabled = false;
			}
		}       
		//_________________________________________________
		if (PlayerPrefs.GetInt("Fog Effect") == 1)
			RenderSettings.fog = true;
		else
			RenderSettings.fog = false;
		//_________________________________________________
		if (PlayerPrefs.GetInt("Dynamic Resolution") == 1)
        {
			for(int a = 0;a< cams.Length;a++)
				cams[a].allowDynamicResolution = true;
		}
		else
		{
			for (int a = 0; a < cams.Length; a++)
				cams[a].allowDynamicResolution = false;
		}
		//_________________________________________________
		if (isFirstScene)
		{
			if (PlayerPrefs.GetInt("Resolution Quality") == 2)
			{
				Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 0.5f),
					(int)(PlayerPrefs.GetInt("OriginalY") * 0.5f), true);
			}
			if (PlayerPrefs.GetInt("Resolution Quality") == 1)
			{
				Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 0.7f),
					(int)(PlayerPrefs.GetInt("OriginalY") * 0.7f), true);
			}
			if (PlayerPrefs.GetInt("Resolution Quality") == 0)
			{
				Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 1),
					(int)(PlayerPrefs.GetInt("OriginalY") * 1), true);
			}
			//_________________________________________________
			
			if(PlayerPrefs.GetInt("Anti Aliasing") == 0)
				QualitySettings.antiAliasing = 0;
			if (PlayerPrefs.GetInt("Anti Aliasing") == 1)
				QualitySettings.antiAliasing = 2;
			if (PlayerPrefs.GetInt("Anti Aliasing") == 2)
				QualitySettings.antiAliasing = 4;
			if (PlayerPrefs.GetInt("Anti Aliasing") == 3)
				QualitySettings.antiAliasing = 8;

			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality Level"), false);

		}
		//_________________________________________________
		if (PlayerPrefs.GetInt("TAA") == 1)
		{
			for (int a = 0; a < cams.Length; a++)
			{
				cams[a].GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
					.antialiasingMode =
					UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.TemporalAntialiasing;
			}
		}
		else
		{
			for (int a = 0; a < cams.Length; a++)
			{
				cams[a].GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>()
					.antialiasingMode =
					UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.None;
			}
		}
		//_________________________________________________
		if (PlayerPrefs.GetInt("vSync") == 0)
			QualitySettings.vSyncCount = 0;
		else
			QualitySettings.vSyncCount = 1;
		//_________________________________________________
		for (int a = 0; a < cams.Length; a++)
		{
			if (PlayerPrefs.GetInt("HDR") == 1)
				cams[a].allowHDR = true;
			if (PlayerPrefs.GetInt("HDR") == 0)
				cams[a].allowHDR = false;
		}
		//_________________________________________________
		#endregion

	}

	// Call this from settings menu script tp update displat fps settings durring menu
	public void Update_DisplayFPS()
    {
		//_________________________________________________
		if (PlayerPrefs.GetInt("Display FPS") == 1)
		{
			if (!gameObject.GetComponent<Display_FPS>())
				gameObject.AddComponent<Display_FPS>();

			gameObject.GetComponent<Display_FPS>().color = fpsColor;
			gameObject.GetComponent<Display_FPS>().fontStyle = fontStyle;
			gameObject.GetComponent<Display_FPS>().alignment = alignment;
		}
		else
		{
			Display_FPS[] dFPS = GameObject.FindObjectsOfType<Display_FPS>();

			for (int a = 0; a < dFPS.Length; a++)
				Destroy(dFPS[a]);
		}
	}

	// Use the below function to enable / disable / toggle menu objects
	public void Enable_Object(GameObject target)
	{
		target.SetActive(true);
	}

	public void Disable_Object(GameObject target)
	{
		target.SetActive(false);
	}

	public void Toggle_Object(GameObject target)
	{
		target.SetActive(!target.activeSelf);
	}
}
