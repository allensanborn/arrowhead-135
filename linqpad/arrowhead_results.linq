<Query Kind="Program">
  <Connection>
    <ID>628f3d9d-bba6-4de2-99f1-ece942ca3f99</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="CsvLINQPadDriver" PublicKeyToken="no-strong-name">CsvLINQPadDriver.CsvDataContextDriver</Driver>
    <DisplayName>C:\Users\allen\Documents\code\github\arrowhead-135\data\arrowhead-results.csv (2023-02-04 11:04:00, 1 file 282.4 KB)</DisplayName>
    <DriverData>
      <Files>C:\Users\allen\Documents\code\github\arrowhead-135\data\arrowhead-results.csv
</Files>
      <HeaderDetection>HasHeader</HeaderDetection>
    </DriverData>
  </Connection>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	var results = arrowhead_results
	.Select(row => new
	{
		year = row.year,
		category = row.category,
		position = row.position,
		name = row.name,
		lastname = row.lastname,
		firstname = row.firstname,
		gender = row.gender,
		age = row.age,
		bib = row.bib,
		state_province_country = row.state_province_country,
		city = row.city,
		country = row.country,
		start_time = ConvertCellToDateTime(row, "start"),
		gateway_enter_time = ConvertCellToDateTime(row, "gateway_enter"),
		gateway_exit_time = ConvertCellToDateTime(row, "gateway_exit"),
		gateway_time_duration = CalculateDuration(row, "gateway_enter", "gateway_exit"),
		melgeorges_enter_time = ConvertCellToDateTime(row, "melgeorges_enter"),
		melgeorges_exit_time = ConvertCellToDateTime(row, "melgeorges_exit"),
		melgeorges_time_duration = CalculateDuration(row, "melgeorges_enter", "melgeorges_exit"),
		myrtle_lake_enter_time = ConvertCellToDateTime(row, "myrtle_lake_enter"),
		myrtle_lake_exit_time = ConvertCellToDateTime(row, "myrtle_lake_exit"),
		myrtle_lake_time_duration = CalculateDuration(row, "myrtle_lake_enter", "myrtle_lake_exit"),
		crescent_enter_time = ConvertCellToDateTime(row, "crescent_enter"),
		crescent_exit_time = ConvertCellToDateTime(row, "crescent_exit"),
		crescent__time_duration = CalculateDuration(row, "crescent_enter", "crescent_exit"),
		ski_pulk_enter_time = ConvertCellToDateTime(row, "ski_pulk_enter"),
		ski_pulk_exit_time = ConvertCellToDateTime(row, "ski_pulk_exit"),
		ski_pulk_time_duration = CalculateDuration(row, "ski_pulk_enter", "ski_pulk_exit"),
		finish_time = ConvertCellToDateTime(row, "finish_time"),
		overall_time_duration = row.overall_duration,
		overall_time_duration_computed = CalculateDuration(row, "start", "finish_time"),
		comments = row.comments,
		unsupported = row.unsupported
	});
	string tempFileName = Path.GetDirectoryName(Util.CurrentQueryPath) + "\\arrowhead_results_computed.csv";
	tempFileName.Dump();
	Util.WriteCsv(results, tempFileName);
	//results.Dump();
	results
	.Where(r => (!string.IsNullOrEmpty(r.overall_time_duration) && string.IsNullOrEmpty(r.overall_time_duration_computed))
	|| (string.IsNullOrEmpty(r.overall_time_duration) && !string.IsNullOrEmpty(r.overall_time_duration_computed)))
	.Dump();
}

// You can define other methods, fields, classes and namespaces here
string CalculateDuration(Rarrowhead_result row, string cellname_enter, string cellname_exit)
{
	string enter = ConvertCellToDateTime(row, cellname_enter);
	string exit = ConvertCellToDateTime(row, cellname_exit);
	if (string.IsNullOrEmpty(row[cellname_enter]) || string.IsNullOrEmpty(row[cellname_exit]))
	{
		return null;
	}
	DateTime enterdt;
	DateTime exitdt;
	DateTime.TryParse(enter, out enterdt);
	DateTime.TryParse(exit, out exitdt);

	var timespan = exitdt - enterdt;
	string result = (int)timespan.TotalHours + timespan.ToString(@"\:mm\:ss");
	return result;
}

string ConvertCellToDateTime(Rarrowhead_result row, string cellname)
{
	var cultureInfo = new CultureInfo("en-US");
	var value = row[cellname].Trim();
	string[] input = new[] { value, row["year"] };
	if (string.IsNullOrEmpty(value))
	{
		return null;
	}
	else
	{
		var arr = value.Split(" ");
		if (arr.Length == 0)
		{
			return null;
		}
		else
		{
			var date = arr[0];
			var dateArray = date.Split("/");
			DateTime result = new DateTime();
			int year = 0, month = 0, day = 0, hour = 0, minute = 0;
			switch (dateArray.Length)
			{
				case 0:
					throw new Exception("should not have a date array of length 0 here");
				case 1:
					throw new Exception("should not have a date array of length 1 here");
				case 2:
					// day / month					
					year = row["year"].ToInt().Value;
					month = dateArray[0].ToInt().Value;
					day = dateArray[1].ToInt().Value;
					break;
				case 3:
					// day / month / year
					year = dateArray[2].ToInt().Value;
					month = dateArray[0].ToInt().Value;
					day = dateArray[1].ToInt().Value;
					break;
			}
			var time = arr[1].Trim();
			var timeArray = time.Split(":");
			switch (timeArray.Length)
			{
				case 0:
					throw new Exception("should not have a time array of length 0 here");
				case 1:
					throw new Exception("should not have a time array of length 1 here");
				case 2:
					hour = timeArray[0].ToInt().Value;
					minute = timeArray[1].ToInt().Value;
					break;
			}
			result = new DateTime(year, month, day, hour, minute, 0);
			return result.ToString("o");
		}
	}
}