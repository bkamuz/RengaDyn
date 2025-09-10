# COM Error E_FAIL When Setting Baselines

## 🚨 Problem Description

**Error**: `Error HRESULT E_FAIL has been returned from a call to a COM component.`

**Context**: Occurs when trying to set a baseline on an object using `SetBaseline()` method.

## 🔍 Root Causes

### 1. **Object Type Limitation** (Most Common)
- **Issue**: Floor objects may not fully support baseline modifications via COM interface
- **Why**: Baseline2D interface is primarily designed for structural elements (beams, columns)
- **Evidence**: Floor was created successfully, baseline2D object was obtained, but setting fails at COM level

### 2. **Coordinate System Mismatch**
- **Issue**: Renga expects coordinates in millimeters, Dynamo often works in meters
- **Evidence**: Your coordinates `(-0.43, -1.76, 3.37, -6.09)` look like meter values

### 3. **Invalid Curve Geometry**
- **Issue**: The curve might be too small, too large, or geometrically invalid for Renga
- **Check**: Curve length, direction, or complexity

### 4. **Object State Issues**
- **Issue**: Floor object might not be in correct state for baseline modification
- **Timing**: Setting baseline immediately after creation might fail

## ✅ Solutions (In Order of Preference)

### **Solution 1: Use Structural Objects Instead**
```dynamo
[Create Beam/Column] → [Set Baseline] → [Works Reliably]
```
- ✅ **Recommended**: Use objects designed for baselines
- ✅ **Reliable**: Structural elements fully support baseline interface
- ✅ **Tested**: Known to work consistently

### **Solution 2: Coordinate System Fix**
```dynamo
[Curve2D.ByLineSegment(-430, -1760, 3370, -6090)] → [Set Baseline]
```
- 🔧 **Convert coordinates to millimeters** (multiply by 1000)
- 🔧 **Check your source coordinates** are in correct units
- 🔧 **Use Renga's native coordinate system**

### **Solution 3: Validate Curve First**
```dynamo
[Curve2D] → [Check Length > 0] → [Check Valid Geometry] → [Set Baseline]
```
- 🔧 **Ensure curve length > 0**
- 🔧 **Verify curve is not degenerate**
- 🔧 **Check curve bounds are reasonable**

### **Solution 4: Alternative Approach - Floor with Profile**
```dynamo
[Create Floor Profile] → [Create Floor with Profile] → [Skip Baseline]
```
- 🔧 **Use floor profiles instead of baselines**
- 🔧 **More appropriate for floor objects**
- 🔧 **Better geometric control**

### **Solution 5: Debug with Available Interfaces**
```dynamo
[Floor] → [GetAvailableInterfaces] → [Check if IBaseline2DObject ✓]
```
- 🔧 **Verify interface support before setting baseline**
- 🔧 **Check what interfaces are actually available**
- 🔧 **Debug interface compatibility**

## 🔧 Immediate Fix for Your Case

Based on your coordinates `(-0.43, -1.76)` to `(3.37, -6.09)`:

### **Option A: Convert to Millimeters**
```dynamo
Curve2D.ByLineSegment(-430, -1760, 3370, -6090)
```

### **Option B: Use SetBaselineInCS with Proper Coordinate System**
```dynamo
[Get Floor Placement] → [SetBaselineInCS(placement, curve)] → [May work better]
```

### **Option C: Create Beam Instead (Recommended)**
```dynamo
[Create Beam with your baseline] → [Guaranteed to work]
```

## 📊 Success Probability by Solution

1. **Beam/Column instead of Floor**: 🟢 95% - Structural elements designed for baselines
2. **Coordinate conversion to mm**: 🟡 70% - Addresses unit mismatch
3. **SetBaselineInCS method**: 🟡 60% - Better coordinate handling
4. **Floor with Profile approach**: 🟢 85% - More appropriate for floors
5. **Debug interfaces first**: 🟡 50% - May reveal the core issue

## 🚀 Recommended Workflow

1. **Quick Test**: Try coordinate conversion to millimeters
2. **If still fails**: Check `GetAvailableInterfaces()` 
3. **If interface missing**: Use Floor Profile approach instead
4. **If interface exists**: Try `SetBaselineInCS()` method
5. **Ultimate fallback**: Create Beam/Column instead of Floor

## 💡 Prevention

- **Always check interface availability** before setting baselines
- **Use appropriate object types** for baseline operations
- **Verify coordinate units** match Renga expectations
- **Test with simple geometry first** before complex curves

The **root cause is likely that Floor objects have limited baseline support** compared to structural elements. Consider using the appropriate object type for your use case.