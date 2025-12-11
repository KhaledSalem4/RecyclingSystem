# Points System Documentation

## Overview
The Recycling System implements a point-based reward system where users earn points based on the weight and type of materials they recycle. Points are automatically calculated and awarded when an order is completed.

## Point Values per Kilogram

| Material Type | Points per Kilogram |
|--------------|---------------------|
| **Plastic**  | 5 points           |
| **Paper**    | 8 points           |
| **Can**      | 10 points          |

## How It Works

### 1. Order Creation
- Users create orders with materials specifying:
  - Material type (Plastic, Paper, or Can)
  - Weight in kilograms (Size field)
  - Other order details (factory, date, etc.)

### 2. Order Processing
- Orders start with status: `Pending`
- Collectors are assigned to pick up the materials
- Materials are delivered to factories

### 3. Order Completion & Points Award
- When an order status changes to `Completed`, the system:
  1. Calculates total points based on all materials in the order
  2. Awards points to the user's account
  3. Updates the order status

### Point Calculation Formula
```
Total Points = ? (Material Weight × Points Per Kg)
```

**Example:**
```
Order with:
- 3 kg of Plastic  = 3 × 5  = 15 points
- 2 kg of Paper    = 2 × 8  = 16 points  
- 1.5 kg of Can    = 1.5 × 10 = 15 points
--------------------------------------------------
Total Points Awarded = 46 points
```

## API Endpoints

### Complete Order (Award Points)
```http
POST /api/order/{id}/complete
```

**Response:**
```json
{
  "message": "Order completed successfully",
  "pointsAwarded": 46,
  "pointsBreakdown": {
    "plastic": "5 points per kg",
    "paper": "8 points per kg",
    "can": "10 points per kg"
  }
}
```

### Get Estimated Points (Before Completion)
```http
GET /api/order/{id}/points
```

**Response:**
```json
{
  "orderId": 123,
  "estimatedPoints": 46,
  "pointsBreakdown": {
    "plastic": "5 points per kg",
    "paper": "8 points per kg",
    "can": "10 points per kg"
  }
}
```

## Business Rules

### Points Award Conditions
? **Points ARE awarded when:**
- Order status changes to `Completed`
- User account is active
- Materials are properly recorded

? **Points are NOT awarded when:**
- Order is `Cancelled`
- Order is already `Completed` (prevents double-awarding)
- Materials have invalid types

### Error Handling
- **Order Not Found**: Returns 404 error
- **Already Completed**: Returns 400 "Order is already completed"
- **Cancelled Order**: Returns 400 "Cannot complete a cancelled order"
- **Invalid Material Type**: Ignored in calculation (no points for that material)

## Technical Implementation

### Services Layer (OrderService)
```csharp
// Points configuration
private readonly Dictionary<string, int> _pointsPerKg = new()
{
    { MaterialType.Plastic.ToString(), 5 },
    { MaterialType.Paper.ToString(), 8 },
    { MaterialType.Can.ToString(), 10 }
};

// Complete order and award points
public async Task<bool> CompleteOrderAsync(int orderId)
{
    var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
    int totalPoints = CalculatePointsForOrder(order.Materials);
    
    order.Status = OrderStatus.Completed;
    user.Points += totalPoints;
    
    await _unitOfWork.SaveChangesAsync();
    return true;
}
```

### Database Schema
```sql
-- ApplicationUser table includes Points column
CREATE TABLE AspNetUsers (
    Id nvarchar(450) PRIMARY KEY,
    FullName nvarchar(100) NOT NULL,
    Points int NOT NULL DEFAULT 0,
    ...
)

-- Material table stores weight (Size) and type
CREATE TABLE Materials (
    ID int PRIMARY KEY IDENTITY,
    TypeName nvarchar(100),  -- 'Plastic', 'Paper', or 'Can'
    Size float(18),          -- Weight in kilograms
    Price decimal(18,2),
    ...
)

-- Orders link users with materials
CREATE TABLE Orders (
    ID int PRIMARY KEY IDENTITY,
    Status nvarchar(20) NOT NULL,  -- 'Pending', 'Completed', 'Cancelled'
    UserId nvarchar(450) NOT NULL,
    ...
)

-- OrderMaterial junction table (Many-to-Many)
CREATE TABLE OrderMaterial (
    MaterialId int NOT NULL,
    OrderId int NOT NULL,
    PRIMARY KEY (MaterialId, OrderId)
)
```

## Usage Example

### Step 1: Create Order with Materials
```http
POST /api/order
Content-Type: application/json

{
  "status": "Pending",
  "orderDate": "2025-02-01",
  "userId": "user123",
  "factoryId": 1,
  "materials": [
    { "typeName": "Plastic", "size": 3.0, "price": 15.00 },
    { "typeName": "Paper", "size": 2.0, "price": 20.00 },
    { "typeName": "Can", "size": 1.5, "price": 25.00 }
  ]
}
```

### Step 2: Check Estimated Points
```http
GET /api/order/123/points

Response:
{
  "orderId": 123,
  "estimatedPoints": 46
}
```

### Step 3: Complete Order (Award Points)
```http
POST /api/order/123/complete

Response:
{
  "message": "Order completed successfully",
  "pointsAwarded": 46
}
```

### Step 4: Check User Points
```http
GET /api/user/user123

Response:
{
  "id": "user123",
  "fullName": "John Doe",
  "points": 146  // Previous 100 + newly awarded 46
}
```

## Future Enhancements

Potential improvements to the points system:

1. **Dynamic Point Values**
   - Store point values in database for easy configuration
   - Allow admins to adjust point rates

2. **Bonus Multipliers**
   - Weekend/holiday bonus points
   - First-time user bonuses
   - Bulk recycling incentives

3. **Point Expiration**
   - Points expire after 12 months
   - Encourage regular participation

4. **Tiered Rewards**
   - Bronze/Silver/Gold user levels
   - Higher tiers earn more points per kg

5. **Point History**
   - Track point transactions
   - Show point earning and redemption history

6. **Referral Bonuses**
   - Earn points for referring new users
   - Community growth incentives

## Testing

### Test Scenarios

1. **Basic Point Award**
   - Create order with materials
   - Complete order
   - Verify points awarded correctly

2. **Multiple Materials**
   - Order with all three material types
   - Verify each material calculated correctly

3. **Edge Cases**
   - Already completed order (should fail)
   - Cancelled order (should fail)
   - Order with no materials (0 points)
   - Invalid material type (ignored)

4. **Transaction Integrity**
   - Points and status updated atomically
   - Rollback on failure

### Sample Test Data
```csharp
// Test Case 1: Single material type
Materials: [Plastic: 5kg]
Expected Points: 25

// Test Case 2: Mixed materials
Materials: [Plastic: 2kg, Paper: 3kg, Can: 1kg]
Expected Points: (2×5) + (3×8) + (1×10) = 44

// Test Case 3: Fractional weights
Materials: [Plastic: 2.5kg, Paper: 1.75kg]
Expected Points: (2.5×5) + (1.75×8) = 26 (rounded)
```

## Support

For questions or issues with the points system:
- Check order status before completing
- Verify materials are properly recorded
- Ensure material types match exactly: "Plastic", "Paper", "Can"
- Contact support if points not awarded correctly

---

**Last Updated:** 2025-02-01  
**Version:** 1.0.0
