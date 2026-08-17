using System.Collections.Generic;
using UnityEngine;
using View.UI.Builder;

namespace View.UI
{
    /// <summary>
    /// Пул объектов для всплывающих текстов (FloatingScore).
    /// </summary>
    public class FloatingScorePool : MonoBehaviour
    {
        private int initialPoolSize = 10;

        private Transform _canvasTransform;
        private Transform _parentTransform;
        private readonly Queue<FloatingScore> _pool = new();

        private void Awake()
        {
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
            foreach (var t in allTransforms)
            {
                if (t.name == UiIds.FloatingScoreHost && t.parent != null && t.parent.name == UiIds.Content)
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
            var floatingScore = WidgetFactory.CreateFloatingScore(_parentTransform);
            floatingScore.transform.SetParent(_parentTransform, false);
            return floatingScore;
        }

        /// <summary>
        /// Получает экземпляр FloatingScore из пула.
        /// </summary>
        public FloatingScore GetScore()
        {
            var scoreInstance = _pool.Count > 0 ? _pool.Dequeue() : CreateNewInstance();
            scoreInstance.transform.SetParent(_parentTransform, false);
            _parentTransform.SetAsLastSibling();
            scoreInstance.transform.SetAsLastSibling();
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