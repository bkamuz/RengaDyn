# Curve2D PolyCurve Conversion Guide

This guide explains how to convert `Curve2DType_PolyCurve` to Dynamo `PolyCurve` using the new RengaDyn nodes.

## Available Dynamo Nodes

### 1. Curve2D.ToDynamoPolyCurve (Static Method)

**Location**: `DynRenga.DynGeometry.Curve2D.ToDynamoPolyCurve`

**Description**: Converts any `Curve2D` to Dynamo `PolyCurve` using the general method.

**Inputs**:

- `curve2D` (Curve2D): The 2D curve to convert
- `parts_in_meter` (int, optional): Number of segments per meter (default: 2)

**Outputs**:

- `PolyCurve`: Dynamo PolyCurve or null if conversion fails

### 2. Curve2D.ToDynamoPolyCurveFromSegments (Static Method)

**Location**: `DynRenga.DynGeometry.Curve2D.ToDynamoPolyCurveFromSegments`

**Description**: Converts `Curve2D` to Dynamo `PolyCurve` using segment-based approach (optimized for PolyCurve types).

**Inputs**:

- `curve2D` (Curve2D): The 2D curve to convert
- `parts_in_meter` (int, optional): Number of segments per meter (default: 2)

**Outputs**:

- `PolyCurve`: Dynamo PolyCurve or null if conversion fails

### 3. Curve2D Instance Methods

#### ToDynamoPolyCurve()

**Description**: Instance method to convert the curve to Dynamo PolyCurve.

**Inputs**:

- `parts_in_meter` (int, optional): Number of segments per meter (default: 2)

**Outputs**:

- `PolyCurve`: Dynamo PolyCurve

#### IsPolyCurve()

**Description**: Checks if the curve is a PolyCurve type.

**Inputs**: None

**Outputs**:

- `bool`: True if it's a PolyCurve, False otherwise

#### GetCurveTypeString()

**Description**: Gets the curve type as a string.

**Inputs**: None

**Outputs**:

- `string`: Curve type string (e.g., "Curve2DType_PolyCurve")

#### GetSegmentCount()

**Description**: Gets the number of segments in a PolyCurve (only for PolyCurve types).

**Inputs**: None

**Outputs**:

- `int`: Number of segments or -1 if not a PolyCurve

#### GetSegment(int index)

**Description**: Gets a specific segment from a PolyCurve by index.

**Inputs**:

- `index` (int): Segment index (0-based)

**Outputs**:

- `Curve2D`: The segment curve or null if invalid index

## Usage Examples

### Basic Conversion

```
[Curve2D] → [ToDynamoPolyCurve] → [PolyCurve]
```

### Static Method Conversion

```
[Curve2D] → [Curve2D.ToDynamoPolyCurve] → [PolyCurve]
```

### Segment-Based Conversion (Recommended for PolyCurve)

```
[Curve2D] → [Curve2D.ToDynamoPolyCurveFromSegments] → [PolyCurve]
```

### Check Curve Type First

```
[Curve2D] → [IsPolyCurve] → [If True: ToDynamoPolyCurveFromSegments]
```

### Get Segment Information

```
[Curve2D] → [GetSegmentCount] → [Number]
[Curve2D] → [GetSegment] → [Curve2D] (for each segment)
```

## Complete Workflow Example

Here's a complete workflow for working with PolyCurve baselines:

```
1. [ModelObject] → [GetBaseline2DObject] → [Baseline2DObject]
2. [Baseline2DObject] → [GetBaseline] → [Curve2D]
3. [Curve2D] → [IsPolyCurve] → [Boolean]
4. [If True: Curve2D] → [ToDynamoPolyCurveFromSegments] → [PolyCurve]
5. [If False: Curve2D] → [ToDynamoPolyCurve] → [PolyCurve]
```

## Advanced Usage

### Working with Segments

```
[Curve2D] → [GetSegmentCount] → [Number]
[For each segment: Curve2D] → [GetSegment] → [Curve2D] → [ToDynamoPolyCurve] → [PolyCurve]
[All PolyCurves] → [PolyCurve.ByJoinedCurves] → [Final PolyCurve]
```

### Custom Resolution

```
[Curve2D] → [ToDynamoPolyCurveFromSegments] → [PolyCurve] (with custom parts_in_meter)
```

## Method Comparison

| Method | Best For | Performance | Accuracy |
|--------|----------|-------------|----------|
| `ToDynamoPolyCurve()` | All curve types | Good | Good |
| `ToDynamoPolyCurveFromSegments()` | PolyCurve types | Better | Better |
| Static methods | Easy access | Same as instance | Same as instance |

## Error Handling

- All methods return `null` if the input is `null`
- Methods handle COM interop errors gracefully
- Segment-based methods fall back to general methods if segment access fails
- Invalid segment indices return `null`

## Performance Tips

1. **Use `ToDynamoPolyCurveFromSegments()` for PolyCurve types** - More accurate and efficient
2. **Adjust `parts_in_meter`** - Higher values = more accurate but slower
3. **Check curve type first** - Use `IsPolyCurve()` to choose the right method
4. **Use segments for complex PolyCurves** - Better handling of complex geometry

## Troubleshooting

### Issue: Getting null from conversion

**Solution**: Check if the Curve2D is valid and not null

### Issue: Poor quality conversion

**Solution**: Increase `parts_in_meter` parameter

### Issue: Slow conversion

**Solution**: Decrease `parts_in_meter` parameter or use segment-based method

### Issue: Missing segments

**Solution**: Use `GetSegmentCount()` to verify the curve has segments

## Related Classes

- `Curve2D`: Base 2D curve class
- `Baseline2DObject`: For getting baselines from model objects
- `ModelObject`: For accessing model objects
- `Placement2D`: For coordinate system transformations

## Notes

- All methods are marked with `[dr.IsVisibleInDynamoLibrary(true)]` for Dynamo visibility
- The implementation follows existing RengaDyn patterns
- Russian documentation is provided for consistency
- Methods handle both direct casting and `GetInterfaceByName` approaches
