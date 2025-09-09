# Renga Coordinate Systems in Dynamo Guide

## ✅ **Yes! You can use real coordinate systems from Renga in Dynamo**

You already have comprehensive coordinate system support built into your RengaDyn project!

## 🎯 **Available Coordinate System Features**

### **1. Renga to Dynamo Conversion**

#### **2D Coordinate Systems:**

```
[Renga Placement2D] → [Placement2D.ToDynamoCoordinateSystem()] → [Dynamo CoordinateSystem]
```

#### **3D Coordinate Systems:**

```
[Renga Placement3D] → [Placement3D.ToDynamoCoordinateSystem()] → [Dynamo CoordinateSystem]
```

### **2. Enhanced Baseline2DObject with CS Support**

#### **Get Baseline in Specific Coordinate System:**

```
[Baseline2DObject] → [GetBaselineInCSAsDynamo(Placement2D)] → [Dynamo Curve in CS]
```

#### **Get Baseline in Global Coordinate System:**

```
[Baseline2DObject] → [GetBaselineInGlobalCS()] → [Dynamo Curve in Global CS]
```

#### **Transform Baseline to Coordinate System:**

```
[Baseline2DObject] → [TransformBaselineToCoordinateSystem(Placement2D)] → [Dynamo Curve]
```

### **3. Coordinate System Utilities**

#### **Create Coordinate Systems:**

```
[CoordinateSystemUtils] → [CreateWorldCoordinateSystem()] → [Dynamo CoordinateSystem]
[CoordinateSystemUtils] → [CreateCoordinateSystem(Point, Vector, Vector)] → [Dynamo CoordinateSystem]
[CoordinateSystemUtils] → [CreateCoordinateSystem3D(Point, Vector, Vector, Vector)] → [Dynamo CoordinateSystem]
[CoordinateSystemUtils] → [CreateCoordinateSystemByPoints(Point, Point, Point)] → [Dynamo CoordinateSystem]
[CoordinateSystemUtils] → [CreateCoordinateSystem2D(Point, Point)] → [Dynamo CoordinateSystem]
[CoordinateSystemUtils] → [CreateCoordinateSystemByAngle(Point, double)] → [Dynamo CoordinateSystem]
```

#### **Get Coordinate System Information:**

```
[CoordinateSystemUtils] → [GetCoordinateSystemInfo(CoordinateSystem)] → [String with CS info]
```

## 🔧 **Usage Examples**

### **Example 1: Get Baseline in Different Coordinate Systems**

```
[ModelObject] → [GetBaseline2DObject] → [Baseline2DObject]
[Baseline2DObject] → [GetBaselineInGlobalCS()] → [Dynamo Curve in Global CS]
[Baseline2DObject] → [GetBaselineInCSAsDynamo(Placement2D)] → [Dynamo Curve in Specific CS]
```

### **Example 2: Create Custom Coordinate System**

```
[CoordinateSystemUtils] → [CreateCoordinateSystemByAngle(Point, 45.0)] → [Dynamo CoordinateSystem]
[Baseline2DObject] → [TransformBaselineToCoordinateSystem(Placement2D)] → [Dynamo Curve]
```

### **Example 3: Convert Renga Placement to Dynamo**

```
[Renga Placement2D] → [Placement2D.ToDynamoCoordinateSystem()] → [Dynamo CoordinateSystem]
[Renga Placement3D] → [Placement3D.ToDynamoCoordinateSystem()] → [Dynamo CoordinateSystem]
```

## 📋 **Available Methods**

### **Placement2D Methods:**

- `Origin()` - Get origin point
- `AxisX()` - Get X-axis vector
- `AxisY()` - Get Y-axis vector
- `ToDynamoCoordinateSystem()` - Convert to Dynamo CS
- `IsOrthogonal` - Check if orthogonal
- `IsNormal` - Check if normal
- `IsLeft` - Check if left-handed
- `GetTransformFrom` - Get transform from CS to global
- `GetTransformInto` - Get transform from global to CS
- `GetCopy` - Get copy of CS

### **Placement3D Methods:**

- `Origin()` - Get origin point
- `AxisX()` - Get X-axis vector
- `AxisY()` - Get Y-axis vector
- `AxisZ()` - Get Z-axis vector
- `ToDynamoCoordinateSystem()` - Convert to Dynamo CS
- `IsOrthogonal` - Check if orthogonal
- `IsNormal` - Check if normal
- `IsLeft` - Check if left-handed
- `GetTransformFrom` - Get transform from CS to global
- `GetTransformInto` - Get transform from global to CS
- `Move(Vector)` - Move CS by vector
- `Rotate(Vector)` - Rotate CS around vector
- `Transform(Transform3D)` - Apply transformation
- `GetCopy()` - Get copy of CS

