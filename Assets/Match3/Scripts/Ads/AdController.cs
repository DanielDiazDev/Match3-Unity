using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Match3.Scripts.Ads
{
    public class AdController : MonoBehaviour, IUnityAdsInitializationListener,
        IUnityAdsLoadListener, IUnityAdsShowListener
    {
        public static AdController Instance { get; private set; }
        [SerializeField] private string _androidGameId;
        [SerializeField] private string _androidAdsId;
        [SerializeField] private bool _test;
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Init()
        {
            if (!Advertisement.isInitialized)
            {
                Advertisement.Initialize(_androidGameId, _test, this);
            }
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Completado");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log("Fallo");
        }

        public void OnUnityAdsAdLoaded(string placementId) //Anuncio se carga
        {
            Advertisement.Show(placementId, this);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
        }

        public void OnUnityAdsShowStart(string placementId)
        {
        }

        public void OnUnityAdsShowClick(string placementId)
        {
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
        }

        public void ShowAds()
        {
            Advertisement.Load(_androidAdsId, this);
        }

        
    }
}