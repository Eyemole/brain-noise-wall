using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Audio;
using System.Linq;
using System;
using MathNet.Numerics;

public class EEGMusicPlayer : MonoBehaviour {

	public int channelToRecordFrom = 0; // Which channel of the EEG headset the data is recorded from (ranges from 0-3 for the Muse headset)

	public int SAMPLE_DURATION = 2; // The duration of each data collection cycle 
	public int SAMPLE_RATE = 256; // The sample rate of the incoming data
	public int RECORDING_RATE = 44100; // The sample rate of the audio output 
	private int BUFFER_SIZE; 

	private int cyclePosition; //Variable to track the position in the data collection cycle
	private double[] dataBuffer; //Variable to store incoming data

	public AudioSource audioSource; //The AudioSource that will play the transformed sound

	public EEGSoundTransform soundTransform; // The algorithm used to transform EEG data to sound data 

	// Use this for initialization
	void Start () {

		cyclePosition = 0;

		BUFFER_SIZE = SAMPLE_DURATION * SAMPLE_RATE;
		dataBuffer = new double[BUFFER_SIZE];

		//The EEGSoundTransform and AudioSource components must be attached to the same object that this script is attached to
		soundTransform = GetComponent<EEGSoundTransform> ();
		audioSource = GetComponent<AudioSource> ();
	
		//An empty clip needs to be generated for the AudioSource to work 
		AudioClip emptyClip = AudioClip.Create ("emptyClip", RECORDING_RATE * SAMPLE_DURATION, 1, RECORDING_RATE, false);
		audioSource.clip = emptyClip;
	
	}
	
	// Update is called once per frame
	void Update () {

		dataBuffer [cyclePosition] = (double) EEGData.eegData [channelToRecordFrom];
		cyclePosition++;

		// If enough data has been collected, turn it to sound 
		if (cyclePosition == BUFFER_SIZE - 1) {
			cyclePosition = 0;
			PlayMusic ();
		}
			
	}

	// Transforms the buffered data and play it 
	void PlayMusic() {

		float[] audioData = soundTransform.EEGToSound (dataBuffer);
		audioSource.clip.SetData (audioData, 0);
		audioSource.Play ();
		audioSource.loop = true;

	}

}
