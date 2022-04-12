namespace Assets.Scripts.Core
{
    [System.Serializable]
    public class PayeeDetails
    {
        public Ifnsdetails ifnsDetails;
        public Sproudetails sprouDetails;
        public Sprofdetails sprofDetails;
        public Form form;
        public Payeedetails payeeDetails;
        public int nextStep;
        public int step;
    }

    [System.Serializable]
    public class Ifnsdetails
    {
        public string ifnsCode;
        public string ifnsName;
        public string ifnsInn;
        public string ifnsKpp;
        public string ifnsAddr;
        public string ifnsPhone;
        public string ifnsComment;
        public string sprof;
        public string sprou;
    }

    [System.Serializable]
    public class Sproudetails
    {
        public string ifnsCode;
        public string sproCode;
        public string sproName;
        public string sproAddr;
        public string sproPhone;
        public string sproComment;
    }

    [System.Serializable]
    public class Sprofdetails
    {
        public string ifnsCode;
        public string sproCode;
        public string sproName;
        public string sproAddr;
        public string sproPhone;
        public string sproComment;
    }

    [System.Serializable]
    public class Form
    {
        public string step;
        public string oktmmf;
        public string ifns;
        public string npKind;
    }

    [System.Serializable]
    public class Payeedetails
    {
        public string bankName;
        public string bankBic;
        public string correspAcc;
        public string payeeAcc;
        public string payeeName;
        public string payeeInn;
        public string payeeKpp;
    }
}
