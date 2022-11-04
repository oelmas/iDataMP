namespace MissionPlanner.Utilities
{
    public class FenceCircle
    {
        public enum PolyType
        {
            Inclusive = MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION,
            Exclusive = MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION
        }

        public PointLatLngAlt Center { get; set; }
        public float Radius { get; set; }

        public PolyType Mode { get; set; }
    }
}