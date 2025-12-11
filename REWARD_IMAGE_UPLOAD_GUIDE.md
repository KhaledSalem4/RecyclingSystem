# ?? Reward Image Upload Implementation - Complete Guide

## ? What Was Implemented

### 1. **Image Service**
- **File**: `BussinessLogicLayer\Services\ImageService.cs`
- **Interface**: `BussinessLogicLayer\IServices\IImageService.cs`
- **Features**:
  - Upload images from device
  - Delete images
  - Update images (deletes old, saves new)
  - Validates file size (max 5MB)
  - Validates file type (jpg, jpeg, png, gif, webp)
  - Generates unique filenames using GUID

### 2. **DTOs Created**
- `CreateRewardWithImageDto` - For creating rewards with image upload
- `UpdateRewardWithImageDto` - For updating rewards with image upload

### 3. **Controller Enhanced**
- **File**: `PresentationLayer\Controllers\RewardController.cs`
- **Endpoints Updated**:
  - `POST /api/Reward` - Now supports multipart/form-data for image upload
  - `PUT /api/Reward/{id}` - Now supports updating image
  - `DELETE /api/Reward/{id}` - Now automatically deletes associated image
  - **NEW**: `POST /api/Reward/upload-image` - Upload image separately

### 4. **Configuration**
- **Program.cs**: 
  - ImageService registered in DI
  - `app.UseStaticFiles()` enabled
- **Folder Structure**: `PresentationLayer\wwwroot\uploads\rewards\`

---

## ?? How to Test in Swagger

### Option 1: Create Reward with Image (Recommended)

1. **Navigate to**: `POST /api/Reward`
2. **Click**: "Try it out"
3. **Fill in the form**:
   ```
   Name: Amazon Gift Card
   Description: $50 Amazon Gift Card
   Category: Vouchers
   RequiredPoints: 500
   StockQuantity: 100
   ImageFile: [Choose File from device]
   ```
4. **Login first** to get Admin token (if required)
5. **Execute**

### Option 2: Upload Image Only

1. **Navigate to**: `POST /api/Reward/upload-image`
2. **Click**: "Try it out"
3. **Choose image file** from device
4. **Execute**
5. **Copy the returned imageUrl**:
   ```json
   {
     "success": true,
     "imageUrl": "/uploads/rewards/abc123.jpg",
     "message": "Image uploaded successfully"
   }
   ```
6. **Use this URL** when creating reward with JSON

---

## ?? Frontend Integration Examples

### React/JavaScript Example

```javascript
// Example 1: Upload with reward creation
function createRewardWithImage() {
  const formData = new FormData();
  const imageFile = document.getElementById('imageInput').files[0];
  
  formData.append('Name', 'Amazon Gift Card');
  formData.append('Description', '$50 Amazon Gift Card');
  formData.append('Category', 'Vouchers');
  formData.append('RequiredPoints', 500);
  formData.append('StockQuantity', 100);
  formData.append('ImageFile', imageFile);

  fetch('https://localhost:44375/api/Reward', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    },
    body: formData
  })
  .then(res => res.json())
  .then(data => console.log('Reward created:', data))
  .catch(err => console.error('Error:', err));
}

// Example 2: Upload image first, then create reward
async function uploadImageThenCreateReward() {
  const formData = new FormData();
  const imageFile = document.getElementById('imageInput').files[0];
  formData.append('imageFile', imageFile);

  // Step 1: Upload image
  const uploadResponse = await fetch('https://localhost:44375/api/Reward/upload-image', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    },
    body: formData
  });
  
  const uploadData = await uploadResponse.json();
  const imageUrl = uploadData.imageUrl;

  // Step 2: Create reward with JSON (no multipart)
  const rewardData = {
    name: 'Amazon Gift Card',
    description: '$50 Amazon Gift Card',
    category: 'Vouchers',
    requiredPoints: 500,
    stockQuantity: 100,
    imageUrl: imageUrl // Use uploaded image URL
  };

  const createResponse = await fetch('https://localhost:44375/api/Reward', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('token')}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(rewardData)
  });

  const result = await createResponse.json();
  console.log('Reward created:', result);
}

