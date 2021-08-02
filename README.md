# Simucraft

Tabletop simulator to set up game rules, maps, and play them out with other users online. This was originally built with `Azure` and requires integration with it for out of the box use.

## Requirements

* ASPNETCORE 3.0
* Blazor rc
* [Azure Emulation](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409)
	- Azure Cosmos DB Emulator
	- Microsoft Azure Storage Emulator

## Environment Variables

* `AZURE_DATABASE_CONNECTION_STRING`: found in `Azure Cosmos DB Emulator`, e.g `AccountEndpoint=https://localhost:8081/;AccountKey={key}`
* `AZURE_STORAGE_CONNECTION_STRING`: `UseDevelopmentStorage=true`
* `SENDGRID_APIKEY`: ``
* `SIMUCRAFT_EMAIL`: ``

## B2C configuration

B2C authentication requires the `Authority` and `ClientId` to be filled out in the `appsettings.json` files.

```
{
  "AzureAdB2C": {
    "Authority": "",
    "ClientId": "",
    "ValidateAuthority": false
  }
}
```