"""
Dynamo Python (IronPython 2.7) script: split_plane_into_zones_dynamo.py

Inputs (Dynamo Python node):
    IN[0] -> plane_or_boundary : either a Dynamo Plane (CoordinateSystem/Plane) or a closed PolyCurve
    IN[1] -> points : list of Dynamo Points to use as division coordinates (will be projected to the plane)
    IN[2] -> tol : float tolerance for coordinate deduplication (optional, default 1e-6)
    IN[3] -> debug_out_path : optional path or folder to write debug JSON
    IN[4] -> mode : 'clip' (default) uses half-plane clipping; 'fortune' uses Bowyer-Watson Delaunay->Voronoi

Outputs (Dynamo Python node):
    OUT -> zones : list of zones; each zone is a list of 4 Dynamo Point corner objects (ordered)
    OUT[1] -> x_coords : sorted unique X coordinates used for tessellation (in plane param units)
    OUT[2] -> y_coords : sorted unique Y coordinates used for tessellation (in plane param units)

Behavior:
    - Projects input 'points' onto the provided plane (or plane inferred from boundary).
    - Builds a set of unique X and Y coordinates (in the plane's local coordinate system) from
      the projected points and the boundary extents (if a PolyCurve boundary is provided).
    - Creates orthogonal rectangular zones between adjacent X and Y coordinates and returns
      the corner points for each zone in 3D world coordinates (as Dynamo Points).

Notes:
    - This script is written to be tolerant to variations in Dynamo/Python API names. It avoids
      heavy use of geometry constructors beyond Point.ByCoordinates to maximize compatibility.
    - If a Plane is provided (no boundary), the extents are taken from the points' convex extents.

Author: Generated for user
"""

import math
import clr
import os
try:
    import json
except Exception:
    json = None

# Dynamo geometry
clr.AddReference('ProtoGeometry')
from Autodesk.DesignScript.Geometry import Point

# Safe access helpers
def has_attr(o, name):
    try:
        getattr(o, name)
        return True
    except Exception:
        return False

def get_point_xyz(pt):
    # Dynamo Point-like objects: .X,.Y,.Z
    return float(pt.X), float(pt.Y), float(pt.Z)

def vec_sub(a, b):
    return (a[0]-b[0], a[1]-b[1], a[2]-b[2])

def vec_length(v):
    return math.sqrt(v[0]*v[0] + v[1]*v[1] + v[2]*v[2])

def vec_dot(a, b):
    return a[0]*b[0] + a[1]*b[1] + a[2]*b[2]

def vec_scale(v, s):
    return (v[0]*s, v[1]*s, v[2]*s)

def vec_add(a, b):
    return (a[0]+b[0], a[1]+b[1], a[2]+b[2])

def normalize(v):
    L = vec_length(v)
    if L == 0:
        return (0.0, 0.0, 0.0)
    return (v[0]/L, v[1]/L, v[2]/L)