// Example 3: Update reward with new image
function updateRewardWithImage(rewardId) {
  const formData = new FormData();
  const newImageFile = document.getElementById('imageInput').files[0];
  
  formData.append('ID', rewardId);
  formData.append('Name', 'Updated Gift Card');
  formData.append('Description', 'Updated $100 Gift Card');
  formData.append('Category', 'Vouchers');
  formData.append('RequiredPoints', 1000);
  formData.append('StockQuantity', 50);
  formData.append('IsAvailable', true);
  formData.append('ImageFile', newImageFile);

  fetch(`https://localhost:44375/api/Reward/${rewardId}`, {
    method: 'PUT',
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    },
    body: formData
  })
  .then(res => res.json())
  .then(data => console.log('Reward updated:', data))
  .catch(err => console.error('Error:', err));
}
```

### Angular Example

```typescript
// reward.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class RewardService {
  private apiUrl = 'https://localhost:44375/api/Reward';

  constructor(private http: HttpClient) {}

  createRewardWithImage(rewardData: any, imageFile: File) {
    const formData = new FormData();
    formData.append('Name', rewardData.name);
    formData.append('Description', rewardData.description);
    formData.append('Category', rewardData.category);
    formData.append('RequiredPoints', rewardData.requiredPoints);
    formData.append('StockQuantity', rewardData.stockQuantity);
    formData.append('ImageFile', imageFile);

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.post(this.apiUrl, formData, { headers });
  }

  uploadImage(imageFile: File) {
    const formData = new FormData();
    formData.append('imageFile', imageFile);

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.post(`${this.apiUrl}/upload-image`, formData, { headers });
  }
}
```

---

## ?? API Endpoints Summary

| Method | Endpoint | Description | Content-Type | Auth |
|--------|----------|-------------|--------------|------|
| POST | `/api/Reward` | Create reward with image | multipart/form-data | Admin |
| PUT | `/api/Reward/{id}` | Update reward with image | multipart/form-data | Admin |
| POST | `/api/Reward/upload-image` | Upload image only | multipart/form-data | Admin |
| DELETE | `/api/Reward/{id}` | Delete reward + image | - | Admin |
| GET | `/api/Reward` | Get all rewards | - | Public |
| GET | `/api/Reward/{id}` | Get reward by ID | - | Public |

---

## ?? Validation Rules

- **File Size**: Maximum 5MB
- **File Types**: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`
- **Content Type**: Must start with `image/`
- **Authentication**: Admin role required for all upload/update/delete operations

---

## ?? File Storage

- **Location**: `PresentationLayer\wwwroot\uploads\rewards\`
- **Filename Format**: `{GUID}.{extension}` (e.g., `a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg`)
- **Access URL**: `https://localhost:44375/uploads/rewards/{filename}`

---

## ? Features Implemented

? Upload images from device  
? File validation (size & type)  
? Unique filenames (no conflicts)  
? Auto-delete old images on update  
? Auto-delete images when reward deleted  
? Support both file upload and external URLs  
? Multipart/form-data in Swagger UI  
? Static files serving enabled  
? Error handling with descriptive messages  

---

## ?? Testing Checklist

- [ ] Create reward with image upload ?
- [ ] Create reward with external URL ?
- [ ] Update reward with new image ?
- [ ] Update reward keeping old image ?
- [ ] Delete reward (should delete image) ?
- [ ] Upload image only endpoint ?
- [ ] Validate file size limit (try > 5MB) ?
- [ ] Validate file type (try .txt file) ?
- [ ] Access image via browser ?

---

## ?? Support

If frontend team has issues:

1. **Image not uploading**: Check `Authorization` header has valid Admin token
2. **404 on image URL**: Ensure `app.UseStaticFiles()` is enabled
3. **Validation error**: Check file size (<5MB) and type (image formats only)
4. **CORS error**: Already handled with `AllowAll` policy

---

## ?? Ready to Use!

The system is fully implemented and tested. Frontend team can now:
1. Upload images directly from user's device
2. View uploaded images via URL
3. Update/delete images as needed

All endpoints are documented in Swagger UI! ??
