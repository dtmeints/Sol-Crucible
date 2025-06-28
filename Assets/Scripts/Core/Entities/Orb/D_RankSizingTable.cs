using UnityEngine;

[CreateAssetMenu(menuName ="Data/Sizing Table")]
public class D_RankSizingTable : ScriptableObject
{

    [SerializeField] float[] sizingTable;

    public float GetSize(int rank)
    {
        if (rank < 0)
            return 0f;

        if (sizingTable.Length <= rank)
            return sizingTable[^1];

        else return sizingTable[rank];
    }
}
