using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class GroupCardSelect_TitliSorat : InputDetection_TitliSorat, ButtonHandlers, IRemoveHandler
    {

        [SerializeField] private List<CardSelect_TitliSorat> cardSelects;

        private List<BetButtons> totalBets = new List<BetButtons>();

        public int TotalGrpBetsCount { get => totalBets.Count; }

        private List<BetButtons> prevRoundTotalBets = new List<BetButtons>();

        private List<int> removeCount = new List<int>();

        private void Start()
        {
            cardSelects.ForEach(x => x.groupCardSelect.Add(this));

        TittliSorat_GameManager.instance.ButtonHandlers.Add(this);

            ToggleChipVisibility(false);
        }

        private void UpdateChipVisualData(long a_Bet)
        {
        }

    

        private void ToggleChipVisibility(bool a_State)
        {

        }

        public long GetTotalBetSum()
        {
            long sum = 0;
            totalBets.ForEach(x => sum = x.amount + sum);
            return sum;
        }

        public override void LeftClick()
        {
            base.LeftClick();

            BetButtons bet = TittliSorat_GameManager.instance.SelectedBetbutton;
            int amt = bet.amount * cardSelects.Count;

            if (!TittliSorat_GameManager.instance.Bet(amt))
            {
                return;
            }

            totalBets.Add(bet);

            long totalBet = GetTotalBetSum();

            UpdateChipVisualData(totalBet);

            if (totalBets.Count == 1)
            {
                ToggleChipVisibility(true);
            }

            cardSelects.ForEach(x => x.OnClickGroupLeftClick());


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

                int amt = bet.amount * cardSelects.Count;

            TittliSorat_GameManager.instance.RemoveBet(amt);

                long totalBet = GetTotalBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == 0)
                {
                    ToggleChipVisibility(false);
                }

                cardSelects.ForEach(x => x.OnClickGroupRightClick());


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

                int amt = bet.amount * cardSelects.Count;

            TittliSorat_GameManager.instance.RemoveBet(amt);

                long totalBet = GetTotalBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == 0)
                {
                    ToggleChipVisibility(false);
                }

                cardSelects.ForEach(x => x.OnClickGroupRightClick());

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
                int amt = bet.amount * cardSelects.Count;

            TittliSorat_GameManager.instance.Bet(amt);
            }

            long totalBet = GetTotalBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);

            cardSelects.ForEach(x => x.OnClickGroupLeftClick());
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
            if (prevRoundTotalBets.Count > 0)
            {
                return true;
            }

            return false;
        }

        public long GetPrevRoundTotalSum()
        {
            long sum = 0;
            prevRoundTotalBets.ForEach(x => sum = sum + x.amount);
            return sum;
        }

        public long GetRoundTotalSum()
        {
            long sum = 0;
            totalBets.ForEach(x => sum = sum + x.amount);
            return sum;
        }

        public void DoubleUp(int removeCountIndex)
        {
            if (totalBets.Count == 0)
                return;

            foreach (BetButtons bet in totalBets)
            {
                int amt = bet.amount * cardSelects.Count;

            TittliSorat_GameManager.instance.Bet(amt);
            }

            totalBets.AddRange(totalBets);

            long totalBet = GetTotalBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);

            cardSelects.ForEach(x => x.OnClickGroupLeftClick());

        TittliSorat_GameManager.instance.removeHandlers[removeCountIndex].Add(this);
            removeCount.Add(TittliSorat_GameManager.instance.removeCount);
        }

    public bool CheckIfGroup()
    {
        return true;
    }

    public void RandomBet()
    {
        Debug.Log("Should not be called");
    }
}
