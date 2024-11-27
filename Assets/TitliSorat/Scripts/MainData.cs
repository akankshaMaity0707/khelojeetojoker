using UnityEngine;

[CreateAssetMenu(fileName = "MainData", menuName = "ScriptableObjects/MainData")]
public class MainData : ScriptableObject
{
	public ReceivedLoginData receivedLoginData;

	public PendingDrawDetails pendingDrawDetails;

	public CurrentDrawDetails currentDrawDetails;

	public CurrentDrawDetails lastFewDrawDetails;

}

[System.Serializable]
public class ReceivedLoginData
{
	public string UserID;
	public string retMsg;
	public string retStatus;
	public string Balance;
	public string IsLocked;
	public string Message;
	public int PID;
	public string AutoClaim;
	public string Print;
	public string Assign;
	public Games[] Games;
	public string Now;
}

[System.Serializable]
public class Games
{
	public string GCode;
	public string GName;
	public string GType;
}
[System.Serializable]
public class PendingDrawDetails
{
    public string retMsg;
    public string retStatus;
    public string Query;
    public string Now;
    public string GameID;
    public string Date;
    public Draw[] Draws;
}

[System.Serializable]
public class Draw
{
    public string GID;
    public string DrawDate;
    public string DrawTime;
    public string Status;
}

[System.Serializable]
public class CurrentDrawDetails
{
    public string retMsg;
    public string retStatus;
    public string Query;
    public string Now;
    public string GameID;
    public string Date;
    public CurrentDraws[] Draws;
    public WinDetails WinDtls;
    public string AutoClaimed;
    public string Balance;

    [System.Serializable]
    public class CurrentDraws
    {
        public string GID;
        public string DrawDate;
        public string DrawTime;
        public string Status;
        public string Result;
        public string XF;
        public string TotWin;
    }

    [System.Serializable]
    public class WinDetails
    {
        public string GIDGName;
        public string TicketID;
        public string Win;
        public string DrawDate;
        public string DrawTime;
    }


}

[System.Serializable]
public class BookTicketDetails
{
    public string UserID;
    public string GameID;
    public string Draw;
    public BetsDetails[] Bets;

    [System.Serializable]
    public class BetsDetails
    {
        public string Digit;
        public string Qty;
        public string SDigit;

        public BetsDetails(string digit, string qty, string sDigit)
        {
            Digit = digit;
            Qty = qty;
            SDigit = sDigit;
        }
    }

    public BookTicketDetails(string userID, string gameID, string draw, BetsDetails[] bets)
    {
        UserID = userID;
        GameID = gameID;
        Draw = draw;
        Bets = bets;
    }
}