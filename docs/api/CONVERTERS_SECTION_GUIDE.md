# Converters Section Guide

## Overview

The Converters section provides specialized conversion utilities for transforming Renga geometry objects into Dynamo geometry. This section is organized by source system and geometry type to provide clear, maintainable conversion pathways.

## Structure

```
Converters/
└── Renga/
    └── 2DGeometry/
        └── Curve2DConverter.cs ✅ (Completed)
```

## Purpose

The Converters section addresses the need for seamless integration between Renga BIM SDK geometry and Dynamo geometry systems. It provides:

- **Type-safe conversions** from Renga geometry to Dynamo geometry
- **Handling of complex curve types** (LineSegment, Arc, PolyCurve)
- **Optimized conversion strategies** for different use cases
- **Comprehensive error handling** and debugging support

## Curve2DConverter Features

### Core Conversion Methods

#### 1. **ToDynamoPolyCurveOptimized()** - Primary Method ✅

- **Purpose**: Optimized conversion with special handling for closed curves
- **Handles**: All curve types (LineSegment, Arc, PolyCurve, Undefined)
- **Strategy**: Uses specialized methods for closed PolyCurves and safe conversion for others
- **Benefits**: Better performance for complex closed curves and reliable conversion
- **Parameters**:
  - `curve2D`: DynRenga.DynGeometry.Curve2D to convert
  - `partsPerMeter`: Discretization density (default: 2)

### Specialized Conversion Methods

#### 2. **ToDynamoCurves()** - Individual Segments

- **Purpose**: Converts to list of individual Dynamo curves
- **Use Case**: When you need access to individual curve segments
- **Returns**: List<dg.Curve> containing all curve segments

#### 3. **ToDynamoLine()** - Line Segments Only

- **Purpose**: Converts LineSegment types to Dynamo Line
- **Validation**: Returns null if not a LineSegment type
- **Use Case**: When you know the curve is a simple line

#### 4. **ToDynamoArc()** - Arcs Only

- **Purpose**: Converts Arc types to Dynamo Arc
- **Validation**: Returns null if not an Arc type
- **Use Case**: When you need to preserve arc geometry

### Utility Methods

#### 5. **GetCurveInfo()** - Curve Analysis

- **Purpose**: Provides comprehensive curve information
- **Returns**: Dictionary with curve type, length, parameter range, etc.
- **Multi-Return**: CurveType, IsClosed, Length, ParameterRange, SegmentCount, BoundingBox

#### 6. **GetDebugInfo()** - Debugging Support

- **Purpose**: Provides detailed debug information
- **Includes**: Conversion testing, error reporting, performance metrics
- **Use Case**: Troubleshooting conversion issues

## Design Decisions

### Why PolyCurve as Primary Output?

1. **Unified Handling**: PolyCurve can represent any curve type (line, arc, complex)
2. **Dynamo Compatibility**: PolyCurve is the most flexible curve type in Dynamo
3. **Existing Infrastructure**: Leverages existing conversion methods in Curve2D class
4. **Complex Curve Support**: Handles both simple and complex curve geometries

### Conversion Strategy

1. **Type Detection**: Analyzes Curve2DType to determine conversion approach
2. **Fallback Handling**: Uses safe conversion methods for unknown types
3. **Error Recovery**: Comprehensive exception handling with descriptive messages
4. **Performance Optimization**: Specialized methods for common cases

### Coordinate System Handling

- **Unit Conversion**: Automatically handles Renga (mm) to Dynamo (m) conversion
- **Scale Factor**: Uses existing 1000.0 division factor from Curve2D class
- **Consistency**: Maintains coordinate system consistency across conversions

## Usage Examples

### Basic Conversion

```csharp
// Convert single curve to PolyCurve (recommended method)
var dynamoCurve = Curve2DConverter.ToDynamoPolyCurveOptimized(rengaCurve2D);

// Convert with custom discretization
var highQualityCurve = Curve2DConverter.ToDynamoPolyCurveOptimized(rengaCurve2D, 5);
```

### Specialized Conversions

```csharp
// Convert to individual curves
var curveList = Curve2DConverter.ToDynamoCurves(rengaCurve2D);

// Convert line segment specifically
var line = Curve2DConverter.ToDynamoLine(rengaCurve2D);

// Convert arc specifically
var arc = Curve2DConverter.ToDynamoArc(rengaCurve2D);
```

### Analysis and Debugging

```csharp
// Get curve information
var info = Curve2DConverter.GetCurveInfo(rengaCurve2D);
var curveType = info["CurveType"];
var isClosed = info["IsClosed"];

// Get debug information
var debugInfo = Curve2DConverter.GetDebugInfo(rengaCurve2D);
```

## Error Handling

The converter provides comprehensive error handling:

- **Null Input Validation**: Checks for null inputs with descriptive messages
- **Type Validation**: Validates curve types before specialized conversions
- **Exception Wrapping**: Wraps underlying exceptions with context
- **Debug Information**: Provides detailed error information for troubleshooting

## Performance Considerations

1. **Discretization Control**: `partsPerMeter` parameter controls curve quality vs performance
2. **Optimized Methods**: Specialized methods for common cases (closed curves, line segments)
3. **Lazy Evaluation**: Only converts when needed
4. **Memory Management**: Proper disposal of intermediate objects

## Working Methods Status

### ✅ Working Methods

- **`ToDynamoPolyCurveOptimized()`** - Primary conversion method
- **`ToDynamoCurves()`** - Individual curve segments
- **`ToDynamoLine()`** - Line segment conversion
- **`ToDynamoArc()`** - Arc conversion
- **`GetCurveInfo()`** - Curve analysis
- **`GetDebugInfo()`** - Debugging support

### ❌ Removed Methods (Not Working)

- **`ToDynamoPolyCurve()`** - Removed due to compatibility issues
- **`ToDynamoPolyCurves()`** - Removed due to dependency on non-working method
- **`ToDynamoPolyCurveCombined()`** - Removed due to dependency on non-working method

## Future Extensions

The Converters section is designed for easy extension:

1. **3D Geometry Converters**: Future addition of 3D curve and surface converters
2. **Other Systems**: Support for other CAD/BIM systems
3. **Bidirectional Conversion**: Conversion from Dynamo back to Renga
4. **Advanced Features**: Custom conversion strategies, filtering, etc.

## Integration with Existing Code

The converter leverages existing infrastructure:

- **Curve2D Class Methods**: Uses existing `ToDynamoPolyCurveSafe()` and related methods
- **Error Handling Patterns**: Follows established error handling patterns
- **Dynamo Integration**: Uses standard Dynamo geometry types
- **Namespace Organization**: Follows established namespace conventions

This design ensures consistency with the existing codebase while providing a clean, specialized interface for curve conversion operations.
