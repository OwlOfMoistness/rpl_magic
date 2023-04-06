using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json.Linq;
using Discord.Commands;
using Discord.WebSocket;

namespace RPLBot {
    public class RPLCommands : ModuleBase {
        private bool IsDev(ICommandContext context) => context.Message.Author.Id == 195567858133106697;

        private bool IsMod(ICommandContext context) {
            if (IsDev(context))
                return true;
            if (context.Guild.Id != 704949262802485348)
                return false;
            var roles = context.Guild.Roles;
            foreach (var role in roles)
                if (role.Name.StartsWith("Mod") || role.Name.StartsWith("Core") || role.Name.StartsWith("Jihoz") || role.Name.StartsWith("Staff"))
                    return true;
            return false;
        }


        static RPLCommands() {
        }

        public static Embed GenerateLuckEmbedMessage(string ens, BigInteger node, BigInteger sp, int spc, int mp) {
            var performance = node * 10000 / sp;
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Orange).WithTitle($"**{ens} Node Performance**");
            embed.WithDescription($"`SP mev =   {Web3Net.ToEther(sp)}Ξ/node/day`\n`Node mev = {Web3Net.ToEther(node)}Ξ/node/day`\n" +
                $"You are {((performance > 10000) ? "over" : "under")}performing {((performance > 10000) ? "📈" : "📉")} compared to the smoothing pool\n Performance = {(float)performance / 100f}%");
            embed.WithFooter($"Smoothing Pool count = {spc} | Node's pool count = {mp}\nBot build by owl.eth");

            return embed.Build();
        }

        [Command("block", RunMode = RunMode.Async)]
        public async Task GetLuck(int block) {
            await ReplyAsync($"{await Web3Net.GetBlockTimeStamp(block)}");
            //await ReplyAsync($"{await Web3Net.GetTransactionCount(add)}");
        }

            [Command("luck", RunMode =RunMode.Async)]
        public async Task GetLuck(string address, int timestamp = 1664578800) {
            var ens = address;
            Console.WriteLine($"1)");
            if (address.EndsWith(".eth")) {
                address = await Web3Net.ens(address.ToLower());
            }
            if (!IsAddressValid(address)) return;
                address = Web3Net.GetChecksumAddress(address);
            Console.WriteLine($"2)");
            var spData = JArray.Parse(await RocketscanAPI.GetSMoothingPoolData());
            Console.WriteLine($"3)");
            var userData = JObject.Parse(await RocketscanAPI.GetNodeData(address));
            Console.WriteLine($"4)");
            var attestationData = JObject.Parse(await RocketscanAPI.GetAttestationData(address));
            Console.WriteLine($"5)");
            var dict = GenerateMPDictionnary(attestationData);
            var userStack = GenerateMevStack(userData);

            var firstMevTimestamp = await Web3Net.GetBlockTimeStamp(userStack.Peek().BlockNumber);
            if (firstMevTimestamp > timestamp)
                timestamp = firstMevTimestamp - 1;
            var now = Convert.ToInt32(((DateTimeOffset)(DateTime.UtcNow)).ToUnixTimeSeconds());
            int gap = (now - timestamp) / 86400;


            int firstBlockReward = 0;
            BigInteger gainsOfDay = 0;
            BigInteger averagePerQuarterDayPerPoolCumul2 = 0;
            var s = userStack.Count;
            while (userStack.Count > 0 ){
                Console.WriteLine($"\r{userStack.Count}/{s}");
                var mev = userStack.Pop();
                var blockNumber = mev.BlockNumber;
                var blockTimestamp = await Web3Net.GetBlockTimeStamp(blockNumber);
                var gains = mev.Value;

                if (blockTimestamp < timestamp)
                    continue;
                if (firstBlockReward == 0) {
                    firstBlockReward = blockTimestamp;
                    gainsOfDay = gains;
                    continue;
                }
                if (firstBlockReward + 86400 > blockTimestamp) {
                    gainsOfDay += gains;
                    if (userStack.Count == 0) {
                        var mpAtTime = GetMiniPoolsAtDate(dict, blockTimestamp);
                        averagePerQuarterDayPerPoolCumul2 += gainsOfDay / mpAtTime;
                    }
                }
                else {
                    var mpAtTime = GetMiniPoolsAtDate(dict, blockTimestamp);
                    var rewardsPerDayPerPool = gainsOfDay / mpAtTime;
                    averagePerQuarterDayPerPoolCumul2 += rewardsPerDayPerPool;

                    var skips = (blockTimestamp - firstBlockReward) / 86400;
                    firstBlockReward += 86400 * skips;
                    gainsOfDay = gains;
                    if (userStack.Count == 0) {
                        averagePerQuarterDayPerPoolCumul2 += gainsOfDay / mpAtTime;
                    }
                }
            }
            BigInteger nodeAveragePerDayPerPool = averagePerQuarterDayPerPoolCumul2 / gap;

            BigInteger currentBalance = 0;
            BigInteger averagePerQuarterDayPerPoolCumul = 0;
            BigInteger counter = 0;
            int mpc = 0;
            foreach (var mev in spData) {
                if ((int)mev["timestamp"] < timestamp)
                    continue;
                var balance = BigInteger.Parse((string)mev["balance"]);
                var minipools = BigInteger.Parse((string)mev["minipools"]);
                mpc = (int)mev["minipools"];
                if (balance > currentBalance) {
                    BigInteger splitGains = balance - currentBalance;
                    averagePerQuarterDayPerPoolCumul += splitGains / minipools;
                    currentBalance = balance;
                }
                else {
                    BigInteger splitGains = balance;
                    averagePerQuarterDayPerPoolCumul += splitGains / minipools;
                    currentBalance = balance;
                }
                counter++;
            }
            BigInteger spAveragePerDayPerPool = averagePerQuarterDayPerPoolCumul / counter * 4;

            await ReplyAsync(embed: GenerateLuckEmbedMessage(ens, nodeAveragePerDayPerPool, spAveragePerDayPerPool, mpc, dict.Last().Value));
        }

