using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardCollection", menuName = "LevelUPCardCollection/NewCollection")]
public class LevelUPCardCollection : ScriptableObject
{
    [SerializeField] private List<LevelUpCard> cards;
    [SerializeField] private List<LevelUpCard> defaultCards;
    [SerializeField] private List<LevelUpCard> jumpCards;
    [SerializeField] private List<LevelUpCard> kickCards;
    [SerializeField] private List<LevelUpCard> sheepTransformCards;

    private bool kickAdded = false;
    private bool jumpAdded = false;
    private bool sheepTransformAddedd = false;

    public void Init()
    {
        ResetToDefault();
        InitializeAllCards();
        LevelUpCardAddKick.KickAdded += OnKickAdded;
        LevelUpCardAddJump.JumpAdded += OnJumpAdded;
        LevelUpCardAddSheepTransform.SheepTransformAdded += OnSheepTransformAdded;
        LevelUpButtonUI.UpgradeChosen += OnUpgradeChosen;
    }
    public void DisableBehavior()
    {
        LevelUpCardAddKick.KickAdded -= OnKickAdded;
        LevelUpCardAddJump.JumpAdded -= OnJumpAdded;
        LevelUpCardAddSheepTransform.SheepTransformAdded -= OnSheepTransformAdded;
        LevelUpButtonUI.UpgradeChosen -= OnUpgradeChosen;
    }
    private void ResetToDefault()
    {
        cards = new List<LevelUpCard>(defaultCards);
        jumpAdded = false;
        kickAdded = false;
    }
    public List<LevelUpCard> GetThreeRandom()
    {
        var shuffled = new List<LevelUpCard>(cards);
        int n = shuffled.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]); // swap
        }
        var result = shuffled.GetRange(0, Mathf.Min(3, shuffled.Count));
        if (LevelCounter.LevelCount > 4)
        {
            if (!kickAdded)
            {
                LevelUpCard _ = cards.First(x => x.CardType == LevelUpCardTypes.KickAdd);
                if (!result.Contains(_))
                {
                    result[0] = _;
                }
            }
            if (!jumpAdded)
            {
                LevelUpCard _ = cards.First(x => x.CardType == LevelUpCardTypes.JumpAdd);
                if (!result.Contains(_))
                {
                    result[0] = _;
                }
            }
        }
        return result;
    }
    public void DeleteCard(LevelUpCard card)
    {
        var _card = cards.FirstOrDefault(x => x.CardType == card.CardType);
        if (_card != null)
        {
            cards.Remove(card);
        }
    }
    private void InitializeAllCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].Init();

        }
    }
    private void OnKickAdded()
    {
        kickAdded = true;
        cards.AddRange(kickCards);
    }
    private void OnJumpAdded()
    {
        jumpAdded = true;
        cards.AddRange(jumpCards);
    }
    private void OnSheepTransformAdded()
    {
        sheepTransformAddedd = true;
        cards.AddRange(sheepTransformCards);
    }
    private void OnUpgradeChosen(LevelUpCard card)
    {

    }
}
