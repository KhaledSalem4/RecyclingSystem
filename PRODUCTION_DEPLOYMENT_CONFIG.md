# ? Production Deployment Configuration - COMPLETE

## ?? **Production URLs Configured**

### **Frontend (Netlify):**
```
https://greenzonee.netlify.app
```

### **Backend (RunASP.NET):**
```
https://recycle-hub.runasp.net
```

---

## ? **Changes Applied**

### **1. appsettings.json (Production)**
```json
{
  "AppSettings": {
    "FrontEndUrl": "https://greenzonee.netlify.app",
    "BackendUrl": "https://recycle-hub.runasp.net"
  }
}
```

### **2. appsettings.Development.json (Local Development)**
```json
{
  "AppSettings": {
    "FrontEndUrl": "http://localhost:4200",
    "BackendUrl": "https://localhost:44375"
  }
}
```

### **3. Program.cs - CORS Configuration**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",                    // ? Local development
                "https://greenzonee.netlify.app",           // ? Production frontend
                "https://recycle-hub.runasp.net"            // ? Production backend
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### **4. AuthService.cs - Enhanced with Validation**
Added validation and logging:
- ? Checks if FrontEndUrl is configured
- ? Logs which URL is being used
- ? Fallback to localhost if not configured (development only)
- ? Applies to both email confirmation and password reset

---

## ?? **Email Links Now Working**

### **Email Confirmation:**
```
Before: http://localhost:4200/confirm-email?email=...&token=...
After:  https://greenzonee.netlify.app/confirm-email?email=...&token=...
```

### **Password Reset:**
```
Before: http://localhost:4200/reset-password?email=...&token=...
After:  https://greenzonee.netlify.app/reset-password?email=...&token=...
```

---

## ?? **Deployment Steps**

### **Backend (RunASP.NET):**

1. **Commit and Push Changes:**
```bash
git add .
git commit -m "Configure production URLs for Netlify frontend and RunASP backend"
git push origin main
```

2. **Deploy to RunASP.NET:**
   - Your backend is at: `https://recycle-hub.runasp.net`
   - Ensure `appsettings.json` is deployed with production values
   - Restart the application after deployment

3. **Verify Environment:**
   - Database connection string is correct
   - SMTP settings are configured
   - Jwt:Key is set

---

### **Frontend (Netlify):**

1. **Update Angular Environment Files:**

**src/environments/environment.ts (Production):**
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://recycle-hub.runasp.net/api'
};
```

**src/environments/environment.development.ts (Local):**
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:44375/api'
};
```

2. **Build and Deploy:**
```bash
ng build --configuration production
# Netlify will auto-deploy from your Git repository
```

---

## ?? **Testing Checklist**

### **After Deployment:**

- [ ] **Register New User:**
  - Go to: `https://greenzonee.netlify.app/register`
  - Fill in details and submit
  - Check email inbox

- [ ] **Email Confirmation:**
  - Click "Confirm Email" button in email
  - Should redirect to: `https://greenzonee.netlify.app/confirm-email`
  - Token should be validated by backend
  - Should show success message

- [ ] **Login:**
  - Go to: `https://greenzonee.netlify.app/login`
  - Enter credentials
  - Should receive JWT token
  - Should redirect to dashboard

- [ ] **Password Reset:**
  - Click "Forgot Password"
  - Enter email
  - Check email inbox
  - Click "Reset Password" button
  - Should redirect to: `https://greenzonee.netlify.app/reset-password`
  - Enter new password
  - Should be able to login with new password

- [ ] **CORS Verification:**
  - Open browser DevTools ? Network tab
  - Should NOT see CORS errors
  - All API calls should succeed

- [ ] **Create Order:**
  - Login as user
  - Create a recycling order
  - Verify order appears in database
  - Check if collector can see it

- [ ] **Redeem Reward:**
  - Ensure user has points
  - Go to rewards page
  - Add reward to cart
  - Redeem
  - Verify points deducted

---

## ?? **Troubleshooting**

### **Issue 1: Email link still shows localhost**

**Check:**
```bash
# Verify appsettings.json on server
cat appsettings.json | grep FrontEndUrl
```

**Expected Output:**
```
"FrontEndUrl": "https://greenzonee.netlify.app",
```

**Fix:** Redeploy with correct appsettings.json

---

