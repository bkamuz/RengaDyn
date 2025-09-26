"""
Dynamo Python (IronPython 2.7) script: find_rectangle_neighbors_dynamo.py

Inputs (Dynamo Python node):
    polycurves: list of Dynamo PolyCurve (each rectangle as closed polycurve)
    tol: float tolerance for geometric comparisons (default 1e-6)

Output (Dynamo Python node):
    neighbors_by_index: list of lists, where each inner list contains indices of polycurves
                        that touch the polycurve at one or more side edges.

Notes:
- This script uses only Dynamo geometry objects and basic math; it should run inside
  Dynamo's IronPython 2.7 environment (no external packages required).
- The script assumes rectangles are closed PolyCurves composed of 4 contiguous line segments.
- The neighbor test between two rectangles checks each segment pair for:
    1) collinearity (within tolerance)
    2) 1D overlap of the projected segment intervals (within tolerance)
    3) not just touching at a corner (overlap length > tol) unless you want to include corner-only contacts.

Usage in Dynamo Python node:
- Inputs:
    IN[0] -> polycurves
    IN[1] -> tol
- Output:
    OUT -> neighbors_by_index

Example:
    rectangles = [rectA, rectB, rectC]
    tol = 1e-6
    neighbors_by_index = find_neighbors(rectangles, tol)

Return format example:
    [[1], [0,2], [1]]  # rect 0 neighbors: [1]; rect1 neighbors: [0,2]; rect2: [1]

Author: Generated for user
"""

import math

# Dynamo geometry helpers: to keep compatibility with IronPython without explicit imports
# we will assume the incoming objects behave like Dynamo Geometry PolyCurve and Line objects.

# Helper: compare floats
def is_close(a, b, tol):
    return abs(a - b) <= tol

# Convert a Dynamo line to a tuple of start/end points (each point has X,Y,Z attributes)
def line_endpoints(line):
    # Dynamo Line has .StartPoint and .EndPoint or .Start and .End depending on version
    try:
        a = line.StartPoint
        b = line.EndPoint
    except Exception:
        a = line.Start
        b = line.End
    return a, b

# Vector utilities (points/vectors have .X, .Y, .Z)
def vec_sub(a, b):
    return (a.X - b.X, a.Y - b.Y, a.Z - b.Z)

def dot(u, v):
    return u[0]*v[0] + u[1]*v[1] + u[2]*v[2]

def cross(u, v):
    return (u[1]*v[2] - u[2]*v[1], u[2]*v[0] - u[0]*v[2], u[0]*v[1] - u[1]*v[0])

def length(v):
    return math.sqrt(dot(v, v))

# Check if two 3D vectors are collinear within tol: cross product magnitude small and direction aligned
def are_collinear(u, v, tol):
    c = cross(u, v)
    return length(c) <= tol * max(1.0, length(u), length(v))

# Project a point onto a line defined by origin O and direction D (D does not need to be unit)
# returns scalar parameter t where P = O + t*D
def project_param(O, D, P):
    OP = vec_sub(P, O)
    denom = dot(D, D)
    if denom == 0:
        return 0.0
    return dot(OP, D) / denom

# Check 1D overlap between intervals [a0,a1] and [b0,b1] with tolerance
# Returns overlap length (>= 0)
def interval_overlap(a0, a1, b0, b1, tol):
    if a1 < a0:
        a0, a1 = a1, a0
    if b1 < b0:
        b0, b1 = b1, b0
    lo = max(a0, b0)
    hi = min(a1, b1)
    ov = hi - lo
    if ov <= tol:
        return 0.0
    return ov

# Given two Dynamo Line segments, check if they lie on the same infinite line (collinear) and overlap
# Returns True if they overlap by more than tol (i.e., share a side), False otherwise
def segments_touch_by_side(line1, line2, tol):
    a1, a2 = line_endpoints(line1)
    b1, b2 = line_endpoints(line2)
    u = vec_sub(a2, a1)
    v = vec_sub(b2, b1)

    if length(u) < tol or length(v) < tol:
        return False
