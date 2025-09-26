split_plane_into_zones_dynamo.py

Usage (Dynamo Python node):
- IN[0]: plane_or_boundary -> a Plane/CoordinateSystem or a closed PolyCurve boundary
- IN[1]: points            -> list of Dynamo Points used as zone centers
- IN[2]: tol               -> optional float tolerance (default 1e-6)

Outputs (OUT):
- OUT[0]: zones -> list of zones, each zone is a list of 4 Points [p00,p10,p11,p01] or [None]*4 for invalid points
- OUT[1]: ux    -> sorted unique u coordinates (plane-local)
- OUT[2]: vy    -> sorted unique v coordinates (plane-local)
- OUT[3]: debug -> dictionary with diagnostics (plane inference, counts, per-point UVs, edges, reasons)

Notes:
- The script now creates exactly one zone per input point (preserving input order). Each input point is projected to the plane and assigned to the nearest unique u/v coordinate determined from the whole point set and boundary endpoints. Zone extents are midpoints between adjacent unique coords; outer zones extend half the outer gaps.
- If a point could not be projected, the corresponding zone will be [None, None, None, None]. Check debug['per_point_uv'] for per-point UV values.
- The script is intended to run inside Dynamo (IronPython) where `ProtoGeometry` is available.

Next steps (optional):
- Convert the 4-point zones into closed PolyCurves or Surfaces for visibility.
- Clip zones by a non-axis-aligned boundary polycurve (keep only zones whose centroids are inside boundary).
- Add unit tests in a small Dynamo graph or Python harness using DynamoServices if desired.
 
Notes about clipped output and geometry constructors:
- The script projects the input boundary to plane-local UV coordinates and uses a centroid-in-polygon test to decide whether a zone is inside the boundary. Only zones whose centroid falls inside are returned as geometry.
- The script attempts to create a closed PolyCurve/Polygon for each kept zone using common Dynamo constructors. If the runtime doesn't expose those constructors (varies by Dynamo / Revit / Civil), the script will fall back to returning the 4 corner Points for each zone.
- OUT[0] therefore contains either PolyCurve/Polygon objects (if created) or lists-of-4 Points. OUT[3]['kept_zone_count'] reports how many geometries were kept/created.
