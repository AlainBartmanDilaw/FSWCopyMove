using System.Data.Linq.Mapping;

namespace FSWCopyMove.Data
{
    [Table(Name = "Prm")]
    public class Prm : DbObjet
    {
        [Column(Name = "Nom", DbType = "VARCHAR")]
        public string Nom { get; set; }
        [Column(Name = "Lib", DbType = "VARCHAR")]
        public string Lib { get; set; }
    }
}