# Dynamo / IronPython 2.7 script
# Purpose: Given a list of Dynamo PolyCurves (rectangles), return for each polycurve
# the list of neighbor indices that share an edge with it. Corner-only touches are
# excluded (i.e., touching at a single point does not count as a neighbor).
#
# Inputs:
#   IN[0] -> curves : list of PolyCurve (Dynamo geometry)
#   IN[1] -> tol : float (optional) - distance tolerance for geometric checks (default 1e-6)
#
# Output:
#   OUT -> neighbors_by_index : list of lists of indices
#
# Usage notes:
# - Wire your list of closed PolyCurves (rectangles) into IN[0].
# - Optionally wire a small tolerance (meters or model units) into IN[1].
# - The script will only treat two polycurves as neighbors if they have a
#   non-zero overlapping edge (overlap length > tol). If they only touch at
#   a corner point, they are NOT considered neighbors.
#
# Example (conceptual):
# - Rect A and Rect B share a full side -> A neighbors include B and vice versa.
# - Rect C touches Rect A only at a single corner -> not a neighbor.
# Expected output for [A,B,C] would be something like: [[1],[0],[]]
#
# Concrete conceptual layout (indices):
# 0: Rect A at origin spanning (0,0)-(2,1)
# 1: Rect B to the right of A spanning (2,0)-(4,1)  # shares vertical edge x=2 with A -> edge-touch
# 2: Rect C above A spanning (2,1)-(3,2)            # touches A only at corner (2,1) -> NOT neighbor
#
# Expected neighbors: [[1], [0], []]

import clr
import System
from System import Array

# Dynamo geometry types
clr.AddReference('ProtoGeometry')
from Autodesk.DesignScript.Geometry import Point, Vector, CoordinateSystem

# Helper math
import math

def is_zero(val, tol):
    return abs(val) <= tol

def are_vectors_collinear(v1, v2, tol_angle):
    # Check if v1 and v2 are collinear (parallel or anti-parallel) within tol_angle (radians)
    cross_x = v1.Y * v2.Z - v1.Z * v2.Y
    cross_y = v1.Z * v2.X - v1.X * v2.Z
    cross_z = v1.X * v2.Y - v1.Y * v2.X
    cross_mag = math.sqrt(cross_x*cross_x + cross_y*cross_y + cross_z*cross_z)
    denom = v1.Length * v2.Length
    if is_zero(denom, 1e-12):
        return False
    sin_theta = cross_mag / denom
    theta = math.asin(min(1.0, max(-1.0, sin_theta)))
    return abs(theta) <= tol_angle

def project_point_onto_line_param(pt, line_p0, dir_vec):
    # Parametric projection t such that projection = line_p0 + t * dir_vec
    dx = pt.X - line_p0.X
    dy = pt.Y - line_p0.Y
    dz = pt.Z - line_p0.Z
    dot1 = dx * dir_vec.X + dy * dir_vec.Y + dz * dir_vec.Z
    dot2 = dir_vec.X * dir_vec.X + dir_vec.Y * dir_vec.Y + dir_vec.Z * dir_vec.Z
    if is_zero(dot2, 1e-12):
        return None
    return dot1 / dot2

def perp_distance_point_to_line(pt, line_p0, dir_vec, dir_len):
    # Perpendicular distance from pt to infinite line defined by (line_p0, dir_vec)
    vx = pt.X - line_p0.X
    vy = pt.Y - line_p0.Y
    vz = pt.Z - line_p0.Z
    cross_x = vy * dir_vec.Z - vz * dir_vec.Y
    cross_y = vz * dir_vec.X - vx * dir_vec.Z
    cross_z = vx * dir_vec.Y - vy * dir_vec.X
    cross_mag = math.sqrt(cross_x*cross_x + cross_y*cross_y + cross_z*cross_z)
    if is_zero(dir_len, 1e-12):
        return float('inf')
    return cross_mag / dir_len

# Main algorithm
# Support inputs:
# IN[0] -> listA : list of PolyCurves (rectangles) to find neighbors for
# IN[1] -> listB or tol : if this is a list, it's treated as listB to compare against listA.
#                           if it's a number or None, it's treated as tol and we compare within listA.
# IN[2] -> tol : optional tolerance when IN[1] is listB

