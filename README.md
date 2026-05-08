# Board Game eCommerce 

C# .NET web API designed to be a eCommerce engine to support a mock board game retail business. 

## Features

* Record eCommerce transactions in SQLite DB including data on:
    * Products
    * Sales
    * Customers


## Pre-requsites 

### Dependencies 

* .NET 10.0 installation


## Run Locally

Clone the project

```bash
  git clone git@github.com:cesca2/BoardGameECommerce.git
```

Go to the project directory

```bash
  cd BoardGameECommerce
```

Run the application

```bash
  dotnet run --project BoardGameCommerceAPI
```


## API Reference

In progress

### Get products

```http
  GET /api/Products
```
| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `string` | **optional**. Id of item to fetch , must be valid Guid|


## Data info
Data used to populate mock products are sourced from BoardGameGeek BGG XML API https://boardgamegeek.com/using_the_xml_api, and BoardGamePrices https://boardgameprices.co.uk/api/plugin.

`.csv` files containing data directly from these files is found under `BoardGameData` and these data are used to initialise the Products table. 

## Acknowledgements

Project inspiration from https://www.thecsharpacademy.com/project/18/ecommerce-api 