# Try to obtain a plane description (origin, xaxis, yaxis) from many possible input types
def plane_from_input(obj, fallback_points=None):
    # If obj looks like a Plane/CoordinateSystem
    if obj is None:
        return None

    # Many Dynamo plane-like objects expose Origin and XAxis/YAxis or Origin and Normal
    if has_attr(obj, 'Origin'):
        try:
            origin = obj.Origin
            # X axis name varies: XAxis, XVector, X, XVector
            xaxis = None
            yaxis = None
            for name in ('XAxis', 'XVector', 'X', 'XDir'):
                if has_attr(obj, name):
                    xaxis = getattr(obj, name)
                    break
            for name in ('YAxis', 'YVector', 'Y', 'YDir'):
                if has_attr(obj, name):
                    yaxis = getattr(obj, name)
                    break

            if xaxis is not None and yaxis is not None:
                ox = float(origin.X); oy = float(origin.Y); oz = float(origin.Z)
                xx = float(xaxis.X); xy = float(xaxis.Y); xz = float(xaxis.Z)
                yx = float(yaxis.X); yy = float(yaxis.Y); yz = float(yaxis.Z)
                return ( (ox,oy,oz), (xx,xy,xz), (yx,yy,yz) )
        except Exception:
            pass

    # If obj is a PolyCurve/Curve boundary, infer a best-fit plane from its points
    if has_attr(obj, 'Curves') or has_attr(obj, 'Edges') or has_attr(obj, 'Points'):
        pts = []
        try:
            curves = list(obj.Curves())
        except Exception:
            try:
                curves = list(obj.Edges)
            except Exception:
                curves = []

        for c in curves:
            try:
                p0 = c.StartPoint
                p1 = c.EndPoint
            except Exception:
                try:
                    p0 = c.Start
                    p1 = c.End
                except Exception:
                    continue
            pts.append(get_point_xyz(p0))
            pts.append(get_point_xyz(p1))

        # fallback to provided points if nothing gathered
        if not pts and fallback_points:
            for p in fallback_points:
                pts.append(get_point_xyz(p))

        pts_unique = []
        seen = set()
        for p in pts:
            key = (round(p[0],6), round(p[1],6), round(p[2],6))
            if key in seen:
                continue
            seen.add(key)
            pts_unique.append(p)

        if len(pts_unique) >= 3:
            p0 = pts_unique[0]
            # find a non-collinear second and third
            for i in range(1, len(pts_unique)):
                v1 = vec_sub(pts_unique[i], p0)
                if vec_length(v1) < 1e-9:
                    continue
                for j in range(i+1, len(pts_unique)):
                    v2 = vec_sub(pts_unique[j], p0)
                    # cross product
                    cx = v1[1]*v2[2] - v1[2]*v2[1]
                    cy = v1[2]*v2[0] - v1[0]*v2[2]
                    cz = v1[0]*v2[1] - v1[1]*v2[0]
                    if vec_length((cx,cy,cz)) < 1e-9:
                        continue
                    # success: build orthonormal axes
                    xaxis = normalize(v1)
                    normal = normalize((cx,cy,cz))
                    # ensure yaxis = normal x xaxis
                    yaxis = ( normal[1]*xaxis[2] - normal[2]*xaxis[1],
                              normal[2]*xaxis[0] - normal[0]*xaxis[2],
                              normal[0]*xaxis[1] - normal[1]*xaxis[0] )
                    return (p0, xaxis, yaxis)

    return None

# Project world point into plane parameters (u,v) where world = origin + u*xaxis + v*yaxis
def project_to_plane_params(world_pt, origin, xaxis, yaxis):
    w = get_point_xyz(world_pt)
    rel = vec_sub(w, origin)
    u = vec_dot(rel, xaxis) / (vec_dot(xaxis, xaxis) if vec_dot(xaxis, xaxis) != 0 else 1.0)
    v = vec_dot(rel, yaxis) / (vec_dot(yaxis, yaxis) if vec_dot(yaxis, yaxis) != 0 else 1.0)
    return u, v

# Create a Dynamo Point from origin + u*xaxis + v*yaxis
def world_point_from_params(origin, xaxis, yaxis, u, v):
    w = vec_add(origin, vec_add(vec_scale(xaxis, u), vec_scale(yaxis, v)))
    return Point.ByCoordinates(w[0], w[1], w[2])

# Deduplicate floats within tol and return sorted list
def unique_sorted(vals, tol):
    vals_sorted = sorted(vals)
    out = []
    for v in vals_sorted:
        if not out:
            out.append(v)
            continue
        if abs(v - out[-1]) > tol:
            out.append(v)
    return out

# Main entry: read IN[] (Dynamo node)
IN = globals().get('IN', [None, None, None])
plane_or_boundary = IN[0] if len(IN) > 0 else None
points = IN[1] if len(IN) > 1 else []
tol = float(IN[2]) if len(IN) > 2 and IN[2] is not None else 1e-6
# Optional debug output path (string) - IN[3]
debug_out_path = IN[3] if len(IN) > 3 and IN[3] is not None else None
# Optional mode selection: 'clip' (default) uses half-plane clipping; 'fortune' uses Bowyer-Watson Delaunay -> Voronoi
# IN[4]
mode = IN[4] if len(IN) > 4 and IN[4] is not None else 'clip'
# Debug dictionary to return diagnostic information
debug = {}

if plane_or_boundary is None:
    debug['reason'] = 'no plane_or_boundary provided'
    debug['points_provided'] = 0
    OUT = ([], [], [], debug)
