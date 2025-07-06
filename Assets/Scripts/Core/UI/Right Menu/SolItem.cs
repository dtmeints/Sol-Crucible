using DG.Tweening;
using UnityEngine;

public class SolItem : Item
{
    //public override bool CanPlaceHere() => !ItemPlacer.FirstSol || Vector2.Distance(mouseWorldPos, Vector2.zero) <= GameSettings.Instance.Data.FirstSolMaxDistance; 
    public override Transform Place() {
        GameObject sol = Instantiate(Data.Prefab, mouseWorldPos, Quaternion.identity);
        sol.transform.localScale = Vector3.zero;
        sol.transform.DOScale(Vector3.one * 0.5f, 0.1f).SetEase(Ease.OutCubic).OnComplete(() => {
            sol.transform.localScale = Vector3.one * 0.5f;
            sol.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutElastic).OnComplete(() => sol.transform.localScale = Vector3.one);
        });
        return sol.transform;
    }
}