        public static int GetMiniPoolsAtDate(Dictionary<int, int> dict, int timestamp) {
            foreach (var kvpair in dict) {
                if (timestamp < kvpair.Key)
                    return kvpair.Value;
            }
            return dict.Last().Value;
        }

        public static Dictionary<int, int> GenerateMPDictionnary(JObject json) {
            var dict = new Dictionary<int, int>();
            var keys = json.Properties().Select(p => p.Name).ToList();
            foreach (var key in keys) {
                var date = DateTimeOffset.ParseExact(key, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                var timestamp = (int)date.ToUnixTimeSeconds();
                var mp = (int)json[key]["minipools"];
                if (dict.Count == 0) {
                    dict.Add(timestamp, mp);
                }
                else if (dict.Last().Value < mp) {
                    dict.Add(timestamp, mp);
                }
            }

            return dict;
        }

        public static Stack<MevData> GenerateMevStack(JObject data) {
            var s = new Stack<MevData>();
            var a = 0;
            try {
                foreach (var block in data["blocks"]) {
                    if (block["execution"] == null)
                        break;
                    Console.WriteLine($"{a++}");
                    var blockNumber = (int)block["execution"]["blockNumber"];
                    BigInteger gains = 0;
                    if (block["execution"]["transactions"].Count() > 0)
                        gains = BigInteger.Parse((string)block["execution"]["transactions"][0]["value"]);
                    else
                        gains = BigInteger.Parse((string)block["execution"]["blockPrioFees"]);
                    s.Push(new MevData(blockNumber, gains));
                }
                return s;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public static bool IsAddressValid(string address) {
            if (address.StartsWith("0x")) {
                char[] cleanAddress = address.ToLower().ToCharArray();
                if (cleanAddress.Length == 42) {
                    for (int i = 2; i < cleanAddress.Length; i++) {
                        if (!(cleanAddress[i] >= '0' && cleanAddress[i] <= '9' ||
                              cleanAddress[i] >= 'a' && cleanAddress[i] <= 'f')) return false;
                    }
                }
                else return false;
                return true;
            }
            return false;
        }
    }

    public class MevData {
        public int BlockNumber;
        public BigInteger Value;

        public MevData(int a, BigInteger v) {
            BlockNumber = a;
            Value = v;
        }
    }
}