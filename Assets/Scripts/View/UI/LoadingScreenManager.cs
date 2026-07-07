using UnityEngine;

namespace View.UI
{
    /// <summary>
    /// Управляет экраном загрузки.
    /// </summary>
    public class LoadingScreenManager : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
