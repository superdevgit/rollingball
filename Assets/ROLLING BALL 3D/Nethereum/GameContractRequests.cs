using Nethereum.JsonRpc.UnityClient;
using System.Collections;
using System.Numerics;
using static Nethereum.Metamask.ERC20ContractDefinition;

public class GameContractRequests : UnityRequest<BigInteger>
{
    private string _url;
    private string _contractAddr;
    private int _mode;
    private QueryUnityRequest<EnterPriceFunction, EnterPriceOutputDTO> _enterPrice;

    public GameContractRequests(string url, string contractAddress, int mode, string defaultAccount)
    {
        _url = url;
        _contractAddr = contractAddress;
        _mode = mode;
        _enterPrice = new QueryUnityRequest<EnterPriceFunction, EnterPriceOutputDTO>(_url, defaultAccount);
    }

    public IEnumerator GetEnterPrice()
    {
        yield return _enterPrice.Query(new EnterPriceFunction() { Index = _mode }, _contractAddr);

        if (_enterPrice.Exception != null)
        {
            Exception = _enterPrice.Exception;
            yield break;
        }

        var enterPrice = _enterPrice.Result.EnterPrice;

        Result = enterPrice;
    }
}
