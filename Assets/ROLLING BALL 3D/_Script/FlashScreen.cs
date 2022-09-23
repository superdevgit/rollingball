using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlashScreen : MonoBehaviour {
    public float delayLoadPlayingScene = 1;     //delay some time before go to the Playing scene
    public string loadSceneName = "Playing";
    public Image progressImage;   //show percent loaded data
    public static FlashScreen Instance;

    private bool m_loading = false;
    private string _account;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    /*
    void Start()
    {
        if (MetamaskController.Instance.m_isMetamaskConnected && MetamaskController.Instance.m_account != null)
        {
            _account = MetamaskController.Instance.m_account;
            string account = _account.Substring(0, 6) + "..." + _account.Substring(39, 3);
            GameObject.Find("WalletButton").GetComponentInChildren<TextMeshProUGUI>().text = account;
        }
    }
    */

    public void StartGame() 
    {
        /*
        #if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(JoinGame());
        #else
            StartCoroutine(LoadAsynchronously(loadSceneName));
        #endif
        */
        StartCoroutine(LoadAsynchronously(loadSceneName));
    }
    
    IEnumerator LoadAsynchronously(string name)
    {

        yield return new WaitForSeconds(delayLoadPlayingScene);

        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressImage.fillAmount = progress;
            yield return null;
        }
    }

    public IEnumerator JoinGame()
    {
        if (MetamaskController.Instance.m_isMetamaskConnected)
        {
            if (MetamaskController.Instance.m_balanceOfERC20 > MetamaskController.Instance.m_enterPrice)
            {
                m_loading = true;
                yield return MetamaskController.Instance.JoinOnlineGame(1);
                m_loading = false;
            }
            else
            {
                MetamaskInterop.AlertInsufficientERC20();
                m_loading = false;
                Debug.Log("You don't have enough ROK token, please purchase it");
            }
            StartCoroutine(LoadAsynchronously(loadSceneName));
        }
        else
        {
            OnConnectWallet();
        }
    }

    public void OnConnectWallet()
    {
        MetamaskController.Instance.ConnectWallet();
    }
}
