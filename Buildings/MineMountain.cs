using UnityEngine;
using static StaticData;

public class MineMountain : Building
{
    [SerializeField] private Mine minePrefab;
    [SerializeField] private SpriteRenderer mountainSpriteRenderer;
    private Sprite fullTroleySprite;
    private string resName;

    public void SetMountainType(Sprite sprite, Sprite sprite1,string resName)
    {
        mountainSpriteRenderer.sprite = sprite;
        fullTroleySprite = sprite1;
        this.resName = resName;
    }

    public bool BuyMine()
    {
        if(Price.CanBuy(prices.ToArray()))
        {
            var mine = Instantiate(minePrefab, transform.position, transform.rotation);
            mine.SetMountainType(mountainSpriteRenderer.sprite, fullTroleySprite, resName);
            mine.mineMountain = this;
            mine.Build();
            gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    protected override void OnDisable()
    {

    }

    protected override void OnEnable()
    {
        ForestStatic.ClearArea(transform.position, Size);
    }
}
