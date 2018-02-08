using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCoinCsharpSDK
{
    interface IAccessCloudBank
    {
        BankKeys LoadKeysFromFile(string filepath);
        CloudBankUtils CloudBankUtils { get; }
    }

    interface IShowCoins
    {
        int onesInBank { get; }
        int fivesInBank { get; }
        int twentyFivesInBank { get; }
        int hundresInBank { get; }
        int twohundredfiftiesInBank { get; }
        Task showCoins();
    }

    interface ISendAndRecieveStacks
    {
        void loadStackFromFile(string filepath);
        void saveStackToFile(string filepath);
        string getStackName();
        Task sendStackToCloudBank(string toPublicUrl);
        Task getStackFromCloudBank(int amountToWithdraw);
    }
}
