using PetaPoco;
using System;

namespace Director.Data
{
    [ExplicitColumns]
    [TableName("faces")]
    [PrimaryKey("id", AutoIncrement = false)]
    public class Face
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("uri")]
        public string Uri { get; set; }

        [Column("location")]
        public string Location { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }
    }
}
