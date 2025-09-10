# Curve2D PolyCurve Conversion Workflow

## Conversion Methods

### 1. Basic Conversion
```
[Curve2D] → [ToDynamoPolyCurve] → [Dynamo PolyCurve]
```

### 2. Static Method Conversion
```
[Curve2D] → [Curve2D.ToDynamoPolyCurve] → [Dynamo PolyCurve]
```

### 3. Segment-Based Conversion (RECOMMENDED)
```
[Curve2D] → [Curve2D.ToDynamoPolyCurveFromSegments] → [Dynamo PolyCurve]
```

### 4. Type-Aware Conversion
```
[Curve2D] → [IsPolyCurve] → [If True: ToDynamoPolyCurveFromSegments]
↓
[If False: ToDynamoPolyCurve]
```

### 5. Complete Baseline Workflow
```
[ModelObject] → [GetBaseline2DObject] → [Baseline2DObject]
↓
[Baseline2DObject] → [GetBaseline] → [Curve2D]
↓
[Curve2D] → [IsPolyCurve] → [Boolean]
↓
[If True: Curve2D] → [ToDynamoPolyCurveFromSegments] → [Dynamo PolyCurve]
↓
[If False: Curve2D] → [ToDynamoPolyCurve] → [Dynamo PolyCurve]
```

### 6. Segment Analysis Workflow
```
[Curve2D] → [GetSegmentCount] → [Number]
↓
[For each segment: Curve2D] → [GetSegment] → [Curve2D]
↓
[Each Curve2D] → [ToDynamoPolyCurve] → [Dynamo PolyCurve]
↓
[All PolyCurves] → [PolyCurve.ByJoinedCurves] → [Final PolyCurve]
```

## Available Dynamo Nodes

### DynRenga.DynGeometry.Curve2D
- **ToDynamoPolyCurve(Curve2D, int) → PolyCurve** (Static)
- **ToDynamoPolyCurveFromSegments(Curve2D, int) → PolyCurve** (Static)
- **ToDynamoPolyCurve(int) → PolyCurve** (Instance)
- **IsPolyCurve() → bool** (Instance)
- **GetCurveTypeString() → string** (Instance)
- **GetSegmentCount() → int** (Instance)
- **GetSegment(int) → Curve2D** (Instance)

## Parameters

- **parts_in_meter**: Number of segments per meter (default: 2)
- **index**: Segment index for GetSegment (0-based)

## Error Handling

- Returns null for invalid inputs
- Handles COM interop errors gracefully
- Falls back to general method if segment access fails