using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWLPXL.ActionCharacter
{
	/// <summary>
	/// used to clear pools on scene load
	/// </summary>
    public class SceneRunnerPool : MonoBehaviour
    {
		public static SceneRunnerPool Instance => instance;
		static SceneRunnerPool instance;

		protected virtual void Awake()
		{
			if (instance != null && instance != this)
			{
				Destroy(this.gameObject);

			}
			else
			{
				DontDestroyOnLoad(this.gameObject);
				instance = this;
				SceneManager.sceneLoaded += ResetD;
			}

		}

		
		protected virtual void ResetD(Scene scene, LoadSceneMode mode)
		{
			//clear pools
			SimplePool.IniScene();
		}

		protected virtual void OnDestroy()
		{
			SceneManager.sceneLoaded -= ActorHitBoxManager.ResetD;

		}
	}
}
