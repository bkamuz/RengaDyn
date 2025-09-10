# Baseline2DObject Usage Guide

This guide explains how to use the new `Baseline2DObject` Dynamo nodes to work with 2D baselines in RengaBIM.

## Overview

The `Baseline2DObject` class provides access to the `IBaseline2DObject` interface, which allows you to:

- Get 2D baselines from objects in their own coordinate system
- Get 2D baselines in specified coordinate systems
- Set 2D baselines for objects
- Work with objects that have 2D baseline properties (like beams, columns, etc.)

## Available Dynamo Nodes

### 1. Baseline2DObject.ByModelObject

**Location**: `DynRenga.DynObjects.Baseline2DObject.ByModelObject`

**Description**: Gets a `Baseline2DObject` from a `ModelObject`. Returns `null` if the object doesn't support the `IBaseline2DObject` interface.

**Inputs**:

- `modelObject` (ModelObject): The model object to get the baseline from

**Outputs**:

- `Baseline2DObject` or `null`: The baseline object, or null if not supported

### 2. ModelObject.GetBaseline2DObject

**Location**: `DynRenga.DynObjects.ModelObject.GetBaseline2DObject`

**Description**: Instance method on `ModelObject` to get the baseline interface.

**Inputs**: None (uses the ModelObject instance)

**Outputs**:

- `Baseline2DObject` or `null`: The baseline object, or null if not supported

### 3. Baseline2DObject Methods

Once you have a `Baseline2DObject`, you can use these methods:

#### GetBaseline()

**Description**: Gets the 2D baseline in the object's own coordinate system.

**Inputs**: None

**Outputs**:

- `Curve2D`: The baseline curve

#### GetBaselineInCS(Placement2D)

**Description**: Gets the 2D baseline in a specified coordinate system.

**Inputs**:

- `placement2D` (Placement2D): The target coordinate system

**Outputs**:

- `Curve2D`: The baseline curve in the specified coordinate system

#### SetBaseline(Curve2D)

**Description**: Sets the 2D baseline in the object's own coordinate system.

**Inputs**:

- `baseline` (Curve2D): The new baseline curve

**Outputs**: None

**Note**: Cannot edit baselines of objects with dependent objects (e.g., Roof)

#### SetBaselineInCS(Placement2D, Curve2D)

**Description**: Sets the 2D baseline in a specified coordinate system.

**Inputs**:

- `placement2D` (Placement2D): The target coordinate system
- `baselineInCS` (Curve2D): The new baseline curve in the specified coordinate system

**Outputs**: None

**Note**: Cannot edit baselines of objects with dependent objects (e.g., Roof)

## Example Workflow

Here's a typical workflow for working with 2D baselines:

```
1. Get ModelObject from Selection or Model
   ↓
2. Get Baseline2DObject using ByModelObject or GetBaseline2DObject
   ↓
3. Check if Baseline2DObject is not null
   ↓
4. Use GetBaseline() or GetBaselineInCS() to get the baseline
   ↓
5. Optionally use SetBaseline() or SetBaselineInCS() to modify the baseline
```

## Dynamo Graph Example

```
[Application] → [Project] → [Model] → [GetObjects] → [Filter by Type] → [Baseline2DObject.ByModelObject]
                                                                                    ↓
                                                                              [GetBaseline] → [Curve2D methods]
```

## Supported Object Types

The `IBaseline2DObject` interface is typically available for:

- Beams
- Columns
- Other structural elements with 2D baselines

## Error Handling

- Always check if `Baseline2DObject` is not `null` before using its methods
- Objects without baseline support will return `null`
- Setting baselines on objects with dependent objects will fail

## Related Classes

- `ModelObject`: Base class for all model objects
- `Curve2D`: 2D curve geometry
- `Placement2D`: 2D coordinate system placement
- `Selection`: Methods for selecting model objects

## Notes

- All methods are marked with `[dr.IsVisibleInDynamoLibrary(true)]` for Dynamo visibility
- The implementation follows the existing RengaDyn patterns and conventions
- Russian documentation is provided for consistency with the existing codebase
