using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Models
{
    public class ShippingModels
    {
    }
    //Shipping Request Models
    public class ShippingRequestModels
    {
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ShipStartDate { get; set; }
        public string ShipEndDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string EventID { get; set; }
        public string MKTID { get; set; }
        public string ManifestID { get; set; }
        public string OrderStatus { get; set; }
        public string TrackingNo { get; set; }
        //public string OrderSource { get; set; }
    }

    //Shipping Response Models
    public class ShippingListResponseModels
    {
        public int rcode { get; set; }
        public string rmessage { get; set; }
        public List<ShippingList> Result { get; set; }
    }
    public class ShippingList
    {
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public string MKTID { get; set; }
        public string EventID { get; set; }
        public string ManifestID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EntryDate { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string FullAddress { get; set; }
        public string RptStatus { get; set; }
        public string TrackingNumber { get; set; }
        public string ShipDate { get; set; }
        public string ShipCost { get; set; }
        public string TrackingNumberWithDate { get; set; }
        public string TrackingNumberWithDateLink { get; set; }
        public string ShipType { get; set; }
        public string Comments { get; set; }
        public string PiecePerSKU { get; set; }
        public List<SKUList> SKUDetails { get; set; }
    }
    public class SKUList
    {
        public string MID { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string OrderQuantity { get; set; }
        public string ShippedQuantity { get; set; }
        public string BKOQuantity { get; set; }
        public string TrackNumber { get; set; }
        public string ShipDate { get; set; }
    }
}
