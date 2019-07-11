using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;

namespace FSWCopyMove.Data
{
    public class DbObjet
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Name = "IDT", IsDbGenerated = true, IsPrimaryKey = true, DbType = "INTEGER")]
        [Key]
        public int Idt { get; set; }
    }
}
