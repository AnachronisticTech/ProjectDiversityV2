public class StringRepo
{
    /* public static readonly string StaticStringName = "something";
     * public const string ConstStringName = "something2";
     */

    public class Controllers
    {
        public const string NavigationLabel = "Navigation";
        public const string WalkMultiplierToolTip = "Adjust how fast the player walks.";
        public const string RunMultiplierToolTip = "Adjust the multiplier for the player's running speed.";
        
        public const string JumpingLabel = "Jumping";
        public const string MaxJumpToolTip = "The maximum amount of meters the player can achieve by jumping.";
        public const string ShortJumpMultiplierToolTip = "Multiplier that will apply on the max jump when the player is jumping while moving.";
        
        public const string CrouchingLabel = "Crouching";
        public const string CrouchMultiplierToolTip = "Multiplier that will apply to the current speed while the player is crouching.";
        public const string LongJumpChargeTime = "Affects the charge time of a crouch jump. The higher the number the faster it will fully charge.";
        
        public const string NavigationDebugLabel = "Show navigation data debug";
        public const string CharacterVelocityString = "Character velocity: ";
        public const string ClearStoredNavDataLabel = "Clear stored Navigation data";
    }

    public class Physics
    {
        public const string PlanetToolTip = "Chose which planet's gravity you want to apply or add custom gravity value.";
        public const string GroundTransformToolTip = "Assign the gameobject that you want to check for ground collision of the player.";
        public const string CollisionLayerMaskToolTip = "The masks that can affect gravity collision detection.";

        public const string VisibilityLabel = "Visibility Data";
    }

    public class Waypoint
    {
        public const string WaypointLabel = "Waypoint Data";
        
        public const string WaypointDebugLabel = "Show waypoint data debug";
        public const string WaypointDestinationString = "Waypoint destination: ";
    }

    public class Debug
    {
        public const string OnlyForTestingToolTip = "Only for debuging reasons.";

        public const string VisibilityDebugLabel = "Show visibility data debug";
    }
}
