using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Environment {
	public static class EnvironmentSettings {
		public static SectorGenerator initialSector;
		public static List<Sector> sectorList = new List<Sector>();
		public static List<Vector3> sectorPoints = new List<Vector3>();
		public static Controller player;
		public static int generationPasses = 3;
		public static int passCount = 0;
		public static Transform topParent;
		public static int sectorCount = 0;
		public static int maxSectors = 25;
		public static int boxCount = 0;
		public static int safeBoxCount = 0;
		public static float OveralTimer = 0;
		public static bool ActiveGame = true;
		
		public static void Generate() {
			sectorPoints.Add(Vector3.zero);
			initialSector.Generate();
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>();
		}

		public static void BreakSector(Sector nextBreak) {
			player.NextBreak(nextBreak);
		}
	}
}
