using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EEGSoundTransform {

	// Interface for the algorithms that will transform raw EEG data to sound data

	float[] EEGToSound (double[] inputData);
}
