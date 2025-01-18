# Product Service

## Overview
The Product Service is a RESTful API designed to manage product entries and their price history. It provides endpoints to create, read, update, and delete product data, as well as read product price history. 

This solution uses Docker for containerization, making it easy to run on macOS, Linux, and Windows.

## Features
- Create new products
- Retrieve product details by Id
- Update existing products
- Delete products
- List all products
- List products by category
- List price history of products

## Prerequisites
Before running the solution, ensure you have the following installed:

### 1. Installations
- Install [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Install [Docker Compose](https://docs.docker.com/compose/install/) (if not bundled with Docker Desktop)
- [**For Windows Only**] Install and/or Use [Git Bash](https://git-scm.com/)
- Install Homebrew (if not installed)
    ```bash
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
    ```
- Install GNU Parallel
    ```bash
    brew install parallel
    ```
    or
    choco install parallel

### 2. Verify Installation
    ```bash
    docker --version
    docker-compose --version
    dotnet --version
    parallel --version
    ```

## Getting Started (Individual Service Setup)

Follow these steps to build and run the solution in Docker.

1. Clone the repository:
    ```bash
    git clone https://github.com/claurymr/product-service.git
    ```
2. Navigate to the project directory:
    ```bash
    cd product-service
    ```
3. Build and Run Solution:
    ```bash
    docker-compose up --build
    ```
4. Take down docker:
    ```bash
    docker-compose down
    ```

The service will be available at `http://localhost:8080` or its secure version `https://localhost:8081`.

> **Warning:** This solution is part of the Inventory Management System and should be run together with the rest of the services. The whole system is composed of the following components:
> - [API Gateway](https://github.com/claurymr/store-api) <--- Complete setup instructions can be found here.
> - [Inventory Auth Service](https://github.com/claurymr/inventory-auth-service)
> - [Product Service](https://github.com/claurymr/product-service) (this repository)
> - [Inventory Service](https://github.com/claurymr/inventory-service)


## API Endpoints

### Create a Product
- **URL:** `/products`
- **Method:** `POST`
- **Authorization:** `Bearer`
- **Request Body:**
    ```json
    {
        "name": "Product Name",
        "description": "Product Description",
        "price": 100.00,
        "category": "Category Name",
        "sku": "Sku Code"
    }
    ```
- **Response: 201 Created**
    ```Guid
    "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
    ```

### Get All Products
- **URL:** `/products`
- **Method:** `GET`
- **Authorization:** `Bearer`
- **Response: 200 Ok**
    ```json
    [
        {
            "id": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
            "name": "Product Name",
            "description": "Product Description",
            "price": 100.00,
            "currency": null,
            "category": "Category Name",
            "sku": "Sku Code"
        },
        {
            "id": 2,
            "name": "Another Product",
            "description": "Another Description",
            "price": 200.00,
            "currency": null,
            "category": "Category Name",
            "sku": "Sku Code"
        }
    ]
    ```

### Get a Product by Id
- **URL:** `/products/{id}`
- **Method:** `GET`
- **Authorization:** `Bearer`
- **Response: 200 Ok**
    ```json
    {
        "id": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        "name": "Product Name",
        "description": "Product Description",
        "price": 100.00,
        "currency": null,
        "category": "Category Name",
        "sku": "Sku Code"
    }
    ```

### Get a Product by Category
- **URL:** `/products/categories/{category}`
- **Method:** `GET`
- **Authorization:** `Bearer`
- **Response: 200 Ok**
    ```json
    {
        "id": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        "name": "Product Name",
        "description": "Product Description",
        "price": 100.00,
        "currency": null,
        "category": "Category Name",
        "sku": "Sku Code"
    }
    ```

### Update a Product
- **URL:** `/products/{id}`
- **Method:** `PUT`
- **Authorization:** `Bearer`
- **Request Body:**
    ```json
    {
        "name": "Updated Product Name",
        "description": "Updated Product Description",
        "price": 150.00,
        "category": "Category Name",
        "sku": "Sku Code"
    }
    ```
- **Response: 204 NoContent**


### Get Price History by Product Id
- **URL:** `/pricehistories/{productId}`
- **Method:** `GET`
- **Authorization:** `Bearer`
- **Response: 200 Ok**
    ```json
        [
            {
                "id": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                "productId": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                "productName": "Product Name",
                "productSku": "Sku Code",
                "oldPrice": 90.00,
                "newPrice": 100.00,
                "currency": "USD",
                "action": "Increased",
                "timestamp": "2023-10-01T12:00:00Z"
            },
            {
                "id": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                "productId": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                "productName": "Another Product",
                "productSku": "AnotherSkuCode",
                "oldPrice": 180.00,
                "newPrice": 200.00,
                "currency": "USD",
                "action": "Increased",
                "timestamp": "2023-10-02T12:00:00Z"
            }
        ]
        ```

### Delete a Product
- **URL:** `/products/{id}`
- **Method:** `DELETE`
- **Authorization:** `Bearer`
- **Response: 204 NoContent**

## Development Approach Decisions

### Clean Architecture
To ensure the maintainability and scalability of the Product Service, I implemented Clean Architecture. This approach separates the application into distinct layers, promoting a clear separation of concerns and making the codebase easier to manage and extend.

### CQRS Pattern
I adopted the Command Query Responsibility Segregation (CQRS) pattern to provide better control over the steps when triggering events. This pattern separates read and write operations, allowing for more efficient and scalable handling of data.

### RabbitMQ as Message Broker
For message brokering, I chose RabbitMQ due to its easy setup and robust features. RabbitMQ facilitates communication between different parts of the system, ensuring reliable message delivery and processing. Moreover, no background services are required to be set up as RabbitMQ takes care of tracking consumers and publishers.

### FastEndpoints for Minimal APIs
I used FastEndpoints to set up minimal APIs in a structured and efficient manner. FastEndpoints aligns well with the CQRS pattern, providing a clean and organized way to define API endpoints.

### FluentValidation for Request Data Validation
To ensure the validity of incoming requests, I implemented FluentValidation. This library provides a fluent interface for defining validation rules, making it easy to enforce data integrity and provide meaningful error messages.

### Global Exception Handling
I implemented a simple global exception handling to manage errors consistently across the application and avoid raw exception messages to be returned in the response. This approach ensures that all exceptions are caught and handled gracefully, providing a better user experience and easier debugging.

### SQLite as the Database
For the database, I chose SQLite due to its lightweight nature and minimal setup requirements. For the nature of this project and to use time more sparringly I deemed other db setups unnecessary. SQLite is an embedded database that is easy to use and suitable for the needs of the Product Service, especially during development and testing phases.

### JWT Auth
To handle authentication and authorization, I implemented a simple JWT Auth by setting up predetermined credentials that don't need to be saved to a database. This decision was taken to simplify the process of authentication and authorization while prioritizing other functionalities, as registration and login were not required for the nature of this project. This approach allows for secure access control without the overhead of managing user credentials in a database.