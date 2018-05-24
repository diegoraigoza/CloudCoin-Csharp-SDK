CloudCoin-Csharp-SDK
====================

A .NET Framework class library written to provide quick access to methods that communicate with a server that has CloudCoinConsortium's [CloudService](https://github.com/CloudCoinConsortium/CloudService) web API, in order to create applications that can use CloudCoins.

### Usage

Create a BankKeys object that contains the keys to the CloudService server you wish to connect to, then create an instance of the class CloudBankUtils using that BankKeys. Call CloudBankUtils' methods to communicate with the CloudService.

### Dependencies

Newtonsoft.Json is used to Deserialize CloudService's responses.
