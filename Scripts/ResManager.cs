using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResManager : MonoBehaviour
{
    /// <summary>
    /// Словарь содержащий все глобальные ресурсы на уровне всей игры
    /// </summary>
    public static Dictionary<string, Resource> res = new Dictionary<string, Resource>();
    #region Resource Names
    public const string Wood = "wood";
    public const string Stone = "stone";
    public const string Iron = "iron";
    public const string Food = "food";
    public const string Mood = "mood";
    public const string Water = "water";
    public const string Gold = "gold";
    public const string Human = "human";
    public const string Vegetable = "vegetable";
    public const string Pike = "pike";
    public const string Sword = "sword";
    public const string Bow = "bow";
    #endregion
    [SerializeField] private string[] resNames;
    [SerializeField] private Sprite[] resSprites;
    [SerializeField] private Sprite[] boxSprites;
    [SerializeField] private Sprite[] moodSprites;
    [SerializeField] private GameObject[] panels;

    private void Awake()
    {
        // Создаем экземпляры всех ресурсов в игре
        // Для ресурса Mood используется дополнительные функционал
        for (int i = 0; i < resNames.Length; i++)
        {
            switch (resNames[i])
            {
                case Mood:
                    res.Add(resNames[i], new ResourceMood(resSprites[i], panels[i], moodSprites, resNames[i]));
                    break;
                default:
                    res.Add(resNames[i], new Resource(resSprites[i], boxSprites[i], panels[i], resNames[i]));
                    break;
            }
        }
    }

    /// <summary>
    /// Глобальный ресурс
    /// </summary>
    public class Resource
    {
        // Изменения количества ресурсов возможно только посредством
        // Взаимодействия с конкретным хранилищем, данный класс является
        // Лишь своеобразным интерфейсом взаимодействия с хранилищами

        #region UI Elements
        protected GameObject panel;
        protected Image redBar;
        protected Image greenBar;
        protected TextMeshProUGUI amountText;
        #endregion

        public Resource() { }
        public Resource(Sprite resSprite, Sprite boxSprite, GameObject panel, string name)
        {
            this.amountText = panel.GetComponentInChildren<TextMeshProUGUI>();
            this.redBar = panel.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            this.greenBar = panel.transform.GetChild(1).GetChild(1).GetComponent<Image>();
            this.ResSprite = resSprite;
            this.BoxSprite = boxSprite;
            this.panel = panel;
            this.MaxAmount = 0;
            this.Amount = 0;
            this.Name = name;
        }

        protected List<IStorage> storages = new List<IStorage>(0);

        public string Name { get; protected set; }
        // Глобальное количество 
        public int Amount
        {
            get => amount;
            protected set
            {
                amount = value;
                ChangeUI();
            }
        }
        protected int amount;

        public int MaxAmount
        {
            get => maxAmount;
            set
            {
                maxAmount = value;
                ChangeUI();
            }
        }
        protected int maxAmount;

        public Sprite ResSprite { get; protected set; }
        public Sprite BoxSprite { get; protected set; }

        public IStorage GetStorage(Worker human, IStorage workerLocalStorage, int value = 1)
        {
            foreach (var s in storages)
            {
                if (!s.IsLocal && value <= s.GetMaxAmount(Name) - s.GetAmount(Name)
                    && MyAstarHandler.IsPathPossible(human.transform, s.Destination))
                    return s;
            }
            if (workerLocalStorage.GetMaxAmount(Name) - workerLocalStorage.GetAmount(Name) >= value
                && MyAstarHandler.IsPathPossible(human.transform, workerLocalStorage.Destination))
                return workerLocalStorage;
            return null;
        }

        public List<IStorage> GetStorages() => storages;

        /// <summary>
        /// Используется когда меняем количество ресурсов из вне
        /// </summary>
        /// <param name="value">Добавляемое количество</param>
        public void ToStorage(int value)
        {
            if (value < 0)
            {
                for (int i = 0; value < 0; i++)
                {
                    int delta = storages[i].GetAmount(Name) + value;
                    storages[i].AddAmount(Name, -Mathf.Clamp(-value, 0, storages[i].GetAmount(Name)));
                    value = delta;
                }
            }
            else
            {
                for (int i = 0; value > 0; i++)
                {
                    int delta = storages[i].GetMaxAmount(Name) - storages[i].GetAmount(Name);
                    storages[i].AddAmount(Name, Mathf.Clamp(delta, 0, value));
                    value -= delta;
                }
            }
        }

        /// <summary>
        /// Используется когда хранилище изменило количество ресурсов
        /// </summary>
        /// <param name="value">Добавляемое количество</param>
        public void FromStorage(int value)
        {
            Amount += value;
        }

        public void AddStorage(IStorage storage)
        {
            storages.Add(storage);
        }

        public void RemoveStorage(IStorage storage)
        {
            if (storages.Contains(storage))
            {
                MaxAmount -= storage.GetMaxAmount(Name);
                Amount -= storage.GetAmount(Name);
                storages.Remove(storage);
            }
        }

        public void SetPossibleAmount(int delta)
        {
            int value = amount - delta;

            amountText.text = value.ToString();
            amountText.color = Colors.RedUI;

            greenBar.fillAmount = maxAmount > 0 ? (float)value / maxAmount : 0;
            redBar.fillAmount = maxAmount > 0 ? (float)amount / maxAmount : 0;
        }

        public void SetNormalAmount()
        {
            amountText.color = Colors.BlackUI;
            redBar.fillAmount = 0;
            ChangeUI();
        }

        protected virtual void ChangeUI()
        {
            amountText.text = amount.ToString();
            if (greenBar)
                greenBar.fillAmount = maxAmount > 0 ? (float)amount / (float)maxAmount : 0;
        }
    }

    public class ResourceMood : Resource
    {
        public ResourceMood(Sprite icon, GameObject panel, Sprite[] moods, string name)
        {
            this.image = panel.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            this.amountText = panel.GetComponentInChildren<TextMeshProUGUI>();
            this.redBar = panel.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            this.greenBar = panel.transform.GetChild(1).GetChild(1).GetComponent<Image>();
            this.ResSprite = icon;
            this.panel = panel;
            this.Name = name;

            this.moods = new Sprite[moods.Length];
            for (int i = 0; i < moods.Length; i++)
                this.moods[i] = moods[i];
            MaxAmount = 100;
            FromStorage(70);
        }

        private readonly Sprite[] moods;
        private readonly Image image;

        protected override void ChangeUI()
        {
            base.ChangeUI();
            // Есть три состояния настоения, плохое, среднее и хорошее
            if (moods != null && image != null)
                image.sprite = moods[amount / 33];
        }
    }
}