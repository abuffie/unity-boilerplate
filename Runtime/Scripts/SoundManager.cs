using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Script:  SoundManager.cs
/// Created: 5/11/2023 8:52:41 AM
/// Author:  abuffie
/// </summary>

public class SoundManager : MonoSingleton<SoundManager>{

	[SerializeField] AudioMixer mixer;
	
	// Do not remove you will break your singleton
	// This singleton DOES NOT destroy
	private void Awake(){
		base.Awake(true);
		// Put your awake code here if any
	}
}