### **Baseline2DObject Methods:**

- `GetBaseline()` - Get baseline in object's CS
- `GetBaselineInCS(Placement2D)` - Get baseline in specific CS
- `GetBaselineInCSAsDynamo(Placement2D)` - Get baseline as Dynamo curve in specific CS
- `GetBaselineInGlobalCS()` - Get baseline as Dynamo curve in global CS
- `TransformBaselineToCoordinateSystem(Placement2D)` - Transform baseline to CS
- `GetObjectCoordinateSystem()` - Get object's CS (if available)
- `GetCoordinateSystemInfo()` - Get CS information
- `SetBaseline(Curve2D)` - Set baseline in object's CS
- `SetBaselineInCS(Placement2D, Curve2D)` - Set baseline in specific CS

### **CoordinateSystemUtils Methods:**

- `CreateWorldCoordinateSystem()` - Create world CS
- `CreateCoordinateSystem(Point, Vector, Vector)` - Create 2D CS
- `CreateCoordinateSystem3D(Point, Vector, Vector, Vector)` - Create 3D CS
- `CreateCoordinateSystemByPoints(Point, Point, Point)` - Create CS by 3 points
- `CreateCoordinateSystem2D(Point, Point)` - Create 2D CS by 2 points
- `CreateCoordinateSystemByAngle(Point, double)` - Create CS by angle
- `GetCoordinateSystemInfo(CoordinateSystem)` - Get CS information

## 🎨 **Practical Workflows**

### **Workflow 1: Analyze Object in Different Coordinate Systems**

```
1. [ModelObject] → [GetBaseline2DObject] → [Baseline2DObject]
2. [Baseline2DObject] → [GetCoordinateSystemInfo()] → [String] // Check object CS
3. [CoordinateSystemUtils] → [CreateWorldCoordinateSystem()] → [Dynamo CoordinateSystem]
4. [Baseline2DObject] → [GetBaselineInGlobalCS()] → [Dynamo Curve in Global CS]
5. [Baseline2DObject] → [GetBaselineInCSAsDynamo(Placement2D)] → [Dynamo Curve in Object CS]
```

### **Workflow 2: Create Custom Coordinate System for Analysis**

```
1. [CoordinateSystemUtils] → [CreateCoordinateSystemByAngle(Point, 30.0)] → [Dynamo CoordinateSystem]
2. [Baseline2DObject] → [TransformBaselineToCoordinateSystem(Placement2D)] → [Dynamo Curve]
3. [CoordinateSystemUtils] → [GetCoordinateSystemInfo(CoordinateSystem)] → [String] // Verify CS
```

### **Workflow 3: Convert Renga Placements to Dynamo**

```
1. [Renga Placement2D] → [Placement2D.ToDynamoCoordinateSystem()] → [Dynamo CoordinateSystem]
2. [Baseline2DObject] → [GetBaselineInCSAsDynamo(Placement2D)] → [Dynamo Curve]
3. [Placement2D] → [GetCoordinateSystemInfo()] → [String] // Check CS properties
```

## 🔍 **Coordinate System Properties**

### **Renga Placement Properties:**

- **Origin**: Starting point of the coordinate system
- **AxisX**: X-axis direction vector
- **AxisY**: Y-axis direction vector (2D)
- **AxisZ**: Z-axis direction vector (3D)
- **IsOrthogonal**: Whether axes are perpendicular
- **IsNormal**: Whether axes are unit length
- **IsLeft**: Whether it's a left-handed system

### **Dynamo CoordinateSystem Properties:**

- **Origin**: Starting point
- **XAxis**: X-axis direction vector
- **YAxis**: Y-axis direction vector
- **ZAxis**: Z-axis direction vector

## ⚡ **Performance Tips**

1. **Use object's native CS** when possible (fastest)
2. **Cache coordinate systems** if used multiple times
3. **Use 2D CS** for 2D operations (more efficient)
4. **Check CS properties** before complex operations

## 🎯 **Summary**

You have **full coordinate system support** in your RengaDyn project:

✅ **Renga to Dynamo conversion** - Both 2D and 3D
✅ **Baseline transformation** - Any CS to any CS
✅ **Custom CS creation** - Multiple methods available
✅ **CS analysis tools** - Properties and information
✅ **Type-aware conversion** - Proper curve handling in any CS

Your closed PolyCurve can now be analyzed and transformed in **any coordinate system** you need!
