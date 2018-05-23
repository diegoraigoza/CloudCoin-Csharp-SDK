using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CloudCoinCsharpSDK
{
    interface ICloudBankAccessable
    {
        BankKeys LoadKeysFromFile(string filepath);
        CloudBankUtils CloudBankUtils { get; }
    }

    interface ICloudBankUtils
    {
        int onesInBank { get; }
        int fivesInBank { get; }
        int twentyFivesInBank { get; }
        int hundredsInBank { get; }
        int twohundredfiftiesInBank { get; }
        Task showCoins();
        void loadStackFromFile(string filepath);
        void saveStackToFile(string filepath);
        string getStackName();
        Task sendStackToCloudBank();
        Task getStackFromCloudBank(int amountToWithdraw);
        Task getReceipt();
        Task getReceiptFromCloudBank();
        Task transferCloudCoins(string toPublicKey, int coinsToSend);
    }

    interface IKeys
    {
        string publickey { get; set; }

        string privatekey { get; set; }

        string email { get; set; }
    }

    interface IBankResponse
    {
        string bank_server { get; set; }
        string time { get; set; }
    }



}
