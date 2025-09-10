# Troubleshooting Baseline2DObject Issues

## Problem: Getting null from Baseline2DObject.ByModelObject()

If you're getting `null` from `Baseline2DObject.ByModelObject()` even though you're sure the object has a baseline, here are the steps to troubleshoot:

### Step 1: Check Available Interfaces

Use the new debug method to see which interfaces are available on your object:

```
[ModelObject] → [GetAvailableInterfaces] → [List]
```

This will show you which interfaces the object supports:

- `IBaseline2DObject ✓` - Object supports baseline interface
- `IBaseline2DObject ✗` - Object does NOT support baseline interface
- `IObjectWithMaterial ✓` - Object has material
- etc.

### Step 2: Check Object Type

Verify that your object is actually a type that supports baselines:

```
[ModelObject] → [ObjectType] → [Guid]
```

Common object types that support `IBaseline2DObject`:

- Beams
- Columns
- Structural elements with 2D baselines

### Step 3: Verify Object is Valid

Make sure your ModelObject is not null and properly initialized:

```
[ModelObject] → [Is Null?] → [If False: continue]
```

### Step 4: Check Object Name and ID

Get basic object information:

```
[ModelObject] → [Name] → [String]
[ModelObject] → [Id] → [Number]
```

### Step 5: Try Alternative Methods

The updated `GetBaseline2DObject()` method now tries multiple approaches:

1. **Direct Casting**: `this._i as Renga.IBaseline2DObject`
2. **GetInterfaceByName**: `this._i.GetInterfaceByName("IBaseline2DObject")`

If both fail, the object truly doesn't support the interface.

## Common Issues and Solutions

### Issue 1: Object Type Not Supported

**Problem**: The object type doesn't support `IBaseline2DObject`
**Solution**: Check if your object is actually a beam, column, or other structural element

### Issue 2: Object Not Properly Loaded

**Problem**: The ModelObject is not properly initialized
**Solution**: Ensure you're getting the ModelObject from a valid source (Selection, Model, etc.)

### Issue 3: Interface Not Available

**Problem**: The object doesn't expose the `IBaseline2DObject` interface
**Solution**: This might be a limitation of the Renga version or object type

### Issue 4: COM Interop Issues

**Problem**: COM interface access problems
**Solution**: The updated code handles this with try-catch blocks

## Debug Workflow

Here's a complete debug workflow you can use in Dynamo:

```
1. [Application] → [Project] → [Model] → [GetObjects]
2. [Filter by Object Type] (optional)
3. [GetAvailableInterfaces] → [List]
4. [Filter by "IBaseline2DObject ✓"]
5. [Baseline2DObject.ByModelObject] → [Baseline2DObject]
6. [GetBaseline] → [Curve2D]
```

## Updated Methods

### ModelObject.GetBaseline2DObject()

Now includes:

- Direct casting attempt
- GetInterfaceByName fallback
- Error handling with try-catch
- Returns null if interface not available

### ModelObject.GetAvailableInterfaces()

New debug method that shows:

- Which interfaces are available
- Visual indicators (✓/✗)
- Common interface names

## Expected Behavior

- **If object supports baseline**: Returns `Baseline2DObject` instance
- **If object doesn't support baseline**: Returns `null`
- **If object is invalid**: Returns `null`
- **If COM error occurs**: Returns `null` (with error handling)

## Next Steps

If you're still getting null after following these steps:

1. Check the Renga version compatibility
2. Verify the object type in Renga itself
3. Try with different objects to see if the pattern is consistent
4. Check if the object has a baseline in the Renga interface directly

The updated implementation should now work with more object types and provide better error handling.
