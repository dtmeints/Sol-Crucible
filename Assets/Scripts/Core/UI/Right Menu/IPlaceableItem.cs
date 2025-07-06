using UnityEngine;

public interface IPlaceableItem
{
    bool CanPlaceHere();
    Transform Place();
}