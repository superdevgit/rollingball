using System;
using UnityEngine;
using System.Numerics;
using System.Collections;
using Nethereum.JsonRpc.UnityClient;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.Metamask;
using UnityEngine.UI;
using TMPro;

public class MetamaskController : MonoBehaviour
{
    //private string m_chain = "ethereum";
    //private string m_network = "rinkeby";
    private string m_rpcUrl = "https://bsc-dataseed.binance.org";//"https://rinkeby.infura.io/v3/9aa3d95b3bc440fa88ea12eaa4456161"
    private BigInteger m_chainId = 56; //4
    private string m_tokenContract = "0x2bf82a28655a4f54c8d2dac0c5282e0236fca0b1"; 
    private string m_gameContract = "0xb32b7039eb6d2C0FA118df95A27F79f78E6e5b66";  
    //private string m_gameContractAbi = "[{ \"inputs\": [{ \"internalType\": \"address\", \"name\": \"token\", \"type\": \"address\" }], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [{ \"indexed\": true, \"internalType\": \"address\", \"name\": \"previousOwner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" }], \"name\": \"OwnershipTransferred\", \"type\": \"event\" }, { \"inputs\": [{ \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" }], \"name\": \"enterPrice\", \"outputs\": [{ \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" }], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"finalRound\", \"outputs\": [{ \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" }], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"finalWinnerCount\", \"outputs\": [{ \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" }], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"uint8\", \"name\": \"mode\", \"type\": \"uint8\" }], \"name\": \"joinGame\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"owner\", \"outputs\": [{ \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" }], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"renounceOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" }], \"name\": \"reward\", \"outputs\": [{ \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" }], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"address\", \"name\": \"user\", \"type\": \"address\" }, { \"internalType\": \"uint8\", \"name\": \"round\", \"type\": \"uint8\" }, { \"internalType\": \"uint8\", \"name\": \"bonusTimes\", \"type\": \"uint8\" }], \"name\": \"rewardPerRound\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"uint256\", \"name\": \"price\", \"type\": \"uint256\" }], \"name\": \"setEnterPrice\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"uint256\", \"name\": \"round\", \"type\": \"uint256\" }], \"name\": \"setFinalRound\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" }], \"name\": \"transferOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }]";
    private string m_adminPrivateKey = "0x6bf0d487338c50f63b47643f50900ff08bdbfa9793989dcdffd2c6b18a10edc2";
    private string m_adminPublicKey = "0x1eC02023106a65970e220768e33C8aAfa5Ddf42f";
    private BigInteger m_approveAmount = 9999999999000000000;
    private BigInteger m_minApproveAmount = 5000000000000;

    public string m_account { get; private set; }
    public bool m_isMetamaskConnected = false;
    public BigInteger m_balanceOfERC20 = 0;
    public BigInteger m_allowance = 0;
    public BigInteger m_enterPrice = 0;
    public bool m_isSentReward = false;
    public bool m_isJoinedGame = false;
    public bool m_isApproved = false;
    public bool b_isMultiGame = false;

    public static MetamaskController Instance { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public MetamaskController()
    {
        m_account = "";
    }

    public void ConnectWallet()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (MetamaskInterop.IsMetamaskAvailable())
        {
            MetamaskInterop.EnableEthereum(gameObject.name, nameof(EthereumEnabled), nameof(DisplayError));
        }
        else
        {
            MetamaskInterop.AlertInstallMetamask();
            DisplayError("Metamask is not available, please install it");
        }
#else
        Debug.Log("Editor Test");
        EthereumEnabled("0x1eC02023106a65970e220768e33C8aAfa5Ddf42f");
#endif
        Debug.Log("EnableEthereum Checked");
    }

    public void EthereumEnabled(string _account)
    {
        Debug.Log("called EthereumEnabled---");
        Debug.Log("m_isMetamaskConnected is---" + m_isMetamaskConnected);
        Debug.Log("_account is---" + _account);
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!m_isMetamaskConnected)
        {
            MetamaskInterop.EthereumInit(gameObject.name, nameof(NewAccountSelected), nameof(ChainChanged));
            MetamaskInterop.GetChainId(gameObject.name, nameof(ChainChanged), nameof(DisplayError));
        }
        NewAccountSelected(_account);
#else
        NewAccountSelected(_account);
#endif
    }

    public void NewAccountSelected(string _account)
    {
        Debug.Log("called NewAccountSelected---");
        m_account = _account;
        m_isMetamaskConnected = true;
        string account = _account.Substring(0, 6) + "..." + _account.Substring(39, 3);
        GameObject.Find("WalletButton").GetComponentInChildren<TextMeshProUGUI>().text = account;
        FlashScreen.Instance.StartGame();
#if UNITY_EDITOR
        //FlashScreen.Instance.StartGame();
        return;
#endif
        CheckChainId();
        StartCoroutine(GetAllowance());
        StartCoroutine(GetEnterPrice(1));
        StartCoroutine(GetERC20Balance());
    }

    public void ChainChanged(string _chainId)
    {
        Debug.Log("called Chainchanged, chainId---" + _chainId);
        /*
#if UNITY_WEBGL && !UNITY_EDITOR
        if (BigInteger.Parse(_chainId) != m_chainId)
        {
            MetamaskInterop.ChangeChain(gameObject.name, _chainId, nameof(ChainChanged), nameof(DisplayError));
        } else
        {
            m_chainId = new HexBigInteger(_chainId).Value;
        }
#endif
*/
    }

    public void CheckChainId()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        MetamaskInterop.GetChainId(gameObject.name, nameof(GetChainIdResponse), nameof(DisplayError));