# Dynamo provides IN when running inside a Dynamo Python node. Use a safe fallback
# to avoid NameError during static analysis or local runs.
IN = globals().get('IN', [None, None, None])

listA = IN[0] if len(IN) > 0 else None
second = IN[1] if len(IN) > 1 else None
third = IN[2] if len(IN) > 2 else None

# Optional: when run as a normal Python script, allow a tiny demo (no Dynamo geometry)
if __name__ == '__main__':
    print('Running find_rectangle_neighbors_dynamo.py as standalone - no demo available for Dynamo geometry')

# Determine whether second is a list of curves or a numeric tolerance
def is_iterable(obj):
    try:
        iter(obj)
        return True
    except Exception:
        return False

if listA is None:
    OUT = []
else:
    # Decide inputs
    if second is None or (not is_iterable(second)) or isinstance(second, (float, int)):
        # single-list mode: compare within listA
        listB = listA
        tol = float(second) if second is not None else (float(third) if third is not None else 1e-6)
        symmetric = True
    else:
        # two-list mode: compare listA against listB (no symmetry)
        listB = second
        tol = float(third) if third is not None else 1e-6
        symmetric = False

    # Pre-extract segments for each polycurve in both lists
    def extract_segments(curves):
        segs_list = []
        for c in curves:
            try:
                segs = list(c.Curves())
            except Exception:
                segs = [c]
            segs_list.append(segs)
        return segs_list

    segsA = extract_segments(listA)
    segsB = extract_segments(listB)

    nA = len(listA)
    nB = len(listB)

    # Initialize neighbor lists for items in A (indices refer to listB indices)
    neighbors = [[] for _ in range(nA)]

    # Small angle tolerance for collinearity (radians)
    angle_tol = 1e-6

    for i in range(nA):
        segs_i = segsA[i]
        # Depending on symmetric mode we may avoid double-checking pairs
        j_start = 0
        for j in range(j_start, nB):
            # If symmetric and comparing listA to itself, avoid duplicate pairs and self
            if symmetric and j <= i:
                continue

            segs_j = segsB[j]
            touches = False

            # Compare each segment pair
            for si in segs_i:
                a0 = si.StartPoint
                a1 = si.EndPoint
                dir_i = Vector.ByTwoPoints(a0, a1)
                # precompute denom and length
                denom_len2 = dir_i.X*dir_i.X + dir_i.Y*dir_i.Y + dir_i.Z*dir_i.Z
                if is_zero(denom_len2, 1e-12):
                    continue
                dir_len = math.sqrt(denom_len2)

                for sj in segs_j:
                    b0 = sj.StartPoint
                    b1 = sj.EndPoint
                    dir_j = Vector.ByTwoPoints(b0, b1)

                    # Check collinearity of directions
                    if not are_vectors_collinear(dir_i, dir_j, angle_tol):
                        continue

                    # Project endpoints of sj onto line of si (parametric t where a0->0, a1->1)
                    tb0 = project_point_onto_line_param(b0, a0, dir_i)
                    tb1 = project_point_onto_line_param(b1, a0, dir_i)
                    if tb0 is None or tb1 is None:
                        continue

                    bmin = min(tb0, tb1)
                    bmax = max(tb0, tb1)

                    # Overlap between [0,1] and [bmin,bmax] in param units
                    overlap_param = min(1.0, bmax) - max(0.0, bmin)
                    # Convert overlap to absolute length along segment i
                    overlap_length = overlap_param * dir_len

                    # Require a positive overlapping edge longer than tol (not just a point)
                    if overlap_length <= tol:
                        # Too small or zero -> likely corner-only or gap
                        continue

                    # Ensure the other segment's endpoints are close to the line (within tol)
                    pd0 = perp_distance_point_to_line(b0, a0, dir_i, dir_len)
                    pd1 = perp_distance_point_to_line(b1, a0, dir_i, dir_len)
                    if pd0 <= tol + 1e-9 and pd1 <= tol + 1e-9:
                        touches = True
                        break
                if touches:
                    break

            if touches:
                neighbors[i].append(j)
                # If symmetric (same list) also append reverse
                if symmetric:
                    neighbors[j].append(i)

    OUT = neighbors