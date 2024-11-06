using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TittliSorat_GameManager : MonoBehaviour
{
    [SerializeField] private SpinWheelnew_TitliSorat outerSpinWheel;


    [SerializeField] private List<BetButtons> betButtons;

    [SerializeField] private long UserCoins = 10000;

    [SerializeField] private Text userCoinsText;

    [SerializeField] private Text currentTotalBetText;
    [SerializeField] private Text totalWinText;
    [SerializeField] private List<GameObject> Wining = new List<GameObject>();
    [SerializeField] private List<Sprite> CharacterImages = new List<Sprite>();
    public string Time;
    public GameObject Block;
    public GameObject HistoryData;
    public Transform HistoryDataParent;



    [HideInInspector] public List<ButtonHandlers> ButtonHandlers = new List<ButtonHandlers>();
    [HideInInspector] public List<List<IRemoveHandler>> removeHandlers = new List<List<IRemoveHandler>>();
    [HideInInspector] public List<IWinHandler> winHandlers = new List<IWinHandler>();

    [HideInInspector] public bool canBet = false;

    [HideInInspector] public int removeCount = 0;

    private BetButtons selectedBetbutton;
    private long totalBet = 0;
    private long totalUserCoins = 0;
    private int historyCounter = 0;

    public BetButtons SelectedBetbutton { get => selectedBetbutton; }

    public static TittliSorat_GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SelectBet(1);
        Clear();
        totalWinText.text = "0";
        totalUserCoins = UserCoins;
        userCoinsText.text = totalUserCoins.ToString();

        canBet = true;
    }
    private void Start()
    {
        TimerController_TitliSorat.inst.WheelSpinTime += AfterCompleteTime;
    }
    void AfterCompleteTime()
    {
        int Result = UnityEngine.Random.Range(1, 13);
        outerSpinWheel.SpinTheWheel(12, onOuterWheelSpinComplete);
        outerSpinWheel.AssignWinningSlot(Result);
        OnWin(Result);
       

    }
    private void onOuterWheelSpinComplete(int a_WinningSlot)
    {
        outerSpinWheel.WheelSpinStoppped();
    }
  
    public void SelectBet(int id)
    {
        betButtons.ForEach(x => x.buttonTransfrom.localScale = Vector2.one);

        selectedBetbutton = betButtons.Find(x => x.id == id);

        selectedBetbutton.buttonTransfrom.localScale = Vector2.one * 1.1f;
    }

    public void Randombet(int count)
    {
        int totalAmount = count * selectedBetbutton.amount;
        Debug.LogError(count);
        if (totalAmount <= totalUserCoins)
        {
            Clear();
            List<ButtonHandlers> tempList = new List<ButtonHandlers>(ButtonHandlers.FindAll(x => x.CheckIfGroup() == false));
            List<ButtonHandlers> randomList = new List<ButtonHandlers>();
            for (int i = 0; i < count; i++)
            {
                int value = UnityEngine.Random.Range(0, tempList.Count);
                randomList.Add(tempList[value]);
                tempList.RemoveAt(value);
            }

            foreach(ButtonHandlers handlers in randomList)
            {
                handlers.RandomBet();
            }
        }
    }

    public bool Bet(int amount)
    {
        if (canBet == false)
        {
            return false;
        }

        if (amount > totalUserCoins)
        {
            return false;
        }
        else
        {
            totalBet = totalBet + amount;
            currentTotalBetText.text = totalBet.ToString();
            totalUserCoins = totalUserCoins - amount;
            userCoinsText.text = totalUserCoins.ToString();
            return true;
        }
    }

    public void RemoveBet(int amount)
    {
        totalBet = totalBet - amount;
        currentTotalBetText.text = totalBet.ToString();

        totalUserCoins = totalUserCoins + amount;
        userCoinsText.text = totalUserCoins.ToString();
    }

    public void Clear()
    {
        if (canBet == false)
        {
            return;
        }

        removeHandlers.Clear();
        removeCount = 0;
        ButtonHandlers.ForEach(x => x.Clear());
        totalBet = 0;
        currentTotalBetText.text = totalBet.ToString();
        totalUserCoins = UserCoins;
        userCoinsText.text = totalUserCoins.ToString();

    }

    public void Remove()
    {
        if (canBet == false)
        {
            return;
        }

        if (removeHandlers.Count > 0)
        {
            List<IRemoveHandler> re = removeHandlers[removeCount - 1];
            re.ForEach(x => { if (x != null) x.Remove(); });
            re.Clear();

            removeHandlers.RemoveAt(removeCount - 1);
            removeCount -= 1;
        }
    }

    public void Repeat()
    {
        if (canBet == false)
        {
            return;
        }

        long repeatSum = 0;
        ButtonHandlers.ForEach(x => repeatSum = repeatSum + x.GetPrevRoundTotalSum());
        if (repeatSum <= totalUserCoins)
        {
            ButtonHandlers.ForEach(x => x.Repeat());

        }
    }

    public void DoubleUp()
    {
        if (canBet == false)
        {
            return;
        }

        long repeatSum = 0;
        ButtonHandlers.ForEach(x => repeatSum = repeatSum + x.GetRoundTotalSum());
        if (repeatSum <= totalUserCoins)
        {
            removeHandlers.Add(new List<IRemoveHandler>());
            ButtonHandlers.ForEach(x => x.DoubleUp(removeCount));
            removeCount += 1;
        }
    }

    public void TimerEnd()
    {
        canBet = false;
        betButtons.ForEach(x => x.buttonTransfrom.localScale = Vector2.one);
    }

    public void OnWin(int outerId)
    {
        ButtonHandlers.ForEach(x => x.SavePrevRound());
       
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(9f);
        seq.AppendCallback(() => winHandlers.ForEach(x => x.OnWin(outerId)));
        seq.AppendCallback(() => Wining[outerId - 1].SetActive(true));
        seq.AppendCallback(() => SetGameHistory(outerId));
        seq.AppendInterval(10f);
        seq.AppendCallback(() => Wining[outerId - 1].SetActive(false));
        seq.AppendCallback(() => canBet = true);
        seq.AppendCallback(() => Clear());
        seq.AppendCallback(() => totalWinText.text = "0");
        seq.AppendCallback(() => SelectBet(1));
        seq.AppendCallback(() => TimerController_TitliSorat.inst.StartTimer(90f));
        
        //Wining.ForEach(x =>x.image.SetActive(false));
    }

    public void ShowWinAmount(long amount)
    {
        totalWinText.text = (amount * 10).ToString();
        totalUserCoins = totalUserCoins + (amount * 10);
        userCoinsText.text = totalUserCoins.ToString();
        UserCoins = totalUserCoins;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetGameHistory(int result)
    {
        GameObject go = Instantiate(HistoryData, HistoryDataParent);
        go.transform.GetChild(0).GetComponent<Text>().text = Time.ToString();
        go.transform.GetChild(1).GetComponent<Image>().sprite = CharacterImages[result-1];
        go.SetActive(true);
        if (HistoryDataParent.childCount >10)
        {
            Debug.LogError("Deleted");
            Destroy(HistoryDataParent.GetChild(0));
        }
    }
}

[System.Serializable]
public class BetButtons
{
    public int id;
    public Transform buttonTransfrom;
    public int amount;
}

public interface ButtonHandlers
{
    public void Clear();
    public void Repeat();
    public void DoubleUp(int removeCountIndex);
    public void SavePrevRound();
    public bool CheckifLastSavedDataAvailable();
    public long GetPrevRoundTotalSum();
    public long GetRoundTotalSum();
    public bool CheckIfGroup();
    public void RandomBet();

}

public interface IRemoveHandler
{
    public void Remove();
}

public interface IWinHandler
{
    public void OnWin(int outerId);

}
[System.Serializable]
public class WinningData
{
    public Text time;
    public Image resultimage;
}
