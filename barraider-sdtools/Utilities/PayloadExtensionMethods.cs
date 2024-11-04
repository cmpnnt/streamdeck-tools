using BarRaider.SdTools.Payloads;

namespace BarRaider.SdTools.Utilities
{
    internal static class PayloadExtensionMethods
    {
        internal static string ToStringEx(this ReceivedSettingsPayload rsp)
        {
            return rsp == null ? 
                "ReceiveSettingsPayload is null!" : 
                $"IsInMultiAction: {rsp.IsInMultiAction} Coordinates: ({rsp.Coordinates?.Row},{rsp.Coordinates?.Column}) Settings: {rsp.Settings}";
        }

        internal static string ToStringEx(this AppearancePayload ap)
        {
            return ap == null ? 
                "AppearancePayload is null!" :
                $"State: {ap.State} IsInMultiAction: {ap.IsInMultiAction} Coordinates: ({ap.Coordinates?.Row},{ap.Coordinates?.Column}) Settings: {ap.Settings}";
        }

        internal static string ToStringEx(this KeyPayload kp)
        {
            return kp == null ? 
                "KeyPayload is null!" : 
                $"State: {kp.State} IsInMultiAction: {kp.IsInMultiAction} DesiredState: {kp.UserDesiredState} Coordinates: ({kp.Coordinates?.Row},{kp.Coordinates?.Column}) Settings: {kp.Settings}";
        }

        internal static string ToStringEx(this ReceivedGlobalSettingsPayload gsp)
        {
            return gsp == null ? 
                "ReceiveGlobalSettingsPayload is null!" : 
                $"Settings: {gsp.Settings}";
        }

        internal static string ToStringEx(this DialRotatePayload drp)
        {
            return drp == null ? 
                "DialRotatePayload is null!" : 
                $"Controller: {drp.Controller} Ticks: {drp.Ticks} Coordinates: ({drp.Coordinates?.Row},{drp.Coordinates?.Column}) Settings: {drp.Settings}";
        }

        internal static string ToStringEx(this DialPayload dpp)
        {
            return dpp == null ? 
                "DialPressPayload is null!" : 
                $"Controller: {dpp.Controller} Coordinates: ({dpp.Coordinates?.Row},{dpp.Coordinates?.Column}) Settings: {dpp.Settings}";
        }

        internal static string ToStringEx(this TouchpadPressPayload tpp)
        {
            return tpp == null ? 
                "KeyPayload is null!" : 
                $"Controller: {tpp.Controller} LongPress: {tpp.IsLongPress} Position: {(tpp.TapPosition?.Length == 2 ? tpp.TapPosition[0] + "," + tpp.TapPosition[1] : "Invalid")} Coordinates: ({tpp.Coordinates?.Row},{tpp.Coordinates?.Column}) Settings: {tpp.Settings}";
        }
    }
}
