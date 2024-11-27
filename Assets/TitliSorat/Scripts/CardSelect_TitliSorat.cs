using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

    public class CardSelect_TitliSorat : InputDetection_TitliSorat, ButtonHandlers, IRemoveHandler, IWinHandler
    {
        public int id;

        [SerializeField] private Text chipText;

        [HideInInspector] public List<GroupCardSelect_TitliSorat> groupCardSelect;

        private List<BetButtons> totalBets = new List<BetButtons>();

        private List<BetButtons> prevRoundTotalBets = new List<BetButtons>();

        private List<int> removeCount = new List<int>();

        private void Start()
        {
            TittliSorat_GameManager.instance.ButtonHandlers.Add(this);
            TittliSorat_GameManager.instance.winHandlers.Add(this);

            ToggleChipVisibility(false);
        }

        private void ToggleChipVisibility(bool a_State)
        {
        if (!a_State) {
            chipText.text="0";
        }
        }

       

        private void UpdateChipVisualData(long a_Bet)
        {
            chipText.text = "" + Mathf.Abs(a_Bet).ToString("F0");
        }

        private long GetTotalBetSum()
        {
            long sum = 0;
            totalBets.ForEach(x => sum = x.amount + sum);
            return sum;
        }

        private long GetTotalGroupBetSum()
        {
            long sum = 0;
            groupCardSelect.ForEach(x => sum = sum + x.GetTotalBetSum());
            return sum;
        }

        public override void LeftClick()
        {
            base.LeftClick();

            BetButtons bet = TittliSorat_GameManager.instance.SelectedBetbutton;

            if(!TittliSorat_GameManager.instance.Bet(bet.amount))
            {
                return;
            }

            totalBets.Add(bet);

            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            if (totalBets.Count == 1 || groupCardSelect.Find(x => x.TotalGrpBetsCount == 1) != null)
            {
                ToggleChipVisibility(true);

            }

            TittliSorat_GameManager.instance.removeHandlers.Add(new List<IRemoveHandler>() { this });
            removeCount.Add(TittliSorat_GameManager.instance.removeCount);
            TittliSorat_GameManager.instance.removeCount += 1;
        }

        public override void RightClick()
        {
            base.RightClick();

            if (totalBets.Count > 0)
            {
                BetButtons bet = totalBets[totalBets.Count - 1];
                totalBets.RemoveAt(totalBets.Count - 1);

                TittliSorat_GameManager.instance.RemoveBet(bet.amount);

                long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == 0 && groupCardSelect.TrueForAll(x => x.TotalGrpBetsCount == 0))
                {
                    ToggleChipVisibility(false);
                }

                if (removeCount.Count > 0)
                {
                    List<IRemoveHandler> n = TittliSorat_GameManager.instance.removeHandlers[removeCount[removeCount.Count - 1]];
                    n.Remove(this);
                    if (n.Count == 0)
                    {
                        TittliSorat_GameManager.instance.removeHandlers.RemoveAt(removeCount[removeCount.Count - 1]);
                        TittliSorat_GameManager.instance.removeCount -= 1;
                    }
                    removeCount.RemoveAt(removeCount.Count - 1);
                }
            }
        }

        public void OnClickGroupLeftClick()
        {
            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            if (totalBets.Count == 1 || groupCardSelect.Find(x => x.TotalGrpBetsCount == 1) != null)
            {
                ToggleChipVisibility(true);
            }
        }

        public void OnClickGroupRightClick()
        {
            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            if (totalBets.Count == 0 && groupCardSelect.TrueForAll(x => x.TotalGrpBetsCount == 0))
            {
                ToggleChipVisibility(false);
            }
        }

        public void Clear()
        {
            totalBets.Clear();
            removeCount.Clear();

            ToggleChipVisibility(false);
        }

        public void Remove()
        {
            base.RightClick();

            if (totalBets.Count > 0)
            {
                BetButtons bet = totalBets[totalBets.Count - 1];
                totalBets.RemoveAt(totalBets.Count - 1);

                TittliSorat_GameManager.instance.RemoveBet(bet.amount);

                long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == 0 && groupCardSelect.TrueForAll(x => x.TotalGrpBetsCount == 0))
                {
                    ToggleChipVisibility(false);
                }

            }

            removeCount.RemoveAt(removeCount.Count - 1);
        }

        public void Repeat()
        {
            if (prevRoundTotalBets.Count == 0)
                return;

            totalBets.Clear();

            totalBets.AddRange(prevRoundTotalBets);

            foreach (BetButtons bet in totalBets)
            {
                int amt = bet.amount;

                TittliSorat_GameManager.instance.Bet(amt);
            }

            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);
        }

        public void SavePrevRound()
        {
            if (totalBets.Count > 0)
            {
                prevRoundTotalBets.Clear();
                prevRoundTotalBets.AddRange(totalBets);
            }
        }

        public bool CheckifLastSavedDataAvailable()
        {
            if(prevRoundTotalBets.Count > 0)
            {
                return true;
            }

            return false;
        }

        public long GetRoundTotalSum()
        {
            long sum = 0;
            totalBets.ForEach(x => sum = sum + x.amount);
            return sum;
        }

        public long GetPrevRoundTotalSum()
        {
            long sum = 0;
            prevRoundTotalBets.ForEach(x => sum = sum + x.amount);
            return sum;
        }

        public void DoubleUp(int removeCountIndex)
        {
            if (totalBets.Count == 0)
                return;

            foreach (BetButtons bet in totalBets)
            {
                int amt = bet.amount;

                TittliSorat_GameManager.instance.Bet(amt);
            }

            totalBets.AddRange(totalBets);

            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);

            TittliSorat_GameManager.instance.removeHandlers[removeCountIndex].Add(this);
            removeCount.Add(TittliSorat_GameManager.instance.removeCount);
        }

        public void OnWin(int outerId)
        {
            if(outerId == id )
            {
                long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();
                Sequence seq = DOTween.Sequence();
                seq.AppendCallback(() => TittliSorat_GameManager.instance.ShowWinAmount(totalBet));            
            }
        }

    public bool CheckIfGroup()
    {
        return false;
    }

    public void RandomBet()
    {
        LeftClick();
    }
}