else:
    # Ensure points is a flat list
    pts_list = []
    try:
        for p in points:
            pts_list.append(p)
    except Exception:
        # single point
        if points is not None:
            pts_list = [points]
    debug['points_provided'] = len(pts_list)

    # Attempt to get a plane representation
    plane = plane_from_input(plane_or_boundary, fallback_points=pts_list)
    if plane is None:
        # As a last resort, if input is explicitly a Plane-like object with Origin property
        try:
            origin = plane_or_boundary.Origin
            xaxis = plane_or_boundary.XAxis
            yaxis = plane_or_boundary.YAxis
            plane = (get_point_xyz(origin), get_point_xyz(xaxis), get_point_xyz(yaxis))
            debug['plane_inferred_from_explicit_properties'] = True
        except Exception:
            debug['plane_inferred'] = False
            debug['reason'] = 'cannot infer plane from input'
            OUT = ([], [], [], debug)
            # cannot infer plane
    if plane is None:
        # already set OUT above if failed
        pass
    else:
        origin, xaxis, yaxis = plane
        debug['plane_inferred'] = True
        # record selected algorithm mode
        debug['mode'] = mode
        try:
            debug['origin'] = (float(origin[0]), float(origin[1]), float(origin[2]))
        except Exception:
            # origin might be a Point object tuple already
            try:
                debug['origin'] = get_point_xyz(origin)
            except Exception:
                debug['origin'] = None

        # Gather projected coordinates
        u_vals = []
        v_vals = []

        # If the boundary is a polycurve, include its endpoints projected to ensure full coverage
        try:
            curves = list(plane_or_boundary.Curves())
        except Exception:
            curves = []
        debug['boundary_curve_count'] = len(curves)

        for c in curves:
            try:
                p0 = c.StartPoint
                p1 = c.EndPoint
            except Exception:
                try:
                    p0 = c.Start
                    p1 = c.End
                except Exception:
                    continue
            u0, v0 = project_to_plane_params(p0, origin, xaxis, yaxis)
            u1, v1 = project_to_plane_params(p1, origin, xaxis, yaxis)
            u_vals.extend([u0, u1])
            v_vals.extend([v0, v1])

        # include projected points and record per-point params (preserve order)
        u_pts = []
        v_pts = []
        for p in pts_list:
            try:
                u, v = project_to_plane_params(p, origin, xaxis, yaxis)
                u_vals.append(u)
                v_vals.append(v)
                u_pts.append(u)
                v_pts.append(v)
            except Exception:
                # keep placeholder None for alignment
                u_pts.append(None)
                v_pts.append(None)

        debug['u_vals_raw_count'] = len(u_vals)
        debug['v_vals_raw_count'] = len(v_vals)
        debug['per_point_uv'] = list(zip(u_pts, v_pts))

        # If still no extents (e.g., plane with no points), return empty
        if not u_vals or not v_vals:
            debug['reason'] = 'no projected extents (u_vals or v_vals empty)'
            debug['u_vals'] = u_vals
            debug['v_vals'] = v_vals
            OUT = ([], [], [], debug)
        else:
            # include bounds endpoints as min/max to ensure outer zones
            u_min = min(u_vals)
            u_max = max(u_vals)
            v_min = min(v_vals)
            v_max = max(v_vals)
            # ensure boundary edges included
            u_vals.extend([u_min, u_max])
            v_vals.extend([v_min, v_max])

            # deduplicate coordinates within tol (for debug and edge calculations)
            ux = unique_sorted(u_vals, tol)
            vy = unique_sorted(v_vals, tol)
            debug['ux'] = ux
            debug['vy'] = vy

            # Helper to compute left/right edges for each sorted coord
            def compute_edges(sorted_vals):
                n = len(sorted_vals)
                lefts = [None]*n
                rights = [None]*n
                if n == 1:
                    span = 1.0
                    lefts[0] = sorted_vals[0] - span/2.0
                    rights[0] = sorted_vals[0] + span/2.0
                    return lefts, rights
                for k in range(n):
                    if k == 0:
                        gap = sorted_vals[1] - sorted_vals[0]
                        left = sorted_vals[0] - gap/2.0
                    else:
                        left = (sorted_vals[k-1] + sorted_vals[k]) / 2.0

                    if k == n-1:
                        gap = sorted_vals[n-1] - sorted_vals[n-2]
                        right = sorted_vals[n-1] + gap/2.0
                    else:
                        right = (sorted_vals[k] + sorted_vals[k+1]) / 2.0

                    lefts[k] = left
                    rights[k] = right
                return lefts, rights

            u_lefts, u_rights = compute_edges(ux)
            v_lefts, v_rights = compute_edges(vy)
            debug['u_lefts'] = u_lefts
            debug['u_rights'] = u_rights
            debug['v_lefts'] = v_lefts
            debug['v_rights'] = v_rights

            # Allow an axis-aligned rectangular grid mode: build rectangles between
            # adjacent unique u/x (ux) and v/y (vy) coordinates. This produces
            # zones whose edges are exactly horizontal or vertical in the plane's
            # parameter space. Enable by setting mode to 'axis', 'grid', or
            # 'axis_aligned'. When active, skip the fortune/clip Voronoi code below.
            axis_mode = False
            if mode in ('axis', 'grid', 'axis_aligned'):
                axis_mode = True
                voronoi_polygons_uv = []
                # iterate columns (ux) then rows (vy) to produce rectangles
                for i in range(len(ux)-1):
                    for j in range(len(vy)-1):
                        poly = [ (ux[i],   vy[j]),
                                 (ux[i+1], vy[j]),
                                 (ux[i+1], vy[j+1]),
                                 (ux[i],   vy[j+1]) ]
                        voronoi_polygons_uv.append(poly)

            # Voronoi-like cell construction by intersecting half-planes
            # For each input point p_i compute its cell as intersection of the boundary polygon
            # and half-planes H_ij = { x | dist(x,p_i) <= dist(x,p_j) } for all j != i.

            def clip_polygon_by_halfplane(poly, a, b, c, tol):
                # poly: list of (x,y) points (ordered).
                # For Voronoi cells we want to keep points x where dist(x,pi) <= dist(x,pj).
                # This simplifies to: x·(pi - pj) >= (pi·pi - pj·pj)/2.
                # Therefore keep points where a*x + b*y >= c (with numerical tol).
                if not poly:
                    return []
                out = []
                n = len(poly)
                for i in range(n):
                    sx, sy = poly[i]
                    ex, ey = poly[(i+1) % n]
                    fs = a*sx + b*sy - c
                    fe = a*ex + b*ey - c
                    # inside when value >= -tol
                    inside_s = fs >= -tol
                    inside_e = fe >= -tol
                    if inside_s and inside_e:
                        # keep end
                        out.append((ex, ey))
                    elif inside_s and not inside_e:
                        # leaving: add intersection
                        denom = a*(ex - sx) + b*(ey - sy)
                        if abs(denom) > 1e-12:
                            t = (c - a*sx - b*sy) / denom
                            ix = sx + t*(ex - sx)
                            iy = sy + t*(ey - sy)
                            out.append((ix, iy))
                    elif (not inside_s) and inside_e:
                        # entering: add intersection then end
                        denom = a*(ex - sx) + b*(ey - sy)
                        if abs(denom) > 1e-12:
                            t = (c - a*sx - b*sy) / denom
                            ix = sx + t*(ex - sx)
                            iy = sy + t*(ey - sy)
                            out.append((ix, iy))
                        out.append((ex, ey))
                    else:
                        # both outside: nothing
                        pass
                return out

            # Clip polygon by rectangular (L-infinity) distance comparison between two sites
            def rect_dist(pt, site):
                # pt: (x,y), site: (sx,sy)
                return max(abs(pt[0]-site[0]), abs(pt[1]-site[1]))

            def clip_polygon_by_rectmetric(poly, site_i, site_j, tol):
                # poly: list of (x,y) points ordered. Keep points where L_inf(pt,site_i) <= L_inf(pt,site_j)
                if not poly:
                    return []
                out = []

                def h_at(t, sx, sy, ex, ey, si, sj):
                    x = sx + t*(ex - sx)
                    y = sy + t*(ey - sy)
                    return max(abs(x - si[0]), abs(y - si[1])) - max(abs(x - sj[0]), abs(y - sj[1]))

                def find_root_on_edge(sx, sy, ex, ey, si, sj):
                    fs = h_at(0.0, sx, sy, ex, ey, si, sj)
                    fe = h_at(1.0, sx, sy, ex, ey, si, sj)
                    if abs(fs) <= tol:
                        return (sx, sy)
                    if abs(fe) <= tol:
                        return (ex, ey)
                    if fs*fe > 0:
                        return None
                    a = 0.0; b = 1.0
                    fa = fs; fb = fe
                    for _ in range(50):
                        m = 0.5*(a+b)
                        fm = h_at(m, sx, sy, ex, ey, si, sj)
                        if abs(fm) <= tol:
                            return (sx + m*(ex-sx), sy + m*(ey-sy))
                        # bisection
                        if fa*fm <= 0:
                            b = m; fb = fm
                        else:
                            a = m; fa = fm
                    # final approx
                    m = 0.5*(a+b)
                    return (sx + m*(ex-sx), sy + m*(ey-sy))

                n = len(poly)
                for i in range(n):
                    sx, sy = poly[i]
                    ex, ey = poly[(i+1) % n]
                    fs = rect_dist((sx, sy), site_i) - rect_dist((sx, sy), site_j)
                    fe = rect_dist((ex, ey), site_i) - rect_dist((ex, ey), site_j)
                    inside_s = fs <= tol
                    inside_e = fe <= tol
                    if inside_s and inside_e:
                        out.append((ex, ey))
                    elif inside_s and not inside_e:
                        root = find_root_on_edge(sx, sy, ex, ey, site_i, site_j)
                        if root is not None:
                            out.append(root)
                    elif (not inside_s) and inside_e:
                        root = find_root_on_edge(sx, sy, ex, ey, site_i, site_j)
                        if root is not None:
                            out.append(root)
                        out.append((ex, ey))
                    else:
                        pass
                return out

            # build boundary polygon in UV coords
            boundary_uv = []
            try:
                bpts = []
                seenb = set()
                for c in curves:
                    try:
                        bp = c.StartPoint
                    except Exception:
                        try:
                            bp = c.Start
                        except Exception:
                            continue
                    bu, bv = project_to_plane_params(bp, origin, xaxis, yaxis)
                    key = (round(bu,6), round(bv,6))
                    if key in seenb:
                        continue
                    seenb.add(key)
                    bpts.append((bu, bv))
                if len(bpts) >= 3:
                    boundary_uv = bpts
            except Exception:
                boundary_uv = []

            voronoi_polygons_uv = []
            if mode == 'fortune':
                # Implement a simple Bowyer-Watson Delaunay triangulation on UV points
                # then derive Voronoi vertices as circumcenters and build polygons per input point.
                # This is an incremental, not Fortune's sweep, but provides similar Voronoi cells.
                try:
                    # collect valid points
                    uv_points = []
                    uv_index_map = []
                    for idx, (ui, vi) in enumerate(zip(u_pts, v_pts)):
                        if ui is None or vi is None:
                            continue
                        uv_points.append((ui, vi))
                        uv_index_map.append(idx)

                    # quick degenerate check
                    if len(uv_points) < 2:
                        voronoi_polygons_uv = [[] for _ in u_pts]
                    else:
                        # Bowyer-Watson Delaunay
                        # helper functions
                        def circumcenter(a, b, c):
                            (ax, ay), (bx, by), (cx, cy) = a, b, c
                            d = 2*(ax*(by-cy) + bx*(cy-ay) + cx*(ay-by))
                            if abs(d) < 1e-12:
                                return None
                            ux = ((ax*ax+ay*ay)*(by-cy) + (bx*bx+by*by)*(cy-ay) + (cx*cx+cy*cy)*(ay-by)) / d
                            uy = ((ax*ax+ay*ay)*(cx-bx) + (bx*bx+by*by)*(ax-cx) + (cx*cx+cy*cy)*(bx-ax)) / d
                            return (ux, uy)

                        def in_circumcircle(pt, tri):
                            a,b,c = tri
                            cc = circumcenter(a,b,c)
                            if cc is None:
                                return False
                            dx = pt[0]-cc[0]
                            dy = pt[1]-cc[1]
                            r2 = (a[0]-cc[0])**2 + (a[1]-cc[1])**2
                            return dx*dx + dy*dy <= r2 + tol

                        # super-triangle: make a big triangle that contains all points
                        ux_vals = [p[0] for p in uv_points]
                        vy_vals = [p[1] for p in uv_points]
                        minx, maxx = min(ux_vals), max(ux_vals)
                        miny, maxy = min(vy_vals), max(vy_vals)
                        dx = maxx - minx
                        dy = maxy - miny
                        delta = max(dx, dy) or 1.0
                        midx = (minx + maxx)/2.0
                        midy = (miny + maxy)/2.0
                        st_a = (midx - 20*delta, midy - delta)
                        st_b = (midx, midy + 20*delta)
                        st_c = (midx + 20*delta, midy - delta)

                        # triangles represented as tuples of points
                        triangles = [(st_a, st_b, st_c)]

                        for p in uv_points:
                            bad = []
                            polygon = []
                            for tri in triangles:
                                if in_circumcircle(p, tri):
                                    bad.append(tri)
                            # find boundary (unique) edges
                            edges = []
                            for tri in bad:
                                for edge in ((tri[0],tri[1]), (tri[1],tri[2]), (tri[2],tri[0])):
                                    # normalize edge direction
                                    e = edge
                                    found = False
                                    for i, existing in enumerate(edges):
                                        if (abs(existing[0][0]-e[1][0])<1e-12 and abs(existing[0][1]-e[1][1])<1e-12 and
                                            abs(existing[1][0]-e[0][0])<1e-12 and abs(existing[1][1]-e[0][1])<1e-12):
                                            # opposite edge exists -> remove
                                            edges.pop(i)
                                            found = True
                                            break
                                    if not found:
                                        edges.append(e)
                            # remove bad triangles
                            triangles = [t for t in triangles if t not in bad]
                            # form new triangles from polygon edges
                            for edge in edges:
                                triangles.append((edge[0], edge[1], p))

                        # remove triangles that include super-triangle vertices
                        def is_super_vertex(v):
                            for sv in (st_a, st_b, st_c):
                                if abs(v[0]-sv[0])<1e-9 and abs(v[1]-sv[1])<1e-9:
                                    return True
                            return False

                        triangles = [t for t in triangles if not (is_super_vertex(t[0]) or is_super_vertex(t[1]) or is_super_vertex(t[2]))]

                        # Build adjacency: for each input point, collect circumcenters of incident triangles
                        point_tri_map = {i: [] for i in range(len(uv_points))}
                        # we need to match triangle vertices to input points; because triangles use p references from uv_points,
                        # we can map by coordinates
                        coord_to_index = { (round(p[0],12), round(p[1],12)) : i for i,p in enumerate(uv_points) }

                        for tri in triangles:
                            cc = circumcenter(tri[0], tri[1], tri[2])
                            if cc is None:
                                continue
                            # for each vertex of triangle, if it's an input point, append circumcenter
                            for v in tri:
                                key = (round(v[0],12), round(v[1],12))
                                if key in coord_to_index:
                                    point_tri_map[coord_to_index[key]].append(cc)

                        # build polygon for each original input point
                        voronoi_polygons_uv = []
                        for idx in range(len(u_pts)):
                            if u_pts[idx] is None or v_pts[idx] is None:
                                voronoi_polygons_uv.append([])
                                continue
                            # map to uv_points index
                            try:
                                ui = u_pts[idx]; vi = v_pts[idx]
                                key = (round(ui,12), round(vi,12))
                                if key not in coord_to_index:
                                    voronoi_polygons_uv.append([])
                                    continue
                                pi = coord_to_index[key]
                                centers = point_tri_map.get(pi, [])
                                if not centers:
                                    voronoi_polygons_uv.append([])
                                    continue
                                # sort centers around centroid
                                cx = sum([c[0] for c in centers])/len(centers)
                                cy = sum([c[1] for c in centers])/len(centers)
                                centers_sorted = sorted(centers, key=lambda c: math.atan2(c[1]-cy, c[0]-cx))
                                voronoi_polygons_uv.append(centers_sorted)
                            except Exception:
                                voronoi_polygons_uv.append([])
                except Exception:
                    # fallback to clipping if anything fails
                    mode = 'clip'
                    voronoi_polygons_uv = []
            if mode != 'fortune':
                for i_pt, (ui, vi) in enumerate(zip(u_pts, v_pts)):
                    if ui is None or vi is None:
                        voronoi_polygons_uv.append([])
                        continue
                    # start with boundary polygon (if exists) or large bbox
                    subject = boundary_uv[:] if boundary_uv else [ (ui-1e3, vi-1e3), (ui+1e3, vi-1e3), (ui+1e3, vi+1e3), (ui-1e3, vi+1e3) ]

                    # intersect with every half-plane induced by other points
                    for j_pt, (uj, vj) in enumerate(zip(u_pts, v_pts)):
                        if j_pt == i_pt or uj is None or vj is None:
                            continue
                        # inequality: keep points x where dist(x,pi) <= dist(x,pj)
                        # derive as: x·(pi - pj) <= (pi·pi - pj·pj)/2
                        a = (ui - uj)
                        b = (vi - vj)
                        c = (ui*ui + vi*vi - uj*uj - vj*vj) / 2.0
                        subject = clip_polygon_by_halfplane(subject, a, b, c, tol)
                        if not subject:
                            break

                    voronoi_polygons_uv.append(subject)

            # Optional orthogonalization / snapping post-process.
            # If the mode string contains 'orth' (e.g., 'clip_orth' or 'fortune_orth')
            # we'll snap polygon vertices to the nearest ux/vy grid lines and replace
            # diagonal edges with an L-shaped pair of orthogonal edges.
            orth_flag = False
            try:
                if isinstance(mode, str) and 'orth' in mode:
                    orth_flag = True
            except Exception:
                orth_flag = False

            if orth_flag and not axis_mode:
                # helpers
                def point_in_poly(pt, poly):
                    # ray-casting algorithm, pt=(x,y), poly list of (x,y)
                    x, y = pt
                    inside = False
                    n = len(poly)
                    for i in range(n):
                        xi, yi = poly[i]
                        xj, yj = poly[(i+1) % n]
                        intersect = ((yi > y) != (yj > y)) and (x < (xj - xi) * (y - yi) / (yj - yi + 1e-30) + xi)
                        if intersect:
                            inside = not inside
                    return inside

                def snap_to_grid(pt):
                    x, y = pt
                    # find nearest ux and vy
                    nx = ux[0]
                    best_dx = abs(x - nx)
                    for uxv in ux:
                        d = abs(x - uxv)
                        if d < best_dx:
                            best_dx = d; nx = uxv
                    ny = vy[0]
                    best_dy = abs(y - ny)
                    for vyv in vy:
                        d = abs(y - vyv)
                        if d < best_dy:
                            best_dy = d; ny = vyv
                    return (nx, ny)

                def collapse_duplicates(poly):
                    out = []
                    for p in poly:
                        if not out or (abs(p[0]-out[-1][0])>1e-12 or abs(p[1]-out[-1][1])>1e-12):
                            out.append(p)
                    # also ensure first != last
                    if len(out) > 1 and abs(out[0][0]-out[-1][0])<1e-12 and abs(out[0][1]-out[-1][1])<1e-12:
                        out.pop()
                    return out

                def collapse_collinear(poly):
                    # remove middle points on straight segments
                    if len(poly) < 3:
                        return poly
                    out = []
                    n = len(poly)
                    for i in range(n):
                        a = poly[i-1]
                        b = poly[i]
                        c = poly[(i+1) % n]
                        # check if b is collinear with a->c (axis aligned only)
                        if (abs(a[0]-b[0])<1e-12 and abs(b[0]-c[0])<1e-12) or (abs(a[1]-b[1])<1e-12 and abs(b[1]-c[1])<1e-12):
                            # b is redundant
                            continue
                        out.append(b)
                    return out

                def orthogonalize_poly(orig_poly):
                    if not orig_poly:
                        return []
                    # snap vertices to grid
                    snapped = [ snap_to_grid(p) for p in orig_poly ]
                    snapped = collapse_duplicates(snapped)
                    if len(snapped) < 3:
                        return []
                    newpoly = []
                    n = len(snapped)
                    for i in range(n):
                        s = snapped[i]
                        e = snapped[(i+1) % n]
                        newpoly.append(s)
                        if abs(s[0]-e[0])<1e-12 or abs(s[1]-e[1])<1e-12:
                            # already orthogonal
                            continue
                        # diagonal: choose an intermediate corner that lies inside original polygon
                        cand1 = (s[0], e[1])
                        cand2 = (e[0], s[1])
                        use = None
                        if point_in_poly(cand1, orig_poly):
                            use = cand1
                        elif point_in_poly(cand2, orig_poly):
                            use = cand2
                        else:
                            # fallback to cand1
                            use = cand1
                        newpoly.append(use)

                    # close and cleanup
                    newpoly = collapse_duplicates(newpoly)
                    newpoly = collapse_collinear(newpoly)
                    return newpoly

                # apply orthogonalization to each polygon
                new_voronoi = []
                for p in voronoi_polygons_uv:
                    try:
                        newp = orthogonalize_poly(p)
                        new_voronoi.append(newp)
                    except Exception:
                        new_voronoi.append(p)
                voronoi_polygons_uv = new_voronoi

            # Convert UV polygons to world geometry; in axis/grid mode there may be many
            # rectangular polygons (not tied to input points). We simply convert every
            # polygon in voronoi_polygons_uv to a zone in the output list.
            out_zones = []
            kept = 0
            # try to import geometry module once
            try:
                geom_module = __import__('Autodesk.DesignScript.Geometry', fromlist=['']).Autodesk.DesignScript.Geometry
            except Exception:
                geom_module = None

            for poly_uv in voronoi_polygons_uv:
                if not poly_uv:
                    out_zones.append(None)
                    continue
                try:
                    pts3d = [ world_point_from_params(origin, xaxis, yaxis, u, v) for (u,v) in poly_uv ]
                    geom = None
                    created_by = None

                    # 1) Try Polygon.ByPoints
                    if geom_module is not None and hasattr(geom_module, 'Polygon') and hasattr(geom_module.Polygon, 'ByPoints'):
                        try:
                            geom = geom_module.Polygon.ByPoints(pts3d)
                            created_by = 'Polygon.ByPoints'
                        except Exception:
                            geom = None
                    # 2) Try PolyCurve.ByPoints
                    if geom is None and geom_module is not None and hasattr(geom_module, 'PolyCurve') and hasattr(geom_module.PolyCurve, 'ByPoints'):
                        try:
                            geom = geom_module.PolyCurve.ByPoints(pts3d + [pts3d[0]])
                            created_by = 'PolyCurve.ByPoints'
                        except Exception:
                            geom = None
                    # 3) Try building lines and joining them (Line.ByStartPointEndPoint + PolyCurve.ByJoinedCurves)
                    if geom is None and geom_module is not None and hasattr(geom_module, 'Line'):
                        try:
                            Line = geom_module.Line
                            lines = []
                            n = len(pts3d)
                            for ii in range(n):
                                a = pts3d[ii]
                                b = pts3d[(ii+1) % n]
                                try:
                                    ln = Line.ByStartPointEndPoint(a, b)
                                    lines.append(ln)
                                except Exception:
                                    lines = []
                                    break
                            if lines and hasattr(geom_module.PolyCurve, 'ByJoinedCurves'):
                                try:
                                    geom = geom_module.PolyCurve.ByJoinedCurves(lines)
                                    created_by = 'PolyCurve.ByJoinedCurves'
                                except Exception:
                                    geom = None
                        except Exception:
                            geom = None

                    if geom is not None:
                        out_zones.append(geom)
                        kept += 1
                        debug.setdefault('creation_methods', {}).setdefault(created_by, 0)
                        debug['creation_methods'][created_by] += 1
                    else:
                        out_zones.append(pts3d)
                        kept += 1
                        debug.setdefault('creation_methods', {}).setdefault('points_fallback', 0)
                        debug['creation_methods']['points_fallback'] += 1
                except Exception:
                    out_zones.append(None)
                    debug.setdefault('creation_methods', {}).setdefault('errors', 0)
                    debug['creation_methods']['errors'] += 1

            debug['kept_zone_count'] = kept
            # Attempt to write debug info to a JSON file in the current working directory
            try:
                if debug_out_path:
                    # If a folder was provided, ensure it exists and write file there
                    try:
                        if os.path.isdir(debug_out_path):
                            debug_path = os.path.join(debug_out_path, 'split_plane_debug.json')
                        else:
                            # maybe user provided full file path
                            debug_path = debug_out_path
                    except Exception:
                        debug_path = os.path.join(os.getcwd(), 'split_plane_debug.json')
                else:
                    debug_path = os.path.join(os.getcwd(), 'split_plane_debug.json')

                if json is not None:
                    with open(debug_path, 'w') as f:
                        json.dump(debug, f, indent=2)
                else:
                    with open(debug_path, 'w') as f:
                        f.write(repr(debug))
                debug['debug_file'] = debug_path
            except Exception as e:
                debug['debug_write_error'] = str(e)

            OUT = (out_zones, ux, vy, debug)
