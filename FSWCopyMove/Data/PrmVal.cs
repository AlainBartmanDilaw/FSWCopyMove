using System.Data.Linq.Mapping;

namespace FSWCopyMove.Data
{
    [Table(Name = "PrmVal")]
    public class PrmVal : DbObjet
    {
        [Column(Name = "Prm_Idt", DbType = "INTEGER")]
        public int Prm_Idt { get; set; }
        [Column(Name = "Seq_Num", DbType = "INTEGER")]
        public int Seq_Num { get; set; }
        [Column(Name = "Val", DbType = "VARCHAR")]
        public string Val { get; set; }

    }
}
