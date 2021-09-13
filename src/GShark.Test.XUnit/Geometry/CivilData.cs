using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GShark.Test.XUnit.Geometry
{
	public class CivilData
	{
		#region properties
		public double Easting { get; set; }
		public double Northing { get; set; }
		public double Length { get; set; }
		public double StartStation { get; set; }
		public double EndStation { get; set; }
		public List<AlignmentICurve> Entities { get; set; }
		public ProfileData top { get; set; }
		public ProfileData bottom { get; set; }
		public ProfileData existing { get; set; }
		public List<ProfileData> otherProfiles { get; set; }
		public List<FeatureLineData> featureLines { get; set; }
		public List<SampleLineGroupData> SampleLineGroups { get; set; }
		#endregion
	}

	public class AlignmentICurve
	{
		public double StartStation { get; set; }
		public double EndStation { get; set; }
		public Point3 StartPoint { get; set; }
		public Point3 EndPoint { get; set; }
		public Point3 MidPoint { get; set; }
		public List<Point3> Points { get; set; }
		public string CurveType { get; set; }
	}

	public class ProfileData
	{
		public string Name { get; set; }
		public List<ProfilePoint> ProfilePoints { get; set; }
	}

	public class ProfilePoint
	{
		public double Station { get; set; }
		public Point3 Point { get; set; }
	}

	public class FeatureLineData
	{
		public string Name { get; set; }
		public List<Point3> Points { get; set; }
	}

	public class SampleLineData
	{
		public string Name { get; set; }
		public Point3 StartPoint { get; set; }
		public Point3 MidPoint { get; set; }
		public Point3 EndPoint { get; set; }
	}

	public class SampleLineGroupData
	{
		public string Name { get; set; }
		public List<SampleLineData> SampleLines { get; set; }
	}

}
