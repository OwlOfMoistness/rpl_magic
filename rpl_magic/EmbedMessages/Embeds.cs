using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using System.Text;
using System.Threading.Tasks;

namespace RPLBot.EmbedMessages {
    public class Embeds {
        public static Embed BasicEmbed(string title, string msg, Color color) {
            var embed = new EmbedBuilder();
            embed.WithColor(color);
            embed.WithTitle(title);
            embed.WithDescription(msg);
            return embed.Build();
        }

        public static Embed AirDropEmbed(ulong user, ulong[] reactUsers, string value, string symbol) {
            var embed = new EmbedBuilder();
            embed.WithTitle("Air drop Time! 🎊");
            embed.WithColor(Color.Red);
            var sb = new StringBuilder();
            for (int i = 0; i < reactUsers.Count(); i++) {
                if (i > 39)
                    break;
                sb.Append($"<@!{reactUsers[i]}> ");
            }
            var remaining = reactUsers.Count() - 40;
            sb.Remove(sb.Length - 1, 1);
            var strToAttach = "";
            if (remaining > 0)
                strToAttach = " and " + remaining.ToString() + " more";
            embed.WithDescription($"<@!{user}> has sent **{value} {symbol}** to {sb}{strToAttach}!");
            return embed.Build();
        }

        public static Embed ConvertEmbed(ulong id, string value, string fromSymbol, string toSymbol, string msg, string emote) {
            var embed = new EmbedBuilder();
            embed.WithTitle("Conversion");
            embed.WithDescription($"<@!{id}> here is your conversion.");
            embed.WithColor(Color.Teal);
            embed.AddField($"{emote} {fromSymbol} swapped to {emote} {toSymbol}", $"**{value} ➡️ {msg}**");
            return embed.Build();
        }

        public static Embed DepositEmbed(string baseUrl, string value, string symbol) {
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Green);
            embed.WithTitle("Click here to deposit Tokens");
            embed.WithUrl(baseUrl);
            embed.WithDescription($"Depositing {value} {symbol.ToUpper()} to the TipBot smart contract.");
            return embed.Build();
        }

        public static EmbedBuilder GetAwaitingRollEmbed(string game, string bet, string potentialGain, IUser user, string sym) {
            var embed = new EmbedBuilder();
            embed.WithTitle($"{game} game for {user.Username}");
            embed.WithColor(new Color(95, 211, 232));
            if (game.ToLower() == "coinflip")
                embed.AddField("Bet", bet);
            else if (game.ToLower() == "etheroll")
                embed.AddField("Bet", $"Roll under {bet}");
            embed.AddField("Result", "?", true);
            embed.AddField("Potential gain", $"{potentialGain} {sym}", true);
            return embed;
        }
        public static EmbedBuilder GetFinalRollEmbed(string game, string bet, string potentialGain, IUser user, string sym, bool won, string result) {
            var embed = new EmbedBuilder();
            embed.WithTitle($"{game} game result for {user.Username}");
            if (won) {
                embed.WithDescription("You have won your bet");
                embed.WithColor(Color.Green);
            }
            else {
                embed.WithDescription("You have lost your bet");
                embed.WithColor(Color.Red);
            }
            if (game.ToLower() == "coinflip")
                embed.AddField("Bet", bet);
            else if (game.ToLower() == "etheroll")
                embed.AddField("Bet", $"Roll under {bet}");
            embed.AddField("Result", result, true);
            if (won)
                embed.AddField("Won amount", $"{potentialGain} {sym}", true);
            else
                embed.AddField("Lost amount", $"{potentialGain} {sym}", true);
            embed.WithFooter("React 🔄 to rebet");
            return embed;
        }
    }
}
