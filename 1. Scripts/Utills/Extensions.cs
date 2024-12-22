using UnityEngine;
public static class ExtensionMethods
{
    public static Vector3 ToVector3(this Position position)
    {
        return new Vector3(position.X, position.Y, position.Z);
    }

    public static Vector3 ToVector3(this Rotation rotation)
    {
        return new Vector3(rotation.X, rotation.Y, rotation.Z);
    }
    
    public static Position ToPosition(this Vector3 position)
    {
        Position pos = new Position();
        pos.X = position.x;
        pos.Y = position.y;
        pos.Z = position.z;
        return pos;
    }

    public static Rotation ToRotation(this Quaternion rotation)
    {
        Rotation rot = new Rotation();
        rot.X = rotation.eulerAngles.x;
        rot.Y = rotation.eulerAngles.y;
        rot.Z = rotation.eulerAngles.z;
        return rot;
    }

    public static bool IsSameLayer(int layerMask, int targetLayer)
    {
        return layerMask == (layerMask | (1 << targetLayer));
    }
}

public static class ResourcePath
{
    public static string UIPath { get; private set; } = "UI/Popup/";
}

public static class ScenePath
{
    public static string PopupUIPath { get; private set; } = "_UI/Popup";
}