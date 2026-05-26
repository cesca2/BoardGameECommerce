# Board Game eCommerce 

C# .NET Razor Pages project utilising a web API designed to be an eCommerce engine to support a mock board game retail business. 

## Features

* Record eCommerce transactions in SQLite DB including data on:
    * Products
    * Sales
    * Customers
* Provide User-Interface with Razor Pages ASP.NET Core, including:
    * User Registration & Login with POST/GET
    * Basket & Checkout to complete a sale with POST request
    * Complete product catalogue display with search capability (GET)

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

Run the applications

```bash
  dotnet run --project BoardGameCommerceAPI
  dotnet run --project BoardGameCommerceApp
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
| Query Parameters | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `SearchTerm`      | `string` | **Optional**. Key term to filter product name  |


### Get customer

```http
  GET /api/Customers
```
| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `string` | **optional**. Id of item to fetch , must be valid Guid|

### Create Customer

```http
  POST /api/Customers
```

EXAMPLE INPUT:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Jane Doe",
  "email": "jdoe@email.com"
}
```
| Field      | Type    | Required | Description                           |
| ---------- | ------- | -------- | ----------------------------------    |
| `id`     | string  | Optional     | Must be valid Guid  |
| `name`    | string  | Yes      | Name of customer |
| `email` | string  | Yes      | Customer email. **must be unique** |

### Create Sale

```http
  POST /api/Sales
```

EXAMPLE INPUT:
```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "customer_Id": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
    "quantitiesByProductID": {
      "3fa85f64-5717-4562-b3fc-2c963f66afa2": 1,
      "3fa85f64-5717-4562-b3fc-2c963f66afa1": 2,
      "3fa85f64-5717-4562-b3fc-2c963f66afa4": 3
    }
}
```
| Field      | Type    | Required | Description                           |
| ---------- | ------- | -------- | ----------------------------------    |
| `id`     | string  | Optional     | Must be valid Guid  |
| `customer_id`    | string  | Yes      | Must be valid Guid. Customer Id as in /Customers endpoint |
| `quantitiesByProductID` | Dictionary | Yes      | Product Ids as in /Products endpoint with associated quantity included in the sale.|



## Data info
Data used to populate mock products are sourced from BoardGameGeek BGG XML API https://boardgamegeek.com/using_the_xml_api, and BoardGamePrices https://boardgameprices.co.uk/api/plugin.

`.csv` files containing data directly from these files is found under `BoardGameData` and these data are used to initialise the Products table. 

## Acknowledgements

Project inspiration from https://www.thecsharpacademy.com/project/18/ecommerce-api 