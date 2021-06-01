using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedDial.Meds
{
    public class Polvo : BaseMedication
	{
		public override string WorldModel => "models/abilities/sm_candy.vmdl";
		public override float rotationSpeed => 75f;
		public override string drugName => "POLVO";
		public override float drugDuration => 4f;
		public override DrugType drug => DrugType.Polvo;
	}
}
