using UnityEngine;

public class TargettingIndicator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = RadialInput.Instance.inputPosition;
    }
}