#endif
    }

    public void GetChainIdResponse(string chainId)
    {
        if (BigInteger.Parse(chainId) != m_chainId)
        {
            MetamaskInterop.AlertChangeChain();
        }
    }

    public IEnumerator GetAllowance()
    {
        Debug.Log("called GetAllowance---");
        var tokenReq = new TokenContractRequests(m_rpcUrl, m_tokenContract, m_account, m_gameContract);
        do
        {
            yield return tokenReq.GetAllowance();
            yield return new WaitForSeconds(1.0f);
        } while (tokenReq.Exception != null);

        m_allowance = tokenReq.Result;
        if (m_allowance >= m_minApproveAmount) m_isApproved = true;
        Debug.Log("m_allowance is---" + m_allowance);
        Debug.Log("m_isApproved is---" + m_isApproved);
    }

    public IEnumerator GetERC20Balance()
    {
        Debug.Log("called GetERC20Balance---");
        Debug.Log("m_account is---" + m_account);
        var tokenReq = new TokenContractRequests(m_rpcUrl, m_tokenContract, m_account); 
        do
        {
            yield return tokenReq.GetBalanceOf();
            yield return new WaitForSeconds(1.0f);
        } while (tokenReq.Exception != null);

        m_balanceOfERC20 = tokenReq.Result;
        Debug.Log("m_balanceOfERC20 is---" + m_balanceOfERC20);
    }

    public IEnumerator GetEnterPrice(int _mode)
    {
        Debug.Log("called GetEnterPrice---");
        Debug.Log("m_account is---"+ m_account);
        var gameReq = new GameContractRequests(m_rpcUrl, m_gameContract, _mode, m_account);

        do
        {
            yield return gameReq.GetEnterPrice();
            yield return new WaitForSeconds(1.0f);


        } while (gameReq.Exception != null);

        m_enterPrice = gameReq.Result * 1000000000;
        Debug.Log("m_enterPrice is---" + m_enterPrice);
    }

    public IEnumerator Approve()
    {
        Debug.Log("called Approve---");
        var approveReq = new MetamaskTransactionUnityRequest(m_rpcUrl, m_account);
        print("it's about to call Approve");
        yield return approveReq.SendTransaction<ERC20ContractDefinition.ApproveFunction>(new ERC20ContractDefinition.ApproveFunction() { Spender = m_gameContract, Amount = m_approveAmount }, m_tokenContract, gameObject.name, nameof(ApproveResponse), nameof(DisplayError));
        print("passed the Approve");
    }

    async public void ApproveResponse(string rpcResponse)
    {
        print("ApproveResponse is---" + rpcResponse);
        var txnHash = MetamaskTransactionUnityRequest.DeserialiseTxnHashFromResponse(rpcResponse);
        print("ApproveResponse txnHash---" + txnHash);
        m_isApproved = true;
        await Task.Delay(2000);
        StartCoroutine(JoinGame(1));
    }

    public IEnumerator JoinGame(int _mode)
    {
        Debug.Log("called JoinGame---");
        yield return GetAllowance();
        Debug.Log("m_isApproved is---" + m_isApproved);

        if (!m_isApproved)
        {
            yield return Approve();
        }
        Debug.Log("2nd m_isApproved is---" + m_isApproved);
        if (m_isApproved)
        {
            Debug.Log("3rd m_isApproved is---" + m_isApproved);
            var x = new MetamaskTransactionUnityRequest(m_rpcUrl, m_account);
            print("it's about to call JoinGame");
            yield return x.SendTransaction<ERC20ContractDefinition.JoinGameFunction>(new ERC20ContractDefinition.JoinGameFunction() { Mode = _mode }, m_gameContract, gameObject.name, nameof(JoinGameResponse), nameof(DisplayError));
            print("passed the JoinGame");
        }
    }

    public IEnumerator JoinOnlineGame(int _mode)
    {
        Debug.Log("called JoinOnlineGame---");
        yield return GetAllowance();
        Debug.Log("m_isApproved is---" + m_isApproved);

        if (!m_isApproved)
        {
            yield return Approve();
        }
        Debug.Log("2nd m_isApproved is---" + m_isApproved);
        if (m_isApproved)
        {
            Debug.Log("3rd m_isApproved is---" + m_isApproved);
            var x = new MetamaskTransactionUnityRequest(m_rpcUrl, m_account);
            print("it's about to call JoinOnlineGame");
            yield return x.SendTransaction<ERC20ContractDefinition.JoinGameFunction>(new ERC20ContractDefinition.JoinGameFunction() { Mode = _mode }, m_gameContract, gameObject.name, nameof(JoinGameResponse), nameof(DisplayError));
            print("passed the JoinOnlineGame");
        }
    }

    public void JoinGameResponse(string rpcResponse)
    {
        print("JoinGameResponse is---" + rpcResponse);
        var txnHash =  MetamaskTransactionUnityRequest.DeserialiseTxnHashFromResponse(rpcResponse);
        print("JoinGameResponse txnHash---" + txnHash);
        m_isJoinedGame = true;
        FlashScreen.Instance.StartGame();
        //if(b_isMultiGame) KoroMainMenu.Instance.JoinOnlineMode();
        //else KoroMainMenu.Instance.JoinStoryMode();
    }

    
    public IEnumerator TransferRewardTokenAfterStoryMode()
    {
        Debug.Log("called TransferRewardTokenAfterStoryMode---");
        var storyRewardTokenReq = new TransactionSignedUnityRequest(m_rpcUrl, m_adminPrivateKey, m_chainId);
        storyRewardTokenReq.UseLegacyAsDefault = true;
        var transHash = "";
        do
        {
            var rewardTokenMsg = new ERC20ContractDefinition.NStoryRewardToken
            {
                FromAddress = m_adminPublicKey,
                Address = m_account
            };

            yield return storyRewardTokenReq.SignAndSendTransaction(rewardTokenMsg, m_gameContract); 
            transHash = storyRewardTokenReq.Result;
            yield return new WaitForSeconds(1.0f);

        } while ((transHash == null || transHash == ""));

        m_isSentReward = true;
        Debug.Log("---Reward token txn hash---" + storyRewardTokenReq.Result);
    }
    public IEnumerator TransferRewardTokenAfterMultiMode(int reward)
    {
        Debug.Log("called TransferRewardTokenAfterMultiMode---");
        var multiRewardTokenReq = new TransactionSignedUnityRequest(m_rpcUrl, m_adminPrivateKey, m_chainId);
        multiRewardTokenReq.UseLegacyAsDefault = true;
        var transHash = "";
        do
        {
            var rewardTokenMsg = new ERC20ContractDefinition.NMultiRewardToken
            {
                FromAddress = m_adminPublicKey,
                Address = m_account,
                Reward = reward
            };

            yield return multiRewardTokenReq.SignAndSendTransaction(rewardTokenMsg, m_gameContract);
            transHash = multiRewardTokenReq.Result;
            yield return new WaitForSeconds(1.0f);

        } while ((transHash == null || transHash == ""));

        m_isSentReward = true;
        Debug.Log("---Reward token txn hash---" + multiRewardTokenReq.Result);
    }

    public IEnumerator TransferRewardTokenAfterStoryMode_(int _round, int _bonusTime)
    {
        Debug.Log("called TransferRewardToken---");
        var rewardTokenReq = new TransactionSignedUnityRequest(m_rpcUrl, m_adminPrivateKey, m_chainId);
        rewardTokenReq.UseLegacyAsDefault = true;
        var transHash = "";
        do
        {
            var rewardTokenMsg = new ERC20ContractDefinition.NRewardToken
            {
                FromAddress = m_adminPublicKey,
                Address = m_account,
                Round = _round,
                BonusTimes = _bonusTime
            };

            yield return rewardTokenReq.SignAndSendTransaction(rewardTokenMsg, m_gameContract);
            transHash = rewardTokenReq.Result;
            yield return new WaitForSeconds(1.0f);

        } while ((transHash == null || transHash == ""));

        m_isSentReward = true;
        Debug.Log("--- txn hReward tokenash---" + rewardTokenReq.Result);
    }

    public void SendRewardToken()
    {

        if (!b_isMultiGame)
        {
            Debug.Log("in send reward token----multigame   "+ b_isMultiGame);
            StartCoroutine(TransferRewardTokenAfterStoryMode());
        }
        else
        {
            Debug.Log("in send reward token----multigame    " + b_isMultiGame);
            StartCoroutine(TransferRewardTokenAfterMultiMode(7000));
        }
    }

    public void DisplayError(string errorMessage)
    {
        Debug.Log(errorMessage);
    }
}