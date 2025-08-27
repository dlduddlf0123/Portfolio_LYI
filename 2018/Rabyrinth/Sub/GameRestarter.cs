using UnityEngine;

public class GameRestarter : MonoBehaviour
{
    private void Awake()
    {
        Destroy(MonoSingleton<GameManager>.Inst.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
	}
}
