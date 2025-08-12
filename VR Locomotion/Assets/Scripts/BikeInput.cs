public interface IBikeInput
{
    float throttleInput { get; }
    float tilt { get; }
    float steering { get; }
    bool isBraking { get; }
}
