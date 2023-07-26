using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private GameBootstrapper _bootstrapperPrefab;

        private void Awake()
        {
#if UNITY_EDITOR
            var bootstrapper = FindObjectOfType<GameBootstrapper>();

            if (bootstrapper == null)
            {
                Instantiate(_bootstrapperPrefab);
            }
#else
            if (SceneManager.GetActiveScene().name.Equals(LevelNames.Initial))
            {
                Instantiate(_bootstrapperPrefab);
            }
#endif
        }
    }
}