using System;
using System.Threading.Tasks;
namespace RPLBot {
    public class RocketscanAPI {
        public RocketscanAPI() {
        }

        public static async Task<string> GetSMoothingPoolData() {
            string json = "";
            int safetyNet = 0;
            while (safetyNet < 5) {
                using (System.Net.WebClient wc = new System.Net.WebClient()) {
                    try {
                        json = await wc.DownloadStringTaskAsync("https://rocketscan.io/api/mainnet/smoothingpool");
                        return json;
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        safetyNet++;
                    }
                }
            }
            return json;
        }

        public static async Task<string> GetNodeData(string address) {
            string json = "";
            int safetyNet = 0;
            while (safetyNet < 5) {
                using (System.Net.WebClient wc = new System.Net.WebClient()) {
                    try {
                        json = await wc.DownloadStringTaskAsync("https://rocketscan.io/api/mainnet/beacon/blocks/node/" + address);
                        return json;
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        safetyNet++;
                    }
                }
            }
            return json;
        }

        public static async Task<string> GetAttestationData(string address) {
            string json = "";
            int safetyNet = 0;
            while (safetyNet < 5) {
                using (System.Net.WebClient wc = new System.Net.WebClient()) {
                    try {
                        json = await wc.DownloadStringTaskAsync("https://rocketscan.io/api/mainnet/attestations/node/" + address);
                        return json;
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        safetyNet++;
                    }
                }
            }
            return json;
        }
    }
}