using System;
using UnityEngine;

public class Mine : GatheringBuilding
{
    [SerializeField] private SpriteRenderer[] troleySpriteRenderers;
    [SerializeField] private SpriteRenderer troleySpriteRenderer;
    [SerializeField] private Sprite emptyTroleySprite;
    [SerializeField] private Sprite fullTroleySprite;
    [SerializeField] private SpriteRenderer mountainSpriteRenderer;
    [SerializeField] protected string startWorkTrigger;
    [HideInInspector] public MineMountain mineMountain;
    protected override string StartWorkTrigger { get => startWorkTrigger; }

    protected override void SetActivities()
    {
        var animationTrigger = "Mine";
        var workingTime = 0f;

        var a = new HumanActivity<Worker>
        {
            GetPlace = (worker) => places[0],
            CantReachTarget = (worker) => worker.StopGo(),
            ExecuteAction = (worker) => { StandartExecuteAction(worker, animationTrigger, workingTime); },
            FinishActivity = (worker) => { },
            SetNewActivity = (worker) => { }
        };
        activities.Add(a);
        SetGoToStorage();
    }

    public override HumanActivity<Worker> GetNewActivity(out int index)
    {
        index = -1;
        return activities[0];
    }

    public override void SendToStorage(Worker worker)
    {
        if (storage.Amount < storage.MaxAmount)
        {
            var length = troleySpriteRenderers.Length;
            if ((storage.Amount + 1) % (storage.MaxAmount / length) == 0)
                animator.SetTrigger("Back");

            base.AddAmount(resName, 1);
            worker.StartWork(secondsPerResource);
        }
        else
            base.SendToStorage(worker);
    }

    public void TrolleyEmpty()
        => troleySpriteRenderer.sprite = emptyTroleySprite;

    public void TrolleyFull()
        => troleySpriteRenderer.sprite = fullTroleySprite;

    public void UpdateTroley()
    {
        int length = troleySpriteRenderers.Length;
        for (int i = 0; i < length; i++)
            troleySpriteRenderers[i].sprite = i >= storage.Amount / (storage.MaxAmount / length) ? emptyTroleySprite : fullTroleySprite;
    }

    public override void AddAmount(string name, int value)
    {
        base.AddAmount(name, value);
        UpdateTroley();
    }

    public void SetMountainType(Sprite sprite, Sprite troleySprite, string resName)
    {
        fullTroleySprite = troleySprite;
        mountainSpriteRenderer.sprite = sprite;
        this.resName = resName;
    }

    protected override void OnEnable()
        => MyAstarHandler.UpdateArea(transform.position, Size);

    protected override void OnDisable()
        => MyAstarHandler.UpdateArea(transform.position, Size);

    public override void Destroy()
    {
        mineMountain.gameObject.SetActive(true);
        mineMountain.canMove = false;
        base.Destroy();
    }
}