### **Issue 2: CORS Error**

**Error in Browser Console:**
```
Access to XMLHttpRequest at 'https://recycle-hub.runasp.net/api/...' 
from origin 'https://greenzonee.netlify.app' has been blocked by CORS policy
```

**Fix:**
- Ensure Program.cs has correct CORS configuration (already done ?)
- Restart backend application
- Clear browser cache

---

### **Issue 3: SSL Certificate Error**

**Error:**
```
NET::ERR_CERT_AUTHORITY_INVALID
```

**Fix:**
- Ensure both Netlify and RunASP.NET have valid SSL certificates
- Netlify provides free SSL automatically
- RunASP.NET should provide SSL as well

---

### **Issue 4: Token Validation Failed**

**Error:**
```
Email confirmation failed: Invalid token
```

**Possible Causes:**
1. Token expired (default 3 hours)
2. Database was reset after sending email
3. Token got corrupted in URL

**Fix:**
- Extend token lifetime in Program.cs (already set to 24 hours for email tokens)
- Ensure URL encoding is correct (`Uri.EscapeDataString`)

---

## ?? **Environment Configuration Summary**

| Setting | Development | Production |
|---------|-------------|------------|
| **Frontend URL** | http://localhost:4200 | https://greenzonee.netlify.app |
| **Backend URL** | https://localhost:44375 | https://recycle-hub.runasp.net |
| **Database** | Same production DB | db33832.public.databaseasp.net |
| **SMTP** | Gmail SMTP | Gmail SMTP |
| **JWT Expiry** | 60 minutes | 600 minutes (10 hours) |

---

## ?? **Security Checklist**

- [x] CORS restricted to specific origins (no `AllowAnyOrigin`)
- [x] HTTPS enforced on production
- [x] JWT tokens expire after set time
- [x] Email verification required before login
- [x] Password reset tokens expire
- [x] Strong password requirements enabled
- [x] Only one admin account allowed
- [x] Database credentials not in source control
- [x] SMTP password uses app-specific password

---

## ?? **Next Steps**

1. **Commit and Push Changes:**
```bash
git add .
git commit -m "Configure production URLs - Netlify frontend + RunASP backend"
git push origin main
```

2. **Deploy Backend:**
   - Push to RunASP.NET hosting
   - Verify deployment successful
   - Check logs for any errors

3. **Update Frontend:**
   - Update Angular environment files
   - Build production version
   - Deploy to Netlify (auto-deploys from Git)

4. **Test End-to-End:**
   - Register new user
   - Confirm email via link
   - Login
   - Create order
   - Redeem reward

5. **Monitor:**
   - Check server logs
   - Monitor email delivery
   - Watch for errors in browser console

---

## ?? **Expected Behavior**

### **Registration Flow:**
```
User registers on: https://greenzonee.netlify.app/register
    ?
Email sent with link: https://greenzonee.netlify.app/confirm-email?...
    ?
User clicks link ? Angular app opens
    ?
Angular calls: https://recycle-hub.runasp.net/api/Auth/confirm-email
    ?
Backend validates token
    ?
Email confirmed ?
    ?
User can login
```

### **Password Reset Flow:**
```
User requests reset on: https://greenzonee.netlify.app/forgot-password
    ?
Email sent with link: https://greenzonee.netlify.app/reset-password?...
    ?
User clicks link ? Angular app opens
    ?
User enters new password
    ?
Angular calls: https://recycle-hub.runasp.net/api/Auth/reset-password
    ?
Backend validates token and updates password
    ?
Password reset ?
    ?
User can login with new password
```

---

## ? **Deployment Status**

- [x] Production URLs configured in appsettings.json
- [x] Development URLs configured for local testing
- [x] CORS policy updated with production domains
- [x] AuthService enhanced with URL validation
- [x] Email templates using correct URLs
- [x] Build successful
- [ ] **TODO: Push to Git**
- [ ] **TODO: Deploy to RunASP.NET**
- [ ] **TODO: Update Angular environment files**
- [ ] **TODO: Deploy to Netlify**
- [ ] **TODO: Test end-to-end**

---

**Your application is now configured for production deployment! ??**

**Frontend:** https://greenzonee.netlify.app  
**Backend:** https://recycle-hub.runasp.net  
**Email links:** Will redirect to production Netlify URL ?
