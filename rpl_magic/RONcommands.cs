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
    public class Validator {
        public string Address;
        public string Name;
        public BigInteger Stake;

        public Validator(string a, string n, BigInteger s) {
            Address = a;
            Name = n;
            Stake = s;
        }
    }
    public class RONCommands : ModuleBase {
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

        public static Dictionary<string, string> ConsensusToName = new Dictionary<string, string>() {
            {"0x05ad3ded6fcc510324af8e2631717af6da5c8b5b","MysticNode" },
            {"0x4125217ce8868553e1f61bb030426efd330c2d68","CyberKongz" },
            {"0x4e7ea047ec7e95c7a02cb117128b94ccdd8356bf","Ronin Catalyst" },
            {"0xae53daac1bf3c4633d4921b8c3f8d579e757f5bc","EternityHub" },
            {"0x61089875ff9e506ae78c7fe9f7c388416520e386","ak" },
            {"0x47cfcb64f8ea44d6ea7fab32f13efa2f8e65eec1","META8" },
            {"0xca54a1700e0403dcb531f8db4ae3847758b90b01","aur x arctic x cloud" },
            {"0xbd4bf317da1928cc2f9f4da9006401f3944a0ab5","BlockAhead" },
            {"0xedcafc4ad8097c2012980a2a7087d74b86bddaf9","BYAC" },
            {"0x2bddcaae1c6ccd53e436179b3fc07307ee6f3ef8","owl" },
            {"0x6aaabf51c5f6d2d93212cf7dad73d67afa0148d0","luganodes" }
        };


        static RONCommands() {
        }

        public static Embed GenerateStakeEmbedMessage(string[] adds, List<BigInteger> stakes) {
            var vals = new List<Validator>();
            for (int i = 0; i < 11; i++)
                vals.Add(new Validator(adds[i], ConsensusToName[adds[i]], stakes[i]));
            vals = vals.OrderByDescending(x => x.Stake).ToList();
            string str = "`";
            int b = 1;
            foreach (var val in vals) {
                var stake = val.Stake / BigInteger.Parse("1000000000000000000");
                str += $"{b}. {val.Name} - {stake} RON\n";
                b++;
            }
            str += "`";
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Orange).WithTitle($"**Validator Leaderboard**");

            embed.WithDescription(str);

            return embed.Build();
        }

        [Command("val", RunMode = RunMode.Async)]
        public async Task GetLuck() {
            (var addresses, var stakes) = await Web3Net.GetRonStakes();
            var embed = GenerateStakeEmbedMessage(addresses, stakes);
            await ReplyAsync(embed: embed);
            //await ReplyAsync($"{await Web3Net.GetTransactionCount(add)}");
        }

    }

}