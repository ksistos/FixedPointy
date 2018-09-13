namespace FixedPointy.Collisions {
  // todo: rework intersection - too complex
  public static class PolygonCollision {
    // Check if polygon A is going to collide with polygon B for the given velocity
    public static PolygonCollisionResult CheckCollision(FixPolygon polygonA, FixPolygon polygonB, FixVec2 velocity) {
      var result = new PolygonCollisionResult {Intersect = true, WillIntersect = true};

      var edgeCountA = polygonA.Edges.Length;
      var edgeCountB = polygonB.Edges.Length;
      var minIntervalDistance = Fix.MaxValue;
      var translationAxis = new FixVec2();

      // Loop through all the edges of both polygons
      for(var edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++) {
        var edge = edgeIndex < edgeCountA ? polygonA.Edges[edgeIndex] : polygonB.Edges[edgeIndex - edgeCountA];

        // ===== 1. Find if the polygons are currently intersecting =====

        // Find the axis perpendicular to the current edge
        var axis = new FixVec2(-edge.Y, edge.X).Normalize();

        // Find the projection of the polygon on the current axis
        Fix minA;
        Fix maxA;
        Fix minB;
        Fix maxB;
        ProjectPolygon(axis, polygonA, out minA, out maxA);
        ProjectPolygon(axis, polygonB, out minB, out maxB);

        // Check if the polygon projections are currently intersecting
        if(IntervalDistance(minA, maxA, minB, maxB) > 0)
          result.Intersect = false;

        // ===== 2. Now find if the polygons *will* intersect =====

        // Project the velocity on the current axis
        var velocityProjection = axis.Dot(velocity);

        // Get the projection of polygon A during the movement
        if(velocityProjection < 0)
          minA += velocityProjection;
        else
          maxA += velocityProjection;

        // Do the same test as above for the new projection
        var intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
        if(intervalDistance > 0)
          result.WillIntersect = false;

        // If the polygons are not intersecting and won't intersect, exit the loop
        if(!result.Intersect && !result.WillIntersect)
          break;

        // Check if the current interval distance is the minimum one. If so store
        // the interval distance and the current distance.
        // This will be used to calculate the minimum translation FixVec2
        intervalDistance = FixMath.Abs(intervalDistance);
        if(intervalDistance < minIntervalDistance) {
          minIntervalDistance = intervalDistance;
          translationAxis = axis;

          var d = polygonA.Center - polygonB.Center;
          if(d.Dot(translationAxis) < 0)
            translationAxis = -translationAxis;
        }
      }

      // The minimum translation FixVec2 can be used to push the polygons apart.
      // First moves the polygons by their velocity
      // then move polygonA by MinimumTranslationVector.
      if(result.WillIntersect)
        result.MinimumTranslationVector = translationAxis * minIntervalDistance;

      return result;
    }

    // Calculate the distance between [minA, maxA] and [minB, maxB]
    // The distance will be negative if the intervals overlap
    private static Fix IntervalDistance(Fix minA, Fix maxA, Fix minB, Fix maxB) {
      if(minA < minB)
        return minB - maxA;

      return minA - maxB;
    }

    // Calculate the projection of a polygon on an axis and returns it as a [min, max] interval
    private static void ProjectPolygon(FixVec2 axis, FixPolygon polygon, out Fix min, out Fix max) {
      // To project a point on an axis use the dot product
      var d = axis.Dot(polygon.Points[0]);
      min = d;
      max = d;
      for(var i = 0; i < polygon.Points.Length; i++) {
        d = polygon.Points[i].Dot(axis);
        if(d < min)
          min = d;
        else if(d > max)
          max = d;
      }
    }
  }
}