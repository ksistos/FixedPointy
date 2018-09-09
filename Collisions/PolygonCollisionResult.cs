namespace FixedPointy.Collisions {
  public struct PolygonCollisionResult {
    public bool WillIntersect; // Are the polygons going to intersect forward in time?
    public bool Intersect; // Are the polygons currently intersecting

    public FixVec2 MinimumTranslationVector; // The translation to apply to polygon A to push the polygons apart.
  }
}