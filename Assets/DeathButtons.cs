using UnityEngine;

public class DeathButtons : MonoBehaviour
{
    public void RestartButton()
    {
        GameManager.Instance.StartGame();
    }
}
