using System.Runtime.InteropServices;

public class MetamaskInterop
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    public static extern string EnableEthereum(string gameObjectName, string callback, string fallback);

    [DllImport("__Internal")]
    public static extern void EthereumInit(string gameObjectName, string callBackAccountChange, string callBackChainChange);

    [DllImport("__Internal")]
    public static extern void GetChainId(string gameObjectName, string callback, string fallback);   
    
    [DllImport("__Internal")]
    public static extern bool IsMetamaskAvailable();

    [DllImport("__Internal")]
    public static extern string GetSelectedAddress();

    [DllImport("__Internal")]
    public static extern string Request(string rpcRequestMessage, string gameObjectName, string callback, string fallback);

    [DllImport("__Internal")]
    public static extern void AlertInsufficientERC20();

    [DllImport("__Internal")]
    public static extern void AlertInstallMetamask();

    [DllImport("__Internal")]
    public static extern void AlertChangeChain();

#endif
}

