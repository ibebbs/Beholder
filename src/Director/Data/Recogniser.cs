using PetaPoco;
using System;

namespace Director.Data
{

    [ExplicitColumns]
    [TableName("recogniser")]
    [PrimaryKey("id", AutoIncrement = false)]
    public class Recogniser
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
