//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CanteenVanLang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class FOOD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FOOD()
        {
            this.MENUs = new HashSet<MENU>();
        }
    
        public int ID { get; set; }
        public string FOOD_CODE { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên món ăn")]
        [MaxLength(50, ErrorMessage = "Số lượng ký tự không thể nhiều hơn 50. Vui lòng nhập lại.")]
        public string FOOD_NAME { get; set; }

        [Range(0, 1000000, ErrorMessage = "Giá chỉ bao gồm số và nhỏ hơn 1 000 000. Vui lòng nhập lại.")]
        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public int PRICE { get; set; }
        public bool STATUS { get; set; }

        [MaxLength(500, ErrorMessage = "Số lượng ký tự không thể nhiều hơn 500. Vui lòng nhập lại.")]
        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string DESCRIPTION { get; set; }
        public string IMAGE_URL { get; set; }
        public Nullable<int> CATEGORY_ID { get; set; }
    
        public virtual CATEGORY CATEGORY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENU> MENUs { get; set; }
    }
}
