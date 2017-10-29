using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Audio;
using System.Linq;
using System;
using MathNet.Numerics;

public class MaxPowerFrequencies : MonoBehaviour, EEGSoundTransform {

	// Algorithm that selects the highest-energy frequencies in the 
	// [0, MAX_FREQ] range and rescales them into human audible range 

	public int SAMPLE_DURATION = 2;
	public int SAMPLE_RATE = 256;
	public int RECORDING_RATE = 44100;
	public int MAX_FREQ = 54;
	public int NUM_MAX_FREQS = 4;
	public int[] AUDIBLE_RANGE = {200, 2000};

	private double[] outputData;

	private double[] powerSpectrum;
	private double[] shortenedPowerSpectrum;

	private double[] highestPowers;
	private double[] highestFrequencies;
	private double[] frequencyRange;

	public MaxPowerFrequencies() {
		
		outputData = new double[RECORDING_RATE * SAMPLE_DURATION];

		shortenedPowerSpectrum = new double[MAX_FREQ];

		highestPowers = new double[NUM_MAX_FREQS];
		highestFrequencies = new double[NUM_MAX_FREQS];

		frequencyRange = Generate.LogSpaced (MAX_FREQ, Math.Log10 (AUDIBLE_RANGE [0]), Math.Log10 (AUDIBLE_RANGE [1]));
	}

	public float[] EEGToSound (double[] inputData) {

		ComplexSignal cs = Signal.FromArray (inputData, SAMPLE_RATE).ToComplex();
		cs.ForwardFourierTransform ();
		powerSpectrum = Tools.GetPowerSpectrum(cs.GetChannel(0));

		Array.ConstrainedCopy(powerSpectrum, 0, shortenedPowerSpectrum, 0, MAX_FREQ);

		double maxSignal = shortenedPowerSpectrum.Max();
		shortenedPowerSpectrum = shortenedPowerSpectrum.Select (x => x / maxSignal).ToArray ();

		Array.Sort (shortenedPowerSpectrum, frequencyRange);
		Array.ConstrainedCopy (shortenedPowerSpectrum, 0, highestPowers, 0, NUM_MAX_FREQS);
		Array.ConstrainedCopy (frequencyRange, 0, highestFrequencies, 0, NUM_MAX_FREQS);

		for (int i = 0; i < NUM_MAX_FREQS; i ++) {
			double[] sineWave = Generate.Sinusoidal (RECORDING_RATE * SAMPLE_DURATION, RECORDING_RATE, highestFrequencies [i], highestPowers [i]);
			outputData = outputData.Zip(sineWave , (x,y) => x + y).ToArray();
		}

		float[] audioData = outputData.Select<double, float> (x => (float)x).ToArray ();

		return audioData;

	}
		

}
