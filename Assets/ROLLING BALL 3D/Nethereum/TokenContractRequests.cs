using Nethereum.JsonRpc.UnityClient;
using Nethereum.Metamask;
using System.Collections;
using System.Numerics;
using static Nethereum.Metamask.ERC20ContractDefinition;

public class TokenContractRequests : UnityRequest<BigInteger>
{
    private string _url;
    private string _tokenContractAddr;
    private string _gameContractAddr;
    private string _account;
    private QueryUnityRequest<BalanceOfFunction, BalanceOfOutputDTO> _balanceOfFunction;
    private QueryUnityRequest<AllowanceFunction, AllowanceOutputDTO> _allowanceFunction;

    public TokenContractRequests(string url, string tokenContractAddr, string defaultAccount, string gameContractAddr = "")
    {
        _url = url;
        _tokenContractAddr = tokenContractAddr;
        _gameContractAddr = gameContractAddr;
        _account = defaultAccount;
        _balanceOfFunction = new QueryUnityRequest<BalanceOfFunction, BalanceOfOutputDTO>(_url, defaultAccount);
        _allowanceFunction = new QueryUnityRequest<AllowanceFunction, AllowanceOutputDTO>(_url, defaultAccount);
    }

    public IEnumerator GetBalanceOf()
    {
        yield return _balanceOfFunction.Query(new BalanceOfFunction() { Account = _account }, _tokenContractAddr);

        if (_balanceOfFunction.Exception != null)
        {
            Exception = _balanceOfFunction.Exception;
            yield break;
        }

        var balance = _balanceOfFunction.Result.Balance;

        Result = balance;
    }

    public IEnumerator GetAllowance()
    {
        yield return _allowanceFunction.Query(new AllowanceFunction() { Owner = _account, Spender = _gameContractAddr }, _tokenContractAddr);

        if (_allowanceFunction.Exception != null)
        {
            Exception = _allowanceFunction.Exception;
            yield break;
        }

        var allowance = _allowanceFunction.Result.Allowance;

        Result = allowance;
    }
}