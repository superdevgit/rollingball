using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Runtime.InteropServices;
using Nethereum.Contracts;
using System.Numerics;

namespace Nethereum.Metamask
{
    public class ERC20ContractDefinition
    {
        public partial class AllowanceFunction : AllowanceFunctionBase { }
        [Function("allowance", "uint256")]
        public class AllowanceFunctionBase : FunctionMessage
        {
            [Parameter("address", "owner", 1)]
            public string Owner { get; set; }

            [Parameter("address", "spender", 2)]
            public string Spender { get; set; }
        }

        public partial class AllowanceOutputDTO : AllowanceOutputDTOBase { }
        [FunctionOutput]
        public class AllowanceOutputDTOBase : IFunctionOutputDTO
        {
            [Parameter("uint256", "", 1)]
            public BigInteger Allowance { get; set; }
        }

        public class ApproveFunction : ApproveFunctionBase { }
        [Function("approve", "bool")]
        public class ApproveFunctionBase : FunctionMessage
        {
            [Parameter("address", "spender", 1)]
            public string Spender { get; set; }

            [Parameter("uint256", "amount", 1)]
            public BigInteger Amount { get; set; }
        }

        public partial class BalanceOfFunction : BalanceOfFunctionBase { }
        [Function("balanceOf", "uint256")]
        public class BalanceOfFunctionBase : FunctionMessage
        {
            [Parameter("address", "account", 1)]
            public string Account { get; set; }
        }

        public partial class BalanceOfOutputDTO : BalanceOfOutputDTOBase { }
        [FunctionOutput]
        public class BalanceOfOutputDTOBase : IFunctionOutputDTO
        {
            [Parameter("uint256", "", 1)]
            public BigInteger Balance { get; set; }
        }

        public class EnterPriceFunction : EnterPriceFunctionBase { }
        [Function("enterPrice", "uint256")]
        public class EnterPriceFunctionBase : FunctionMessage
        {
            [Parameter("uint256", "", 1)]
            public int Index { get; set; }
        }

        public partial class EnterPriceOutputDTO : EnterPriceOutputDTOBase { }
        [FunctionOutput]
        public class EnterPriceOutputDTOBase : IFunctionOutputDTO
        {
            [Parameter("uint256", "", 1)]
            public BigInteger EnterPrice { get; set; }
        }

        public class JoinGameFunction : JoinGameFunctionBase { }
        [Function("joinGame", "bool")]
        public class JoinGameFunctionBase : FunctionMessage
        {
            [Parameter("uint8", "mode", 1)]
            public int Mode { get; set; }
        }

       

        [Function("rewardPerRound", "bool")]
        public class NRewardToken : FunctionMessage
        {
            [Parameter("address", "user", 1)]
            public string Address { get; set; }

            [Parameter("uint8", "round", 2)]
            public int Round { get; set; }

            [Parameter("uint8", "bonusTimes", 3)]
            public int BonusTimes { get; set; }
        }
        [Function("rewardMultiGame", "bool")]
        public class NMultiRewardToken : FunctionMessage
        {
            [Parameter("address", "user", 1)]
            public string Address { get; set; }

            [Parameter("uint256", "rewardMulti", 2)]
            public int Reward { get; set; }

            

        }
        
        [Function("rewardAfterWinStoryMode", "bool")]
        public class NStoryRewardToken : FunctionMessage
        {
            [Parameter("address", "user", 1)]
            public string Address { get; set; }
        }

    }

}