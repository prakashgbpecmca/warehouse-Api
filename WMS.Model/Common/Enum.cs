using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Common
{
    public class WMSOrderDeliveryTypeEnum
    {
        public const string StandardDelivery = "StandardDelivery";
        public const string StandardDeliveryDisplay = "Standard Delivery";
        public const string ExpressDelivery = "ExpressDelivery";
        public const string ExpressDeliveryDisplay = "Express Delivery";
        public const string UrgentDelivery = "UrgentDelivery";
        public const string UrgentDeliveryDisplay = "Urgent Delivery";
    }
    public enum WMSOrderStatusEnum
    {
        Draft = 1,
        NewOrder, //2
        Processed, //3
        Rejected, //4
        SampleCreation, //5
        InProduction, //6
        ReadyForDispatch, //7
        Shipped, //8 

    }
    public enum WMSOrderDesignStatusEnum
    {

        NewDesign = 10,
        DesignProcessed, //11
        DesignRejected, //12
        DesignCreation, //13 
        DesignSampleRekjected // 14
    }
    public enum WMSUserRoleEnum
    {
        Admin = 1,
        Sales, //2
        Customer, //3
        SalesCoOrdinator, //4
        Merchandise, //5
        Warehouse, //6
        SalesRepresentative

    }

    public class WMSOrdersUnit
    {
        public const string Unit = "kg";
        public const string Unit2 = "gm";
    }

    public class StockTypeEnum
    {
        public const string SockRange = "SockRange";
        public const string StockRange = "StockRange";
    }
    public class EmblishmentOptionEnum
    {
        public const string UpperArmRightType = "UpperArmRight";
        public const string UpperArmRightDisplay = "Right Hand Sleeve (RHS)";
        public const string LowerArmRightType = "LowerArmRight";
        public const string LowerArmRightDisplay = "Lower Arm Right";
        public const string UpperArmLeftType = "UpperArmLeft";
        public const string UpperArmLeftDisplay = "Left Hand Sleeve (LHS)";
        public const string LowerArmLeftType = "LowerArmLeft";
        public const string LowerArmLeftDisplay = "Lower ArmLeft ";
        public const string ChestCenterTopType = "ChestCenterTop";
        public const string ChestCenterTopDisplay = "Chest Front (CF)";
        public const string RightChestType = "RightChest";
        public const string RightChestDisplay = "Right Hand Chest (RHC)";
        public const string LeftChestType = "LeftChest";
        public const string LeftChestDisplay = "Left Hand Chest (LHC)";
        public const string FrontMainType = "FrontMain";
        public const string FrontMainDisplay = "Front Main";
        public const string BackNeckType = "BackNeck";
        public const string BackNeckDisplay = "Top Back (TB)";
        public const string PlayerNameType = "PlayerName";
        public const string PlayerNameDisplay = "Player Name";
        public const string PlayerNumberType = "PlayerNumber";
        public const string PlayerNumberDisplay = "Player Number";
        public const string MidBackType = "MidBack";
        public const string MidBackDisplay = "Mid Back";
        public const string LowerBackType = "LowerBack";
        public const string LowerBackDisplay = "Lower Back";
        public const string LowerSleeveRightType = "LowerSleeveRight";
        public const string LowerSleeveRightDisplay = "Lower Sleeve Right";
        public const string LowerSleeveLeftType = "LowerSleeveLeft";
        public const string LowerSleeveLeftDisplay = "Lower Sleeve Left";
        public const string ThighRightType = "ThighRight";
        public const string ThighRightDisplay = "Thigh Right";
        public const string ThighLeftType = "ThighLeft";
        public const string ThighLeftDisplay = "Front Left Thigh (FLT)";
        public const string BackThighLeftType = "BackThighLeft";
        public const string BackThighLeftDisplay = "Back Left Thigh (BLT)";
        public const string BackThighRightType = "BackThighRight";
        public const string BackThighRightDisplay = "Back Right Thigh (BRT)";
        public const string SockFrontRightType = "SockFrontRight";
        public const string SockFrontRightDisplay = "Sock Front Right";
        public const string SockFrontLeftType = "SockFrontLeft";
        public const string SockFrontLeftDisplay = "Sock Front Left";
        public const string SockCalfLeftType = "SockCalfLeft";
        public const string SockCalfLeftDisplay = "Sock Calf Left";
        public const string SockCalfRightType = "SockCalfRight";
        public const string SockCalfRightDisplay = "Sock Calf Right";
        public const string LeftShoulder = "LeftShoulder";
        public const string LeftShoulderDisplay = "Left Shoulder (LHSD)";
        public const string RightShoulder = "RightShoulder";
        public const string RightShoulderDisplay = "Right Shoulder (RHSD)";
        public const string UploadLogoType = "UploadLogo";
        public const string UplaodLogoDisplay = "Upload Logo";
        public const string InsertTextType = "InsertText";
        public const string InsertTextDisplay = "Insert Text";

    }
}
