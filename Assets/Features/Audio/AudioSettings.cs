using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Scriptable Objects/AudioSettings")]
public class AudioSettings : ScriptableObject
{
    public AudioClip clip;
    public float minPitch, maxPitch;
    public float lengthPercent;
}
