using System.Collections.Generic;
using UnityEngine;

namespace Environment {
	public static class EnvironmentSettings {
		public static SectorGenerator initialSector;
		public static List<Sector> sectorList = new List<Sector>();
		public static List<Vector3> sectorPoints = new List<Vector3>();
		public static int generationPasses = 3;
		public static int passCount = 0;
		public static Transform topParent;

		public static void Generate() {
			sectorPoints.Add(Vector3.zero);
			initialSector.Generate();
		}
	}
}
