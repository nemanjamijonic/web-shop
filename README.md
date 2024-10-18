# Web Shop Project

## Description

This project is a web-based application developed using the .NET Framework and follows the MVC (Model-View-Controller) pattern. The application serves as a simple online shop where users can browse products, add them to the cart, and complete a purchase.

Instead of using a traditional database, all data in this project is stored in text files. This approach allows the project to be lightweight and easy to set up without needing additional database services.

## Technologies Used

- **.NET Framework**
- **MVC Architecture**
- **C#**
- **HTML, CSS (bootstrap), JavaScript** for the front-end
- **Text files** for data storage

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/nemanjamijonic/web-shop-mvc.git
   ```
2. **Navigate to the project folder:**

```bash
   git clone https://github.com/nemanjamijonic/web-shop-mvc.git
```

```bash
cd web-shop-mvc/WebShop
```

3. **Open the solution in Visual Studio:**

Double-click on the WebShop.sln file to open the solution in Visual Studio.

4. **Run the application:**

Set the project as the startup project and run it using IIS Express or your preferred server.

## File Structure

**Controllers:** Contains logic for handling user interactions and views.

**Models:** Defines the data structure and logic for the shop's entities. Models are divided into the following categories:

- **DatabaseModels:** Contains models representing data structures stored in the database, such as **FavouriteProduct.cs** and **ProductOwner.cs**.
- **Domain:** Contains core business logic entities, such as **User.cs**, **Product.cs**, and **Order.cs**. These models define key objects within the application domain.
- **DTO (Data Transfer Objects):** These objects are used for transferring data between application layers. It includes various input/output models like **CreateProductInput.cs**, **UpdateUserProfile.cs**, and **FavouriteProductInput.cs**.
- **Enums:** Contains enumerations such as **Role.cs**, **Gender.cs**, and **OrderState.cs**, which define sets of related constant values used in the business logic.

**Views:** HTML pages that represent the user interface of the application.

**Database:** Contains logic for reading and writing data to text files.

**AppData:** Contains text files used for data storage.

## Future Enhancements

1. Implementing a database for better data management.
2. Adding product search and filtering functionality.
3. Integrating payment gateways for real-time transactions.
