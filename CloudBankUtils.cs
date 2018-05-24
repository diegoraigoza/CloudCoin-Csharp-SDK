using System.Collections.Generic;
using System.IO;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudCoinCsharpSDK
{
    

    public class CloudBankUtils : ICloudBankUtils
    {
        //Fields
        private BankKeys keys;
        private string rawStackForDeposit;
        private string rawStackFromWithdrawal;
        private string rawReceipt;
        private HttpClient cli;
        private string receiptNumber;
        private int totalCoinsWithdrawn;
        public int onesInBank { get; private set; }
        public int fivesInBank { get; private set; }
        public int twentyFivesInBank { get; private set; }
        public int hundredsInBank { get; private set; }
        public int twohundredfiftiesInBank { get; private set; }


        //Constructor

        public CloudBankUtils( BankKeys startKeys ) {
            keys = startKeys;
            cli = new HttpClient();
            totalCoinsWithdrawn = 0;
            onesInBank = 0;
            fivesInBank = 0;
            twentyFivesInBank = 0;
            hundredsInBank = 0;
            twohundredfiftiesInBank = 0;
        }//end constructor


        //Methods

        //Calls the CloudService's show_coins service for the server that this object holds the keys for.
        //The results are saved in this class's public properties if successful.
        public async Task showCoins()
        {
            //the private key is sent as form url encoded content
            var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("pk", keys.privatekey) });
            string json = "error";
            try
            {
                var showCoins = await cli.PostAsync("https://" + keys.publickey + "/show_coins.aspx", formContent);
                json = await showCoins.Content.ReadAsStringAsync();
                var bankTotals = JsonConvert.DeserializeObject<BankTotal>(json);
                if (bankTotals.status == "coins_shown")
                {
                    onesInBank = bankTotals.ones;
                    fivesInBank = bankTotals.fives;
                    twentyFivesInBank = bankTotals.twentyfives;
                    hundredsInBank = bankTotals.hundreds;
                    twohundredfiftiesInBank = bankTotals.twohundredfifties;
                }
                else
                {
                    Console.Out.WriteLine(bankTotals.status);
                    var failResponse = JsonConvert.DeserializeObject<FailResponse>(json);
                    Console.Out.WriteLine(failResponse.message);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.Out.WriteLine("Exception: " + ex.Message);
                Console.Out.WriteLine("Check your connection, or your public key");
                return;
            }//end try catch
            catch (JsonSerializationException ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(json);
            }
            catch (JsonReaderException ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(json);
            }
        }//end show coins

        //Sets rawStackForDeposit to a CloudCoin stack read from a file
        //param filepath is the full filepath and filename of the CloudCoin stack that is being loaded
        public void loadStackFromFile(string filepath)
        {
            rawStackForDeposit = File.ReadAllText(filepath);
        }

        //Sends the CloudCoin in rawStackForDeposit to the CloudService server that this object holds the keys for
        //loadStackFromFile needs to be called first
        public async Task sendStackToCloudBank()
        {
            string CloudBankFeedback = "";
            var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("stack", rawStackForDeposit) });
            try
            {
                var result_stack = await cli.PostAsync("https://" + keys.publickey + "/deposit_one_stack.aspx", formContent);
                CloudBankFeedback = await result_stack.Content.ReadAsStringAsync();
                var cbf = JsonConvert.DeserializeObject<DepositResponse>(CloudBankFeedback);
                Console.Out.WriteLine(cbf.message);
                receiptNumber = cbf.receipt;
            }
            catch (HttpRequestException ex)
            {
                Console.Out.WriteLine("Exception: " + ex.Message);
                Console.Out.WriteLine("Check your connection, or your public key");
                return;
            }
            catch (JsonSerializationException ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(CloudBankFeedback);
            }
            catch (JsonReaderException ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(CloudBankFeedback);
            }

        }//End send stack

        //Sends the CloudCoin in rawStackForDeposit to a CloudService server specified by param toPublicURL
        //loadStackFromFile needs to be called first
        public async Task sendStackToCloudBank(string toPublicURL)
        {
            string CloudBankFeedback = "";
            var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("stack", rawStackForDeposit) });
            try
            {
                var result_stack = await cli.PostAsync("https://" + toPublicURL + "/deposit_one_stack.aspx", formContent);
                CloudBankFeedback = await result_stack.Content.ReadAsStringAsync();
                var cbf = JsonConvert.DeserializeObject<DepositResponse>(CloudBankFeedback);
                Console.Out.WriteLine(cbf.message);
                receiptNumber = cbf.receipt;
            }
            catch (HttpRequestException ex)
            {
                Console.Out.WriteLine("Exception: " + ex.Message);
                Console.Out.WriteLine("Check your connection, or your public key");
                return;
            }
            catch (JsonSerializationException ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(CloudBankFeedback);
            }
            catch (JsonReaderException ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(CloudBankFeedback);
            }

        }//End send stack

        //Retrieve the receipt generated by the CloudService for the last sendStackToCloudBank call
        //Requires sendStackToCloudBank to have been previously called
        //The retrieved receipt will be saved in rawReceipt
        public async Task getReceipt()
        {
            try
            {
                var result_receipt = await cli.GetAsync("https://" + keys.publickey + "/" + keys.privatekey + "/Receipts/" + receiptNumber + ".json");
                rawReceipt = await result_receipt.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.Out.WriteLine("Exception: " + ex.Message);
                Console.Out.WriteLine("Check your connection, your public key, or you may not have made a Deposit yet.");
                return;
            }
            
        }//End get Receipt


        //Retrieves CloudCoins from CloudService server that this object holds the keys for.
        //param amountToWithdraw is the amount of CloudCoins to withdraw
        //The resulting stack that is retrieved is saved in rawStackFromWithdrawal
        public async Task getStackFromCloudBank( int amountToWithdraw)
        {
            totalCoinsWithdrawn = amountToWithdraw;
            var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string,string>("amount",amountToWithdraw.ToString()),
                new KeyValuePair<string, string>("pk", keys.privatekey)});
            try
            {
                var result_stack = await cli.PostAsync("https://" + keys.publickey + "/withdraw_account.aspx", formContent);
                rawStackFromWithdrawal = await result_stack.Content.ReadAsStringAsync();
                var failResponse = JsonConvert.DeserializeObject<FailResponse>(rawStackFromWithdrawal);
                Console.Out.WriteLine(failResponse.status);
                Console.Out.WriteLine(failResponse.message);
            }
            catch (HttpRequestException ex)
            {
                Console.Out.WriteLine("Exception: " + ex.Message);
                Console.Out.WriteLine("Check your connection, or your public key");
                return;
            }
            catch (JsonReaderException ex)
            {
                Console.Out.WriteLine(rawStackFromWithdrawal);
            }
            catch (JsonSerializationException ex)
            {
                Console.Out.WriteLine(rawStackFromWithdrawal);
            }
        }//End get stack from cloudbank

        //Calculate a CloudCoin note's denomination using it serial number(sn)
        private int getDenomination(int sn)
        {
            int nom = 0;
            if ((sn < 1))
            {
                nom = 0;
            }
            else if ((sn < 2097153))
            {
                nom = 1;
            }
            else if ((sn < 4194305))
            {
                nom = 5;
            }
            else if ((sn < 6291457))
            {
                nom = 25;
            }
            else if ((sn < 14680065))
            {
                nom = 100;
            }
            else if ((sn < 16777217))
            {
                nom = 250;
            }
            else
            {
                nom = '0';
            }

            return nom;
        }//end get denomination

        //Retrieves CloudCoins from CloudService server that this object holds the keys for.
        //The amount withdrawn is the same as the amount last deposited with sendStackToCloudBank
        //The resulting stack that is retrieved is saved in rawStackFromWithdrawal
        public async Task getReceiptFromCloudBank()
        {
            var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string,string>("rn",receiptNumber),
                new KeyValuePair<string, string>("pk", keys.privatekey)});
            try
            {
                var result_receipt = await cli.PostAsync("https://" + keys.publickey + "/get_receipt.aspx", formContent);
                string rawReceipt = await result_receipt.Content.ReadAsStringAsync();
                var deserialReceipt = JsonConvert.DeserializeObject<Receipt>(rawReceipt);
                for (int i = 0; i < deserialReceipt.rd.Length; i++)
                    if (deserialReceipt.rd[i].status == "authentic")
                        totalCoinsWithdrawn += getDenomination(deserialReceipt.rd[i].sn);
            }
            catch (HttpRequestException ex)
            {
                Console.Out.WriteLine("Exception: " + ex.Message);
                Console.Out.WriteLine("Check your connection, or your public key");
                return;
            }
            catch(JsonReaderException ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(rawReceipt);
                return;
            }
            catch(JsonSerializationException ex)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(rawReceipt);
                return;
            }


            try
            {
                var formContent2 = new FormUrlEncodedContent(new[] { new KeyValuePair<string,string>("amount",totalCoinsWithdrawn.ToString()),
                new KeyValuePair<string, string>("pk", keys.privatekey)});
                var result_stack = await cli.PostAsync("https://" + keys.publickey + "/withdraw_account.aspx", formContent2);
                rawStackFromWithdrawal = await result_stack.Content.ReadAsStringAsync();
                var failResponse = JsonConvert.DeserializeObject<FailResponse>(rawStackFromWithdrawal);
                Console.Out.WriteLine(failResponse.status);
                Console.Out.WriteLine(failResponse.message);
            }
            catch (JsonReaderException ex)
            {
                Console.Out.WriteLine(rawStackFromWithdrawal);
            }
            catch (JsonSerializationException ex)
            {
                Console.Out.WriteLine(rawStackFromWithdrawal);
            }
            catch (HttpRequestException ex)
            {
                Console.Out.WriteLine("Exception: " + ex.Message);
                Console.Out.WriteLine("Check your connection, or your public key");
            }

        }

        //Parses pertinent information from the receipt last gathered by getReceipt and returns it in the form of an Interpretation object
        public Interpretation interpretReceipt()
        {
            Interpretation inter = new Interpretation();
            string interpretation = "";
            try
            {
                //tell the client how many coins were uploaded how many counterfeit, etc.
                var deserialReceipt = JsonConvert.DeserializeObject<Receipt>(rawReceipt);
                int totalNotes = deserialReceipt.total_authentic + deserialReceipt.total_fracked;
                int totalCoins = 0;
                for (int i = 0; i < deserialReceipt.rd.Length; i++)
                    if (deserialReceipt.rd[i].status == "authentic")
                        totalCoins += getDenomination(deserialReceipt.rd[i].sn);
                interpretation = "receipt number: " + deserialReceipt.receipt_id + " total authentic notes: " + totalNotes + " total authentic coins: " + totalCoins;
                inter.interpretation = interpretation;
                inter.receipt = deserialReceipt;
                inter.totalAuthenticCoins = totalCoins;
                inter.totalAuthenticNotes = totalNotes;

            }catch(JsonSerializationException ex)
            {
                Console.Out.WriteLine(ex.Message);
                interpretation = rawReceipt;
            }
            catch(JsonReaderException ex)
            {
                Console.Out.WriteLine(ex.Message);
                interpretation = rawReceipt;
            }
           
            return inter;
        }

        //Writes a CloudCoin stack file for the CloudCoin retrieved the last call of either getStackFromCloudBank or getReceiptFromCloudBank
        //param path is the full file path where the new file will be written
        public void saveStackToFile(string path)
        {
            File.WriteAllText(path + getStackName(), rawStackFromWithdrawal);
            //WriteFile(path + stackName, rawStackFromWithdrawal);
        }

        //Generates a filename for the CloudCoin stack file to be written by saveStackToFile
        public string getStackName()
        {
            if (receiptNumber == null)
            {
                DateTime date = DateTime.Now;
                string tag = "Withdrawal" + date.ToString("MMddyyyyhhmmsff");
                return totalCoinsWithdrawn + ".CloudCoin." + tag + ".stack";
            }
            return totalCoinsWithdrawn + ".CloudCoin." + receiptNumber + ".stack";
        }

        //Calls getStackFromCloudBank and sendStackToCloudBank in order to transfer CloudCoins from one CloudService to another
        //param coinsToSend is the amount of CloudCoins to be transfered
        //param toPublicKey is the public url of the CloudService that is receiving the CloudCoins
        public async Task transferCloudCoins( string toPublicKey, int coinsToSend) {
            //Download amount
            await getStackFromCloudBank(coinsToSend);
            rawStackForDeposit = rawStackFromWithdrawal;//Make it so it will send the stack it recieved
            await sendStackToCloudBank( toPublicKey);
            //Upload amount
        }//end transfer


    }//end class
}//end namespace
