using PetaPoco;
using System;

namespace Director.Data
{
    [ExplicitColumns]
    [TableName("recognition")]
    [PrimaryKey("id", AutoIncrement = false)]
    public class Recognition
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("face_id")]
        public Guid FaceId { get; set; }

        [Column("recogniser_id")]
        public Guid RecogniserId { get; set; }

        [Column("label")]
        public string Label { get; set; }

        [Column("confidence")]
        public float Confidence { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }
    }
}
