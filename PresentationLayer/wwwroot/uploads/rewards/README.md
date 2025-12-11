# Reward Image Upload System

## ?? Folder Structure Created
```
PresentationLayer/
  ??? wwwroot/
      ??? uploads/
          ??? rewards/
              (uploaded reward images will be saved here)
```

## ?? Implementation Summary

### 1. **ImageService** 
   - Location: `BussinessLogicLayer\Services\ImageService.cs`
   - Handles image upload, update, and deletion
   - Validates file size (max 5MB) and formats (jpg, jpeg, png, gif, webp)

### 2. **DTOs Created**
   - `CreateRewardWithImageDto` - For creating rewards with image upload
   - `UpdateRewardWithImageDto` - For updating rewards with image upload

### 3. **Controller Updated**
   - `RewardController` now supports multipart/form-data
   - Three endpoints for image handling:
     - `POST /api/Reward` - Create reward with image
     - `PUT /api/Reward/{id}` - Update reward with image
     - `POST /api/Reward/upload-image` - Upload image only

### 4. **Program.cs Updated**
   - ImageService registered in DI container
   - `app.UseStaticFiles()` enabled for serving uploaded images

## ?? How Frontend Team Should Use It

### Example 1: Create Reward with Image Upload
```javascript
const formData = new FormData();
formData.append('Name', 'Amazon Gift Card');
formData.append('Description', '$50 Amazon Gift Card');
formData.append('Category', 'Vouchers');
formData.append('RequiredPoints', 500);
formData.append('StockQuantity', 100);
formData.append('ImageFile', fileInputElement.files[0]); // The image file from device

fetch('https://localhost:44375/api/Reward', {
  method: 'POST',
  headers: {
    'Authorization': 'Bearer YOUR_JWT_TOKEN'
  },
  body: formData
});
```

### Example 2: Upload Image Only
```javascript
const formData = new FormData();
formData.append('imageFile', fileInputElement.files[0]);

fetch('https://localhost:44375/api/Reward/upload-image', {
  method: 'POST',
  headers: {
    'Authorization': 'Bearer YOUR_JWT_TOKEN'
  },
  body: formData
})
.then(res => res.json())
.then(data => {
  console.log('Image URL:', data.imageUrl);
  // Use this URL in ImageUrl field when creating reward
});
```

### Example 3: Update Reward with New Image
```javascript
const formData = new FormData();
formData.append('ID', 1);
formData.append('Name', 'Updated Reward');
formData.append('Description', 'Updated description');
formData.append('Category', 'Electronics');
formData.append('RequiredPoints', 300);
formData.append('StockQuantity', 50);
formData.append('IsAvailable', true);
formData.append('ImageFile', newImageFile); // New image file

fetch('https://localhost:44375/api/Reward/1', {
  method: 'PUT',
  headers: {
    'Authorization': 'Bearer YOUR_JWT_TOKEN'
  },
  body: formData
});
```

## ?? API Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/Reward` | Create reward with image | Admin |
| PUT | `/api/Reward/{id}` | Update reward with image | Admin |
| POST | `/api/Reward/upload-image` | Upload image only | Admin |
| DELETE | `/api/Reward/{id}` | Delete reward and its image | Admin |

## ? Features

- ? File size validation (max 5MB)
- ? File type validation (jpg, jpeg, png, gif, webp)
- ? Unique filenames using GUID
- ? Automatic old image deletion on update
- ? Automatic image deletion when reward is deleted
- ? Support for both file upload and external URLs
- ? Multipart/form-data support in Swagger

## ?? Security Notes

- All image upload endpoints require Admin authentication
- Only image files are accepted (validated by extension and content type)
- Files are saved in isolated folder (`wwwroot/uploads/rewards`)
- Maximum file size: 5MB

## ?? Accessing Uploaded Images

Images are accessible via:
```
https://localhost:44375/uploads/rewards/filename.jpg
```

Example:
```
https://localhost:44375/uploads/rewards/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg
```
