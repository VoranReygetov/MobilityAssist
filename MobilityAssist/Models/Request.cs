//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MobilityAssist.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Request
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Request()
        {
            this.Responces = new HashSet<Responce>();
        }
    
        public int request_id { get; set; }
        public int user_id { get; set; }
        public int address_id { get; set; }
        public Nullable<int> destination_id { get; set; }
        public int help_id { get; set; }
        public bool req_status { get; set; }
        public string req_desc { get; set; }
        public System.DateTime req_date { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual Address Address1 { get; set; }
        public virtual HType HType { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Responce> Responces { get; set; }
    }
}
