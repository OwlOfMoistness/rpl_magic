using System;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using System.Threading.Tasks;
using System.Numerics;

namespace RPLBot {

    public class Web3Net {
        public static string RPC = "http://localhost:8545";

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
}
