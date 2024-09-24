# Currency Exchange Manager

## Overview

This application allows users to convert currencies using real-time exchange rates from an external API, caching the rates in Redis and storing conversion history in MySQL.

## Features

- Convert an amount from one currency to another using real-time exchange rates from an external API.
- Cache the latest currency rates in Redis for 15 minutes to reduce API calls.
- Store historical currency rates in a MySQL database for record-keeping.
- Provide an endpoint to fetch conversion history.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)
- [Redis](https://redis.io/download) (Or use a cloud-based Redis service like [Redis Labs](https://redislabs.com/))

## Setup

1. Clone the repository:
   git clone https://github.com/ArnoldRamodike/CurrencyConverterAPI
   cd CurrencyConverterAPI

2. restore and run build the roject:
   dotnet restore
   dotnet build

3. Run migrations to initialize the database:
   dotnet ef migrations add migrationsinit --output-dir Data/Migrations
   dotnet ef database update

4. To run the projects:
   dotnet run or dotnet watch --no-hot-reload

## Endpoints to run

1. Convert Currency: GET http://localhost:5000/convert?baseCurrency=ZAR&targetCurrency=EUR&amount=100
2. Get Conversion History: GET http://localhost:5000/history

**Set Up Environment Variables**

Create a new file named `appsettings.json` in the root of your project and add the following content:

```
#Database
 "DefaultConnection": ""

#Redis
"RedisConnection": ""
```
