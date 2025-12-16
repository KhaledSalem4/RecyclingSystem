# ?? Apply Address Fields Migration

## ? All Code Changes Have Been Applied Successfully!

The following changes have been made to your codebase:

### ?? Files Updated:
1. ? `DataAccessLayer\Entities\ApplicationUser.cs` - Added address fields
2. ? `BussinessLogicLayer\DTOs\AppUser\ApplicationUserDto.cs` - Added address fields
3. ? `BussinessLogicLayer\DTOs\AppUser\UpdateUserDto.cs` - Added address fields
4. ? `BussinessLogicLayer\DTOs\AppUser\RegisterUserDto.cs` - Added address fields
5. ? `BussinessLogicLayer\DTOs\AppUser\HireCollectorDto.cs` - Added address fields
6. ? `BussinessLogicLayer\DTOs\Order\OrderDto.cs` - Added user & collector address fields
7. ? `BussinessLogicLayer\Services\ApplicationUserService.cs` - Updated all methods to handle addresses
8. ? `BussinessLogicLayer\Services\AuthService.cs` - Updated RegisterAsync to save addresses
9. ? `BussinessLogicLayer\Services\OrderService.cs` - Updated ToDto to include addresses

---

## ??? Database Migration Steps

### **Step 1: Create Migration**

Open a terminal in your solution directory and run:

```bash
dotnet ef migrations add AddAddressFieldsToApplicationUser --project DataAccessLayer --startup-project PresentationLayer
```

### **Step 2: Review the Migration**

The migration will create SQL to add these columns to the `AspNetUsers` table:
- `City` (nvarchar, nullable)
- `Street` (nvarchar, nullable)
- `BuildingNo` (nvarchar, nullable)
- `Apartment` (nvarchar, nullable)

### **Step 3: Apply Migration to Database**

```bash
dotnet ef database update --project DataAccessLayer --startup-project PresentationLayer
```

### **Step 4: Verify in Database**

Check your SQL Server database:
- Table: `AspNetUsers`
- New columns should be visible

---

## ?? Testing the Changes

### **1. Test User Registration with Address**

```http
POST https://localhost:44375/api/auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "+1234567890",
  "password": "Test@123",
  "confirmPassword": "Test@123",
  "city": "Cairo",
  "street": "Main Street",
  "buildingNo": "15",
  "apartment": "3A"
}
```

### **2. Test Update User Profile**

```http
PUT https://localhost:44375/api/user/profile
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "+1234567890",
  "city": "Giza",
  "street": "Pyramid Road",
  "buildingNo": "20",
  "apartment": "5B"
}
```

### **3. Test Create Order (Address Auto-Included)**

```http
POST https://localhost:44375/api/order
Content-Type: application/json

{
  "email": "john@example.com",
  "typeOfMaterial": "Plastic",
  "quantity": 10,
  "city": "Cairo",
  "street": "Main St",
  "buildingNo": "15",
  "apartment": "3A"
}
```

### **4. Test Get Order (Should Return Address)**

```http
GET https://localhost:44375/api/order/1
```

**Expected Response:**
```json
{
  "id": 1,
  "status": "Pending",
  "orderDate": "2025-12-14",
  "userId": "user-123",
  "userName": "john@example.com",
  "userCity": "Cairo",
  "userStreet": "Main Street",
  "userBuildingNo": "15",
  "userApartment": "3A",
  "collectorId": null,
  "collectorName": null,
  "collectorCity": null,
  "collectorStreet": null,
  "collectorBuildingNo": null,
  "collectorApartment": null,
  "factoryId": 1,
  "factoryName": "Green Factory"
}
```

### **5. Test Hire Collector with Address**

```http
POST https://localhost:44375/api/collector/hire
Authorization: Bearer ADMIN_TOKEN
Content-Type: application/json

{
  "fullName": "Ahmed Collector",
  "email": "ahmed.collector@example.com",
  "password": "Collector@123",
  "phoneNumber": "+1234567890",
  "city": "Cairo",
  "street": "Collector Street",
  "buildingNo": "25",
  "apartment": "2C"
}
```

---

## ?? Flutter Integration

Update your Flutter app to include address fields:

### **Registration Model**
```dart
class RegisterRequest {
  final String fullName;
  final String email;
  final String phoneNumber;
  final String password;
  final String confirmPassword;
  final String? city;
  final String? street;
  final String? buildingNo;
  final String? apartment;

  Map<String, dynamic> toJson() => {
    'fullName': fullName,
    'email': email,
    'phoneNumber': phoneNumber,
    'password': password,
    'confirmPassword': confirmPassword,
    'city': city,
    'street': street,
    'buildingNo': buildingNo,
    'apartment': apartment,
  };
}
```

### **Order Response Model**
```dart
class OrderResponse {
  final int id;
  final String status;
  final String userId;
  final String userName;
  final String? userCity;
  final String? userStreet;
  final String? userBuildingNo;
  final String? userApartment;
  final String? collectorId;
  final String? collectorName;
  final String? collectorCity;
  final String? collectorStreet;
  final String? collectorBuildingNo;
  final String? collectorApartment;

  factory OrderResponse.fromJson(Map<String, dynamic> json) => OrderResponse(
    id: json['id'],
    status: json['status'],
    userId: json['userId'],
    userName: json['userName'],
    userCity: json['userCity'],
    userStreet: json['userStreet'],
    userBuildingNo: json['userBuildingNo'],
    userApartment: json['userApartment'],
    collectorId: json['collectorId'],
    collectorName: json['collectorName'],
    collectorCity: json['collectorCity'],
    collectorStreet: json['collectorStreet'],
    collectorBuildingNo: json['collectorBuildingNo'],
    collectorApartment: json['collectorApartment'],
  );
}
```

---

## ?? Important Notes

1. **Stop your application** if it's running before applying migrations
2. **Backup your database** before applying migrations
3. After migration, **restart your application**
4. If using **Hot Reload**, you may need to restart the app to apply changes

---

## ?? Rollback (If Needed)

If something goes wrong:

```bash
# Get previous migration name
dotnet ef migrations list --project DataAccessLayer

# Rollback to previous migration
dotnet ef database update PreviousMigrationName --project DataAccessLayer --startup-project PresentationLayer

# Or remove the migration entirely
dotnet ef migrations remove --project DataAccessLayer --startup-project PresentationLayer
```

---

## ? Verification Checklist

- [ ] Migration created successfully
- [ ] Database updated successfully
- [ ] User registration works with address
- [ ] User profile update works with address
- [ ] Order creation shows user address
- [ ] Order detail shows both user and collector addresses
- [ ] Collector hire works with address
- [ ] Flutter app updated to handle new fields

---

## ?? Success!

Once all steps are completed, your system will:
- ? Store user addresses in the database
- ? Display user addresses in order details
- ? Show collector addresses when assigned to orders
- ? Support address updates via profile management
- ? Include addresses in all API responses

**Happy Coding! ??**
