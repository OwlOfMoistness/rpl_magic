using System;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.Generic;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace RPLBot {

    public class Web3Net {
        public static string RPC = "http://192.168.1.199:8545";
        public static string RON_RPC = "http://localhost:8745";

        public Web3Net() {
        }

        public static async Task<string> ens(string name) {
            var web3 = new Web3(RPC);
            return (await web3.Eth.GetEnsService().ResolveAddressAsync(name));
        }   


        public static string GetChecksumAddress(string address) {
            return Nethereum.Util.AddressExtensions.ConvertToEthereumChecksumAddress(address);
        }

        public static decimal ToEther(BigInteger value) {
            return Nethereum.Util.UnitConversion.Convert.FromWei(value, Nethereum.Util.UnitConversion.EthUnit.Ether);
        }

        public static async Task<int> GetBlockTimeStamp(int block) {
            try {
                var web3 = new Web3(RPC);
                var hexBlock = new HexBigInteger(new BigInteger(block));
                var blockData = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(hexBlock);
                return int.Parse(blockData.Timestamp.ToString());
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public static async Task<(string[], List<BigInteger>)> GetRonStakes() {
            var consensusAddresses = new string[11] {
                "0x05ad3ded6fcc510324af8e2631717af6da5c8b5b",
                "0x4125217ce8868553e1f61bb030426efd330c2d68",
                "0x4e7ea047ec7e95c7a02cb117128b94ccdd8356bf",
                "0xae53daac1bf3c4633d4921b8c3f8d579e757f5bc",
                "0x61089875ff9e506ae78c7fe9f7c388416520e386",
                "0x47cfcb64f8ea44d6ea7fab32f13efa2f8e65eec1",
                "0xca54a1700e0403dcb531f8db4ae3847758b90b01",
                "0xbd4bf317da1928cc2f9f4da9006401f3944a0ab5",
                "0xedcafc4ad8097c2012980a2a7087d74b86bddaf9",
                "0x2bddcaae1c6ccd53e436179b3fc07307ee6f3ef8",
                "0x6aaabf51c5f6d2d93212cf7dad73d67afa0148d0"
            };

            var web3 = new Web3(RON_RPC);
            var funcParams = new GetManyStakingTotals() {
                PoolList = consensusAddresses,
            };
            Console.WriteLine("ok");
            var handler = web3.Eth.GetContractQueryHandler<GetManyStakingTotals>();
            var res = await handler.QueryAsync<List<BigInteger>>("0x545edb750eb8769c868429be9586f5857a768758", funcParams);
            Console.WriteLine("mmh");
            return (consensusAddresses, res);
        }

        public static async Task<BigInteger> GetTransactionCount(string address) {
            try {
                Web3 web3;
                web3 = new Web3(RPC);
                return (await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(address)).Value;
            }
            catch (Exception e) {
                Console.WriteLine("some error: " + e.Message);
                return 0;
            }
        }
    }

    [Function("getManyStakingTotals", "uint256[]")]
    public class GetManyStakingTotals : FunctionMessage {
        [Parameter("address[]", "_poolList", 1)]
        public string[] PoolList { get; set; }
    }

    [FunctionOutput]
    public class StakesOut : IFunctionOutputDTO {
        [Parameter("uint256[]", "stakes")]
        public List<BigInteger> Stakes { get; set; }
    }
}
