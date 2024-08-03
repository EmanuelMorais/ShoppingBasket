# Shopping Basket Application

## Overview

This project is a simple shopping basket application consisting of a .NET backend and a React frontend. The backend handles business logic and API endpoints, while the frontend provides the user interface for interacting with the application.

## Folder Structure

- `ShoppingBasketApi/` - Contains the .NET backend application.
- `shopping-basket-frontend/` - Contains the React frontend application. This folder is also copied to `ShoppingBasketApi/wwwroot` during the build process for serving static files.

## Getting Started

- You will be able to add, remove and change the quantity of some items and calculate the total amount. Some discounts will be applied accordingly with the requirements
### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or later)
- [Node.js](https://nodejs.org/) (version 14.x or later)
- [npm](https://www.npmjs.com/get-npm) or [yarn](https://yarnpkg.com/) for managing frontend dependencies

### Setup

1. **Clone the repository:**

   ```bash
   git clone <https://github.com/EmanuelMorais/ShoppingBasket.git>
   cd ShoppingBasket
2. **Build and run**

    ```bash
    dotnet build
    dotnet run

3. **UI**

    Every change done to the UI code, you just need to call 'make' in the fodler projects and the contents will be copied to the `ShoppingBasketApi/wwwroot`