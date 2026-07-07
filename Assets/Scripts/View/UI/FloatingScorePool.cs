using System.Collections.Generic;
using UnityEngine;

namespace View.UI
{
    /// <summary>
    /// Пул объектов для всплывающих текстов (FloatingScore).
    /// </summary>
    public class FloatingScorePool : MonoBehaviour
    {
        private GameObject _floatingScorePrefab;
        [SerializeField] private int initialPoolSize = 10;

        private Transform _canvasTransform;
        private Transform _parentTransform;
        private readonly Queue<FloatingScore> _pool = new();

        private void Awake()
        {
            _floatingScorePrefab = Resources.Load<GameObject>("Prefabs/Prefab_FloatingScore");
            
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in allTransforms)
            {
                if (t.name == "Score" && t.parent != null && t.parent.name == "Content")
                {
                    _canvasTransform = t;
                    _parentTransform = t;
                    break;
                }
            }
            
            if (_canvasTransform == null)
            {
                Debug.LogError("[FloatingScorePool] Не удалось найти 'Score' внутри 'Content'.");
                _canvasTransform = transform;
                _parentTransform = transform;
            }
        }

        private void Start()
        {
            for (var i = 0; i < initialPoolSize; i++)
            {
                var scoreInstance = CreateNewInstance();
                ReturnScore(scoreInstance);
            }
        }

        private FloatingScore CreateNewInstance()
        {
            var go = Instantiate(_floatingScorePrefab, _canvasTransform, false);
            go.transform.SetParent(_parentTransform, false);
            var floatingScore = go.GetComponent<FloatingScore>();
            return floatingScore;
        }

        /// <summary>
        /// Получает экземпляр FloatingScore из пула.
        /// </summary>
        public FloatingScore GetScore()
        {
            if (_pool.Count <= 0) return CreateNewInstance();
            var scoreInstance = _pool.Dequeue();
            scoreInstance.gameObject.SetActive(true);
            return scoreInstance;
        }

        /// <summary>
        /// Возвращает экземпляр FloatingScore в пул.
        /// </summary>
        public void ReturnScore(FloatingScore scoreInstance)
        {
            scoreInstance.gameObject.SetActive(false);
            _pool.Enqueue(scoreInstance);
        }
    }
}