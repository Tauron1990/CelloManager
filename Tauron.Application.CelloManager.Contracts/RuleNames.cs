namespace Tauron.Application.CelloManager
{
    public static class RuleNames
    {
        #region Historie

        public const string GetCommittedRefillRule = "GetCommittedRefillRule";
        public const string PurgeRule              = "PurgeRule";
        public const string PlaceOrderRule         = "PlaceOrderRule";
        public const string CompledRefillRule      = "CompledRefillRule";
        public const string IsRefillNeededRule     = "IsRefillNeededRule";
        public const string GetPageCount           = "GetPageCount";
        public const string GetPageItems           = "GetPageItems";

        #endregion

        #region Spools

        public const string GetSpoolRule      = "GetSpoolRule";
        public const string SpoolEmptyRule    = "SpoolEmptyRule";
        public const string AddSpoolRule      = "AddSpoolRule";
        public const string UpdateSpoolsRules = "UpdateSpoolsRules";
        public const string AddAmountRule     = "AddAmountRule";
        public const string RemoveSpoolRule   = "RemoveSpoolRule";

        #endregion

        public const string RefillPrinterRule = "RefillPrinterRule";
    }
}