using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMS.Models
{
    [MetadataType(typeof(TrainingCoursCustom))]
    public partial class TrainingCours
    {

    }
    public class TrainingCoursCustom
    {
        public int TrainingCourseID { get; set; }
        public string TrainingCourseName { get; set; }
        public string ShortDescription { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public string TrainingImage { get; set; }
        public bool IsActive { get; set; }
        //[Required]
        [DataType(DataType.Currency)]
        public Nullable<decimal> Price { get; set; }
        public Nullable<System.DateTime> StartDate1 { get; set; }
        public Nullable<System.DateTime> StartDate2 { get; set; }
        public Nullable<System.DateTime> EndDate1 { get; set; }
        public Nullable<System.DateTime> EndDate2 { get; set; }
        public Nullable<int> DiscountDate1 { get; set; }
        public Nullable<int> DiscountDate2 { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> FirstPayment { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> SecondPayment { get; set; }
        public string Coupon1 { get; set; }
        public string Coupon2 { get; set; }
        public string Coupon3 { get; set; }
        public string Coupon4 { get; set; }
        public string Coupon5 { get; set; }
        public Nullable<int> DiscountCoupon1 { get; set; }
        public Nullable<int> DiscountCoupon2 { get; set; }
        public Nullable<int> DiscountCoupon3 { get; set; }
        public Nullable<int> DiscountCoupon4 { get; set; }
        public Nullable<int> DiscountCoupon5 { get; set; }
        public string KeyWords { get; set; }
        public string ImgName { get; set; }
        public string ImgPath { get; set; }
        public virtual User User { get; set; }
        [AllowHtml]
        public string FullDescription { get; set; }
        [AllowHtml]
        public byte[] FullDescriptionBlob { get; set; }
        public Nullable<decimal> PriceFullPayment { get; set; }
        public Nullable<System.DateTime> LimitFullPaymentDate { get; set; }
        //[Required]
        public string SKU { get; set; }
        [AllowHtml]
        public string MeetingDetails { get; set; }
        [AllowHtml]
        public byte[] MeetingDetailsBlob { get; set; }
        public Nullable<int> MaxDiscountPayment1 { get; set; }
        public Nullable<int> MaxDiscountTotalPayment { get; set; }
        public Nullable<int> TrainingTypeID { get; set; }

        public static explicit operator TrainingCoursCustom(TrainingCours v)
        {
            throw new NotImplementedException();
        }
    }
}